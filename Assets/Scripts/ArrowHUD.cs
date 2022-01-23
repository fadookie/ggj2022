using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowHUD : MonoBehaviour
{
    [SerializeField] private Indicator arrowTemplate;
    private List<Indicator> arrows = new List<Indicator>();
    [SerializeField] private float minDistanceFromPlayerForArrow;
    [SerializeField] private float arrowBoxPadding;
    
    // Start is called before the first frame update
    void Start()
    {
        arrowTemplate.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var maxArrows = 0;

        if (CameraController.TryGetInstance(out var cameraController))
        {
            var player = Player.Instance;
            var preyNpcs = NPCManager.Instance.GetNPCsByColor(
                player.CurrentColor.GetOpposite()
            );

            Bounds screenBounds = cameraController.GetScreenInWorldSpace();
            Bounds screenBoundsPlus = screenBounds;
            screenBoundsPlus.size += Vector3.one * .5f;
            var closestPrey = preyNpcs
                .Where(npc => !screenBoundsPlus.Contains(npc.transform.position))
                .Select(npc => (npc: npc, closestPoint : screenBoundsPlus.ClosestPoint(npc.transform.position)))
                .Select(tuple => (npc:tuple.npc, closestPoint: screenBounds.ClosestPoint(tuple.closestPoint), distance : Vector3.Distance(tuple.closestPoint, tuple.npc.transform.position)))
                .OrderBy(tuple => tuple.distance)
                .Take(10)
                .ToList();

            maxArrows = closestPrey.Count;
            for (var i = 0; i < maxArrows; ++i)
            {
                while(i>= arrows.Count)
                {
                    arrows.Add(Instantiate(arrowTemplate, arrowTemplate.transform.parent));
                }

                var arrow = arrows[i];
                arrow.gameObject.SetActive(true);
                var npc = closestPrey[i].npc;
                var distance = closestPrey[i].distance;
                var closestPoint = closestPrey[i].closestPoint;
                arrow.Setup(npc.Color, Mathf.Clamp01(4 / (distance + 4) * .8f + .2f), closestPoint);
            }
        }
        for (var i = maxArrows; i < arrows.Count; ++i)
        {
            arrows[i].gameObject.SetActive(false);
        }
    }

    private static Vector3 SmoothIfInCorner(Vector3 point, Bounds bounds, float radius)
    {
        bounds.size -= Vector3.one * radius;
        var min = bounds.min;
        var max = bounds.max;
        bool BeyondLimit(float value, float min, float max, out float limit)
        {
            if (value > max)
            {
                limit = max;
                return true;
            }
            else if (value < min)
            {
                limit = min;
                return true;
            }
            limit = 0;
            return false;
        }
        if(BeyondLimit(point.x, min.x, max.x, out float x) && BeyondLimit(point.z, min.z, max.z, out float z))
        {
            var corner = new Vector3(x, point.y, z);
            return ((point - corner)).normalized * radius + corner;
        }
        return point;
    }
}
