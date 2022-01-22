using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class WorldGeneration : MonoBehaviour
{

    [SerializeField] bool rebuild;
    [SerializeField] bool clear;


    [SerializeField] NavMeshSurface navMeshSurface;

    [SerializeField] Transform wallParent;
    [SerializeField] private Wall wallPrefab;
    [SerializeField] private int wallPerXSquareUnits = 200;
    [SerializeField] private float wallMinLength;
    [SerializeField] private float wallMaxLength;



    [SerializeField] Transform npcParent;
    [SerializeField] private NPC npcPrefab;
    [SerializeField] private int npcPerXSquareUnits = 200;

    [SerializeField] Vector2 mapSize = new Vector2(100, 100);

    [SerializeField] bool built;

    protected private void Start()
    {
        DelayedBake();
    }

    protected void Update()
    {
        if (rebuild || (Application.isPlaying && !built))
        {
            rebuild = false;

            Buid();
        }
        if(clear)
        {
            clear = false;

            Clear();
        }
    }

    private void Clear()
    {
        while (wallParent.childCount > 0)
        {
            DestroyImmediate(wallParent.GetChild(0).gameObject);
        }
        while (npcParent.childCount > 0)
        {
            DestroyImmediate(npcParent.GetChild(0).gameObject);
        }

        built = false;
    }

    private void Buid()
    {
        Clear();

        int wallCount = Mathf.CeilToInt((mapSize.x * mapSize.y) / wallPerXSquareUnits);
        for (int i = 0; i < wallCount; i++)
        {
            var position = new Vector3((Random.value - .5f) * mapSize.x, 0, (Random.value - .5f) * mapSize.y);
            bool horizontal = Random.value < .5f;
            float length = Random.Range(wallMinLength, wallMaxLength);
            Vector3 size = new Vector3(horizontal ? length : 1, 1, horizontal ? 1 : length);

            var wall = ImprovedInstantiate(wallPrefab, position, wallParent);
            wall.Setup(GameColorUtil.GetRandomGameColor(), size);
        }

        int npcCount = Mathf.CeilToInt((mapSize.x * mapSize.y) / npcPerXSquareUnits);
        for (int i = 0; i < npcCount; i++)
        {
            var position = new Vector3((Random.value - .5f) * mapSize.x, 0, (Random.value - .5f) * mapSize.y);

            var npc = ImprovedInstantiate(npcPrefab, position, npcParent);
            npc.Setup(GameColorUtil.GetRandomGameColor());
        }
        DelayedBake();
        built = true;
    }

    private void DelayedBake()
    {
        if (Application.isPlaying)
        {
            IEnumerator Routine()
            {
                yield return null;
                navMeshSurface.BuildNavMesh();
            }
            StartCoroutine(Routine());
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += ()=> navMeshSurface.BuildNavMesh();
#endif
        }
    }

    private static T ImprovedInstantiate<T>(T prefab, Vector3 position, Transform parent)
        where T : MonoBehaviour
    {
        if(Application.isPlaying)
        {
            return Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }
        else
        {
#if UNITY_EDITOR
            var thing = (T)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
            thing.transform.position = position;
            return thing;
#endif
        }
    }
}
