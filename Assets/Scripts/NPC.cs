using System;
using System.Collections;
using System.Collections.Generic;
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
    private NavMeshAgent navMeshAgent;

    private bool dead;

    protected void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Player.Instance.ObservablePosition.CombineLatest(Player.Instance.ObservableColor, Tuple.Create).Subscribe(tuple => {
            var playerPos = tuple.Item1;
            var playerColor = tuple.Item2;
            if (!playerColor.Equals(color)) {
                Debug.Log(string.Format("NPC {0} run away from player {1}", color, playerColor));
                // run away
                var awayFromPlayer = (transform.position - playerPos).normalized * 20;
                NavMeshHit closestHit;
                if (NavMesh.SamplePosition(awayFromPlayer, out closestHit, 100f,
                    navMeshAgent.areaMask)) {
                    navMeshAgent.destination = closestHit.position;
                }
            } else {
                Debug.Log(string.Format("NPC {0} chase player {1}", color, playerColor));
                // chase player
                navMeshAgent.destination = playerPos;
            }
        }).AddTo(this);
    }

    protected void Update()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = GetMaterial();
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

    public void Die()
    {
        Destroy(gameObject);
    }
}
