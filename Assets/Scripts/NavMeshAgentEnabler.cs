using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshAgentEnabler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        UnityEngine.AI.NavMeshHit closestHit;
        if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out closestHit, 100f,
            UnityEngine.AI.NavMesh.AllAreas)) {
            transform.position = closestHit.position;
//            agent.enabled = true;
            Debug.Log("Fixed NavMesh position");
        } else {
            Debug.LogWarning("Critical failure to fix NavMesh position");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
