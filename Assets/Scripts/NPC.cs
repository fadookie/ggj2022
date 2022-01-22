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

    private bool dead;

    private Coroutine wanderRoutine;

    public void Setup(GameColor gameColor)
    {
        color = gameColor;
    }

    protected void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected void Update()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = GetMaterial();
        }
        gameObject.layer = GetNPCLayer();

        var playerPos = Player.Instance.ObservablePosition.Value;
        var playerColor = Player.Instance.CurrentColor;
        var distanceToPlayer = Vector3.Distance(playerPos, transform.position);
        if (distanceToPlayer > playerNoticeDistance) {
            // Wander around
            if (wanderRoutine == null) {
                wanderRoutine = StartCoroutine(Wander());
            }
        } else {
            if (wanderRoutine != null) {
                StopCoroutine(wanderRoutine);
                wanderRoutine = null;
            }
                    if (!playerColor.Equals(color))
                    {
        //                Debug.Log(string.Format("NPC {0} run away from player {1}", color, playerColor));
                // run away
                var awayFromPlayer = (transform.position - playerPos).normalized * 20;
                NavMeshHit closestHit;
                if (NavMesh.SamplePosition(awayFromPlayer, out closestHit, 100f,
                            navMeshAgent.areaMask))
                        {
                    SafeSetDestination(closestHit.position);
                }
                    }
                    else
                    {
        //                Debug.Log(string.Format("NPC {0} chase player {1}", color, playerColor));
                // chase player
                SafeSetDestination(playerPos);
            }
        }
    }

    private void SafeSetDestination(Vector3 pos) {
        if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh) {
            navMeshAgent.destination = pos;
        } 
    }

    private IEnumerator Wander() {
        while(true) {
            const float randomDistance = 3f;
            var newTarget = transform.position + new Vector3(UnityEngine.Random.Range(-randomDistance, randomDistance), 0, UnityEngine.Random.Range(-randomDistance, randomDistance));
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(newTarget, out closestHit, 100f,
                navMeshAgent.areaMask)) {
                SafeSetDestination(closestHit.position);
            }
            yield return new WaitForSeconds(0.75f);
        }
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

    public void Die()
    {
        Destroy(gameObject);
    }
}
