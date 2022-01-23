using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[ExecuteAlways]
[RequireComponent(typeof(CharacterSprite))]
public class NPC : MonoBehaviour
{
    private static Dictionary<GameObject, NPC> lookup = new Dictionary<GameObject, NPC>();

    public GameColor Color
    {
        get => color.Value;
        private set => color.Value = value;
    }

    [SerializeField] private ReactiveProperty<GameColor> color;

    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float playerNoticeDistanceMin = 5f;
    [SerializeField] private float playerNoticeDistanceMax = 10f;
    [SerializeField] private float playerIgnoreDistanceMin = 10f;
    [SerializeField] private float playerIgnoreDistanceMax = 15f;

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float pathEndThreshold = 0.1f;
    private bool hasPath = false;

    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 2.75f;

    private Collider groundCollider;
    [SerializeField] private CharacterSprite characterSprite;

    private bool noticedPlayer;

    private float awarenessTimeOffset;
    private float awarenessVariationPeriod = 5;

    public void Setup(GameColor gameColor)
    {
        Color = gameColor;
        color.Subscribe(OnColorChange);
        characterSprite.color = gameColor;
        NPCManager.Instance.RegisterNPC(this);
    }

    protected void Start() {
        if (Application.isPlaying) {
            var ground = GameObject.FindGameObjectWithTag("Ground");
            groundCollider = ground.GetComponent<MeshCollider>();
        }
        Update();
    }

    protected void OnEnable()
    {
        lookup.Add(gameObject, this);
        awarenessTimeOffset = Random.value;
    }

    private const float tau = Mathf.PI * 2;

    private float lastUpdatedFleeTargetFlee = -999;

    protected void Update()
    {
        if (Application.isPlaying) {
            float awarenessLerp = Mathf.Sin((awarenessTimeOffset + Time.time / awarenessVariationPeriod) * tau);
            awarenessLerp  = (awarenessLerp + 1f)/ 2f;
            var playerIgnoreDistance = Mathf.Lerp(playerIgnoreDistanceMin, playerIgnoreDistanceMax, awarenessLerp);
            var playerNoticeDistance = Mathf.Lerp(playerNoticeDistanceMin, playerNoticeDistanceMax, awarenessLerp);
            var playerPos = Player.Instance.transform.position;

            { // debug
                Vector3 vectorTowardPlayer = (playerPos - transform.position).normalized;
                var noticePoint = transform.position + vectorTowardPlayer * playerNoticeDistance;
                var ignorePoint = noticePoint + vectorTowardPlayer * playerIgnoreDistance;
                Debug.DrawLine(transform.position, noticePoint, UnityEngine.Color.red);
                Debug.DrawLine(noticePoint, ignorePoint, UnityEngine.Color.yellow);
            }

            var playerColor = Player.Instance.CurrentColor;
            var distanceToPlayer = Vector3.Distance(playerPos, transform.position);
            bool notice = distanceToPlayer < playerNoticeDistance;
            bool ignore = distanceToPlayer > playerIgnoreDistance;

            if (ignore || !(notice || noticedPlayer) ) {
                // Wander around
                characterSprite.angry = false;
                if (noticedPlayer || AtEndOfPath() || !navMeshAgent.hasPath)
                {
                    noticedPlayer = false;
                    navMeshAgent.speed = walkSpeed;
                    var bounds = groundCollider.bounds;
                    var newTarget = new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), 0,
                        UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
                    //Debug.Log(string.Format("wander new target newTarget:{0} pathStatus:{1} pathPending:{2}", newTarget, navMeshAgent.pathStatus, navMeshAgent.pathPending));
                    SafeSetDestination(newTarget);
                }
            }
            else if(notice || noticedPlayer) {
                noticedPlayer = true;
                navMeshAgent.speed = runSpeed;
                if (playerColor != Color) {
                    // run away
                    characterSprite.angry = false;
                    if (lastUpdatedFleeTargetFlee < Time.time - 1f)
                    {
                        lastUpdatedFleeTargetFlee = Time.time;
                        var awayFromPlayer = (transform.position - playerPos).normalized * 20;
                        NavMeshHit closestHit;
                        if (NavMesh.SamplePosition(awayFromPlayer, out closestHit, 100f,
                            navMeshAgent.areaMask))
                        {
                            SafeSetDestination(closestHit.position);
                        }
                    }
                } else {
                    // chase player
                    characterSprite.angry = true;
                    SafeSetDestination(playerPos);
                }
            }
        }
    }

    protected void LateUpdate()
    {
        meshRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
        var scale = meshRenderer.transform.localScale;
        scale.x = Mathf.Abs(meshRenderer.transform.localScale.x) * ((transform.forward.x >= 0) ? 1 : -1);
        meshRenderer.transform.localScale = scale;
    }

    private void OnColorChange(GameColor color)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = GetMaterial();
        }
        gameObject.layer = GetNPCLayer();
        navMeshAgent.areaMask = GetNPCAreaMask();

        characterSprite.color = Color;
    }

    protected void OnDisable()
    {
        lookup.Remove(gameObject);
        NPCManager.Instance.UnregisterNPC(this);
    }

    private void SafeSetDestination(Vector3 pos) {
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) {
            if (!navMeshAgent.SetDestination(pos)) {
                Debug.LogError("Unable to set destination");
            }
        }
        else
        {
            Debug.LogError("navMeshAgent issue");
        }
    }
    
    bool AtEndOfPath()
    {
        hasPath |= navMeshAgent.hasPath;
        if (hasPath && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + pathEndThreshold )
        {
            // Arrived
            hasPath = false;
            return true;
        }
 
        return false;
    }
    

    private Material GetMaterial()
    {
        switch (Color)
        {
            case GameColor.Black: return blackMaterial;
            case GameColor.White: return whiteMaterial;
            default: throw new System.NotImplementedException();
        }
    }

    private int GetNPCLayer()
    {
        switch (Color)
        {
            case GameColor.Black: return LayerMask.NameToLayer("Black NPC");
            case GameColor.White: return LayerMask.NameToLayer("White NPC");
            default: throw new System.NotFiniteNumberException();
        }
    }


    private int GetNPCAreaMask()
    {
        switch (Color)
        {
            case GameColor.Black: return NavAreaTypeUtil.CreateMask(NavAreaType.Walkable, NavAreaType.Jump, NavAreaType.BlackDoor);
            case GameColor.White: return NavAreaTypeUtil.CreateMask(NavAreaType.Walkable, NavAreaType.Jump, NavAreaType.WhiteDoor);
            default: throw new System.NotFiniteNumberException();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public static bool TryGetNPC(GameObject gameObject, out NPC npc)
    {
        return lookup.TryGetValue(gameObject, out npc);
    }
}
