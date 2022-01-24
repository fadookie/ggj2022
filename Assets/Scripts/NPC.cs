using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float fleeSpeed = 2f;

    private CapsuleCollider capsuleCollider;
    private Collider groundCollider;
    [SerializeField] private CharacterSprite characterSprite;
    [SerializeField] private AudioSource audioSource;

    private bool noticedPlayer;
    private bool wasCollidingWithPlayer;

    private float awarenessTimeOffset;
    private float awarenessVariationPeriod = 5;

    public bool PendingPath => navMeshAgent.pathPending;

    public void Setup(GameColor gameColor)
    {
        Color = gameColor;
        color.Subscribe(OnColorChange);
        characterSprite.color = gameColor;
        NPCManager.Instance.RegisterNPC(this);
    }

    protected void Start() {
        capsuleCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
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

    private IEnumerator sizeRoutine;

    private Vector3 normalMeshScale = Vector3.one;

    enum AIState { Wander, Chase, Flee }

    private AIState aiState;

    protected void Update()
    {
        if (Application.isPlaying)
        {
            NPCManager.Instance.UpdateIsPendingPath(this, PendingPath);
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
            var distanceToPlayer = Vector2.Distance(playerPos.ToVector2(), transform.position.ToVector2());
            bool notice = distanceToPlayer < playerNoticeDistance;
            bool ignore = distanceToPlayer > playerIgnoreDistance;
            CollisionCheck(distanceToPlayer);

            if (ignore || !(notice || noticedPlayer) ) {
                // Wander around
                characterSprite.angry = false;
                if (noticedPlayer || AtEndOfPath() || !navMeshAgent.hasPath)
                {
                    if (aiState != AIState.Wander)
                    {
                        if (aiState == AIState.Chase) {
                            audioSource.PlayOneShot(AudioManager.Instance.GetAudioClip(AudioManager.Sound.NPCDeAggro));
                        }
                        aiState = AIState.Wander;
                        sizeRoutine = SizePulse(Size, 1);
                    }
                    noticedPlayer = false; 
                    navMeshAgent.ResetPath();
                    if (NPCManager.Instance.NPCsPendingPath < 10)
                    {
                        navMeshAgent.speed = walkSpeed;
                        var bounds = groundCollider.bounds;
                        var newTarget = new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), 0,
                            UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
                        //Debug.Log(string.Format("wander new target newTarget:{0} pathStatus:{1} pathPending:{2}", newTarget, navMeshAgent.pathStatus, navMeshAgent.pathPending));
                        SafeSetDestination(newTarget);
                    }
                }
            }
            else if(notice || noticedPlayer) {
                noticedPlayer = true;
                if (playerColor != Color) {
                    // run away
                    if (aiState != AIState.Flee)
                    {
                        aiState = AIState.Flee;
                        sizeRoutine = SizePulse(.85f, .85f);
                    }
                    navMeshAgent.speed = fleeSpeed;
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
                    if (aiState != AIState.Chase)
                    {
                        aiState = AIState.Chase;
                        sizeRoutine = SizePulse(1.25f, 1f);
                        audioSource.PlayOneShot(AudioManager.Instance.GetAudioClip(AudioManager.Sound.NPCAggro));
                    }
                    navMeshAgent.speed = chaseSpeed;
                    characterSprite.angry = true;
                    SafeSetDestination(playerPos);
                }
            }
            NPCManager.Instance.UpdateIsPendingPath(this, PendingPath);
            if(sizeRoutine != null && !sizeRoutine.MoveNext())
            {
                sizeRoutine = null;
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

    private float sizeVelocity;

    IEnumerator SizePulse(float initial, float final)
    {
        /*{
            float duration = .25f;
            float t = Time.time + duration;
            while (Time.time <= t)
            {
                Size = Mathf.SmoothDamp(Size, sizeGoal, ref sizeVelocity, 1);
                yield return null;
            }
        }*/
        Size = initial;
        {
            float duration = 1;
            float t = Time.time + duration + 1;
            while (Time.time <= t)
            {
                Size = Mathf.SmoothDamp(Size, final, ref sizeVelocity, 1);
                yield return null;
            }
        }
        sizeVelocity = 0;
        Size = final;
    }

    private float Size
    {
        get => meshRenderer.transform.localScale.z / normalMeshScale.z;
        set
        {
            var scale = normalMeshScale;
            scale.x = normalMeshScale.x * ((transform.forward.x >= 0) ? 1 : -1);
            meshRenderer.transform.localScale = scale * value;
        }
    }


    /**
     * Unity collision detection isn't working reliably for some reason so we'll just do it ourselves
     */
    void CollisionCheck(float distanceToPlayer) {
        if (distanceToPlayer <= Player.Instance.Radius + capsuleCollider.radius) {
            if (Player.Instance.isActiveAndEnabled && !wasCollidingWithPlayer) {
                Player.Instance.OnNPCCollisionEnter(this);
                wasCollidingWithPlayer = true;
            }
        } else {
            wasCollidingWithPlayer = false;
        }
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
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.UnregisterNPC(this);
        }

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
