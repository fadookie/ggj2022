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
    private struct GridTile
    {
        public bool occupied;
        public GameColor? color;
    }

    private GridTile[,] grid;


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
        grid = new GridTile[mapSize.x, mapSize.y];
        int wallCount = Mathf.CeilToInt((mapSize.x * mapSize.y) / wallPerXSquareUnits);
        RectInt mapBounds = GetMapBounds();
        RectInt wallDeadZone = new RectInt(Mathf.RoundToInt(mapSize.x / 2f) - 2, Mathf.RoundToInt(mapSize.y / 2f) - 2, 4, 4);
        MarkFilled(wallDeadZone);

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
                Vector2Int gridPosition = new Vector2Int(1+Mathf.FloorToInt(Random.Range(0, mapBounds.width - size.x -2) / 2f) * 2, 1+Mathf.FloorToInt(Random.Range(1, mapBounds.height - size.y-2) / 2f) * 2);
                RectInt shapeGridBounds = new RectInt(gridPosition, size);
                RectInt expandedShapeGridBounds = shapeGridBounds;
                expandedShapeGridBounds.xMin -= 1;
                expandedShapeGridBounds.yMin -= 1;
                expandedShapeGridBounds.xMax += 1;
                expandedShapeGridBounds.yMax += 1;
                if (IsOpen(shapeGridBounds) && IsOpen(expandedShapeGridBounds, gameColor))
                {
                    MarkFilled(shapeGridBounds, gameColor);
                    var wall = ImprovedInstantiate(wallPrefab, GridToWorld(shapeGridBounds.center), wallParent);
                    wall.Setup(gameColor, new Vector3(shapeGridBounds.size.x, 1, shapeGridBounds.size.y));
                    break;
                }
            }
        }

        yield return null;

        navMeshSurface.BuildNavMesh();

        yield return null;

        int npcCount = Mathf.CeilToInt((mapSize.x * mapSize.y) / npcPerXSquareUnits);
        GameColor lastNPCColor = GameColor.Black;//player color
        HashSet<Vector2Int> npcLocations = new HashSet<Vector2Int>();

        RectInt npcDeadZone = new RectInt(Mathf.RoundToInt(mapSize.x / 2f) - 8, Mathf.RoundToInt(mapSize.y / 2f) - 8, 16, 16);

        List<NPC> npcs = new List<NPC>();
        for (int i = 0; i < npcCount; i++)
        {
            int tries = 100;
            while (--tries > 0)
            {
                var position = new Vector2Int(Random.Range(0, mapBounds.width), Random.Range(0, mapBounds.height));
                var rect = new RectInt(position, Vector2Int.one);
                var color = lastNPCColor.GetOpposite();
                if (!npcLocations.Contains(position) && IsOpen(rect, color) && !npcDeadZone.Contains(position))
                {
                    MarkFilled(rect, color);
                    npcLocations.Add(position);
                    var npc = ImprovedInstantiate(npcPrefab, GridToWorld(rect.center), npcParent);
                    npc.gameObject.SetActive(false);
                    npcs.Add(npc);
                    lastNPCColor = color;
                    npc.Setup(color);

                    break;
                }
            }
        }
        built = true;
        npcs.Sort((a, b) => a.transform.position.sqrMagnitude.CompareTo(b.transform.position.sqrMagnitude));
        foreach (var npc in npcs)
        {
            npc.gameObject.SetActive(true);
            yield return null;
            while(npc.PendingPath)
            {
                yield return null;
            }
        }
        building = null;
    }
    private void MarkFilled(Vector2Int gridPosition, GameColor? gameColor = null)
    {
        grid[gridPosition.x, gridPosition.y].color = gameColor;
        grid[gridPosition.x, gridPosition.y].occupied = true;
    }

    private void MarkFilled(RectInt gridRect, GameColor? gameColor = null)
    {
        for (int x = gridRect.xMin; x < gridRect.xMax; x++)
        {
            for (int y = gridRect.yMin; y < gridRect.yMax; y++)
            {
                MarkFilled(new Vector2Int(x,y), gameColor);
            }
        }
    }

    private bool IsOpen(Vector2Int gridPosition, GameColor? gameColor = null)
    {
        var existing = grid[gridPosition.x, gridPosition.y];
        return (!existing.occupied || existing.color == gameColor);
    }

    private bool IsOpen(RectInt gridRect, GameColor? gameColor = null)
    {
        for (int x = gridRect.xMin; x < gridRect.xMax; x++)
        {
            for (int y = gridRect.yMin; y < gridRect.yMax; y++)
            {
                if(!IsOpen(new Vector2Int(x, y), gameColor))
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
