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
            var wall = Instantiate(wallPrefab, wallParent);
            wall.transform.position = new Vector3((Random.value - .5f) * mapSize.x, 0, (Random.value - .5f) * mapSize.y);
            bool horizontal = Random.value < .5f;
            float size = Random.Range(wallMinLength, wallMaxLength);
            wall.transform.localScale = new Vector3(horizontal ? size : 1, 1, horizontal ? 1 : size);
            wall.Setup(GameColorUtil.GetRandomGameColor());
        }

        int npcCount = Mathf.CeilToInt((mapSize.x * mapSize.y) / npcPerXSquareUnits);
        for (int i = 0; i < npcCount; i++)
        {
            var npc = Instantiate(npcPrefab, npcParent);
            npc.transform.position = new Vector3((Random.value - .5f) * mapSize.x, 0, (Random.value - .5f) * mapSize.y);
            npc.Setup(GameColorUtil.GetRandomGameColor());
        }
        navMeshSurface.BuildNavMesh();
        built = true;
    }

    new private static T Instantiate<T>(T prefab, Transform parent)
        where T : MonoBehaviour
    {
        if(Application.isPlaying)
        {
            return Object.Instantiate(prefab, parent);
        }
        else
        {
#if UNITY_EDITOR
            return (T)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
#endif
        }
    }
}
