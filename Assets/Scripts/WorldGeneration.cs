using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class WorldGeneration : MonoBehaviour
{
    private bool[,] grid;


    [SerializeField] bool rebuild;
    [SerializeField] bool clear;


    [SerializeField] NavMeshSurface navMeshSurface;

    [SerializeField] Transform wallParent;
    [SerializeField] private Wall wallPrefab;
    [SerializeField] private int wallPerXSquareUnits = 200;
    [SerializeField] private int wallMinLength;
    [SerializeField] private int wallMaxLength;



    [SerializeField] Transform npcParent;
    [SerializeField] private NPC npcPrefab;
    [SerializeField] private int npcPerXSquareUnits = 200;

    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);

    private RectInt GetMapBounds() => new RectInt(Mathf.CeilToInt(-mapSize.x / 2f), Mathf.CeilToInt(-mapSize.y / 2f), mapSize.x, mapSize.y);

    [SerializeField] bool built;

    IEnumerator building;

    protected private void Start()
    {
        if (built)
        {
            DelayedBake();
        }
        else Update();
    }

    protected void Update()
    {
        if (rebuild || (Application.isPlaying && building == null && !built))
        {
            rebuild = false;

            building = Buid();
        }

        if(clear)
        {
            clear = false;

            Clear();
        }
        if (building != null)
        {
            building.MoveNext();
        }
    }

    private void Clear(bool cancelBuild = true)
    {
        if(cancelBuild)
        {
            building = null;
        }

        while (wallParent.childCount > 0)
        {
            DestroyImmediate(wallParent.GetChild(0).gameObject);
        }
        while (npcParent.childCount > 0)
        {
            DestroyImmediate(npcParent.GetChild(0).gameObject);
        }
        navMeshSurface.RemoveData();
        built = false;
    }

    private IEnumerator Buid()
    {
        Clear(false);
        if (Ground.TryGetInstance(out Ground ground))
        {
            ground.Size = mapSize;
        }
        grid = new bool[mapSize.x, mapSize.y];
        int wallCount = Mathf.CeilToInt((mapSize.x * mapSize.y) / wallPerXSquareUnits);
        RectInt mapBounds = GetMapBounds();
        MarkFilled(new RectInt(Mathf.RoundToInt(mapSize.x / 2f) - 2, Mathf.RoundToInt(mapSize.y / 2f) - 2, 4, 4));

        Vector3 GridToWorld(Vector2 gridPosition)
        {
            return new Vector3(gridPosition.x + mapBounds.xMin, 0, gridPosition.y + mapBounds.yMin);
        }
        for (int i = 0; i < wallCount; i++)
        {
            GameColor gameColor = GameColorUtil.GetRandomGameColor();

            int tries = 100;
            while (--tries > 0)
            {
                bool horizontal = Random.value < .5f;
                int length = Random.Range(wallMinLength, wallMaxLength);
                Vector2Int size = new Vector2Int(horizontal ? length : 1, horizontal ? 1 : length);
                Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(Random.Range(0, mapBounds.width + 1 - size.x) / 2f) * 2, Mathf.FloorToInt(Random.Range(0, mapBounds.height + 1 - size.y) / 2f) * 2);
                RectInt gridBounds = new RectInt(gridPosition, size);

                if (IsOpen(gridBounds))
                {
                    MarkFilled(gridBounds);
                    var wall = ImprovedInstantiate(wallPrefab, GridToWorld(gridBounds.center), wallParent);
                    wall.Setup(gameColor, new Vector3(gridBounds.size.x, 1, gridBounds.size.y));
                    break;
                }
            }
        }

        yield return null;

        navMeshSurface.BuildNavMesh();

        yield return null;

        int npcCount = Mathf.CeilToInt((mapSize.x * mapSize.y) / npcPerXSquareUnits);
        GameColor lastNPCColor = GameColor.Black;//player color
        for (int i = 0; i < npcCount; i++)
        {
            int tries = 100;
            while (--tries > 0)
            {
                var position = new Vector2Int(Random.Range(0, mapBounds.width), Random.Range(0, mapBounds.height));
                var rect = new RectInt(position, Vector2Int.one);
                if (IsOpen(rect))
                {
                    MarkFilled(rect);
                    var npc = ImprovedInstantiate(npcPrefab, GridToWorld(rect.center), npcParent);
                    var color = lastNPCColor.GetOpposite();
                    lastNPCColor = color;
                    npc.Setup(color);
                    break;
                }

            }
        }
        built = true;
        building = null;
    }
    

    private void MarkFilled(RectInt gridRect)
    {
        for (int x = gridRect.xMin; x < gridRect.xMax; x++)
        {
            for (int y = gridRect.yMin; y < gridRect.yMax; y++)
            {
                grid[x, y] = true;
            }
        }
    }

    private bool IsOpen(Vector2Int gridPosition)
    {
        return !grid[gridPosition.x, gridPosition.y];
    }

    private bool IsOpen(RectInt gridRect)
    {
        for (int x = gridRect.xMin; x < gridRect.xMax; x++)
        {
            for (int y = gridRect.yMin; y < gridRect.yMax; y++)
            {
                if (grid[x, y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void DelayedBake(Action then = null)
    {
        if (Application.isPlaying)
        {
            IEnumerator Routine()
            {
                yield return null;
                navMeshSurface.BuildNavMesh();
                yield return null;
                then?.Invoke();
            }
            StartCoroutine(Routine());
        }
        else
        {
#if UNITY_EDITOR

            UnityEditor.EditorApplication.delayCall += () =>
            {
                navMeshSurface.BuildNavMesh();
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    then?.Invoke();
                };
            };
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
