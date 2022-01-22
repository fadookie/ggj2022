using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class NPC : MonoBehaviour
{
    public GameColor Color => color;
    [SerializeField] private GameColor color;

    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float playerNoticeDistance = 10f;
    
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float pathEndThreshold = 0.1f;
    private bool hasPath = false;

    private bool dead;

    private Coroutine wanderRoutine;
    private Collider groundCollider;

    public void Setup(GameColor gameColor)
    {
        color = gameColor;
    }

    protected void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (Application.isPlaying) {
            var ground = GameObject.FindGameObjectWithTag("Ground");
            groundCollider = ground.GetComponent<MeshCollider>();
        }
        Update();
    }

    protected void Update()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = GetMaterial();
        }
        gameObject.layer = GetNPCLayer();
        navMeshAgent.areaMask = GetNPCAreaMask();

        if (Application.isPlaying) {
            var playerPos = Player.Instance.ObservablePosition.Value;
            var playerColor = Player.Instance.CurrentColor;
            var distanceToPlayer = Vector3.Distance(playerPos, transform.position);
            if (distanceToPlayer > playerNoticeDistance) {
                // Wander around
                if (AtEndOfPath()) {
                    var bounds = groundCollider.bounds;
                    var newTarget = new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), 0,
                        UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
                    Debug.Log(string.Format("wander new target newTarget:{0} pathStatus:{1} pathPending:{2}", newTarget,
                        navMeshAgent.pathStatus, navMeshAgent.pathPending));
                    SafeSetDestination(newTarget);
                }
            } else {
                if (wanderRoutine != null) {
                    Debug.LogWarning("Wander stop");
                    StopCoroutine(wanderRoutine);
                    wanderRoutine = null;
                }
                if (!playerColor.Equals(color)) {
                    //                Debug.Log(string.Format("NPC {0} run away from player {1}", color, playerColor));
                    // run away
                    var awayFromPlayer = (transform.position - playerPos).normalized * 20;
                    NavMeshHit closestHit;
                    if (NavMesh.SamplePosition(awayFromPlayer, out closestHit, 100f,
                        navMeshAgent.areaMask)) {
                        SafeSetDestination(closestHit.position);
                    }
                } else {
                    //                Debug.Log(string.Format("NPC {0} chase player {1}", color, playerColor));
                    // chase player
                    SafeSetDestination(playerPos);
                }
            }
        }
    }

    private void SafeSetDestination(Vector3 pos) {
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) {
            if (!navMeshAgent.SetDestination(pos)) {
                Debug.LogError("Unable to set destination");
            }
        } 
    }

//    private IEnumerator Wander() {
//        Debug.Log("Wander routine start");
//        while(true) {
////            yield return new WaitForSeconds(1f);
////            NavMeshHit closestHit;
////            if (NavMesh.SamplePosition(newTarget, out closestHit, 100f,
////                navMeshAgent.areaMask)) {
////                Debug.Log(string.Format("wander SetDestination:{0}", closestHit.position));
////                SafeSetDestination(closestHit.position);
////            } else {
////                Debug.LogWarning("wander no closest point, Bail");
////            }
//            while(!AtEndOfPath()) {
//                yield return null;
//            }
//            Debug.LogWarning("wander path complete");
//        }
//    }
    
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
        switch (color)
        {
            case GameColor.Black: return blackMaterial;
            case GameColor.White: return whiteMaterial;
            default: throw new System.NotImplementedException();
        }
    }

    private int GetNPCLayer()
    {
        switch (color)
        {
            case GameColor.Black: return LayerMask.NameToLayer("Black NPC");
            case GameColor.White: return LayerMask.NameToLayer("White NPC");
            default: throw new System.NotFiniteNumberException();
        }
    }


    private int GetNPCAreaMask()
    {
        switch (color)
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
}
