using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowHUD : MonoBehaviour
{
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private float minDistanceFromPlayerForArrow;
    [SerializeField] private float arrowBoxPadding;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update() {
        var player = Player.Instance;
        var preyNpcs = NPCManager.Instance.GetNPCsByColor(
            player.CurrentColor.GetOpposite()
        );
        var closestPrey = preyNpcs
            .Select(npc => Tuple.Create(npc, Vector3.Distance(player.transform.position, npc.transform.position)))
            .OrderBy(tuple => tuple.Item2)
            .Where(tuple => tuple.Item2 > minDistanceFromPlayerForArrow)
            .Take(Math.Min(arrows.Length, preyNpcs.Count))
            .ToList();

        var maxArrows = Math.Min(arrows.Length, closestPrey.Count);
        for (var i = 0; i < maxArrows; ++i) {
            var arrow = arrows[i];
            var npc = closestPrey[i].Item1;
            var distanceToPlayer = closestPrey[i].Item2;
            var npcScreenPos = Camera.main.WorldToScreenPoint(npc.transform.position);
            var clampedScreenPos = new Vector3(
                Mathf.Clamp(npcScreenPos.x, arrowBoxPadding, Screen.width - arrowBoxPadding),
                Mathf.Clamp(npcScreenPos.y, arrowBoxPadding, Screen.height - arrowBoxPadding),
                0
            );
            var clampedWorldPos = Camera.main.ScreenToWorldPoint(clampedScreenPos);
            clampedWorldPos.y = arrow.transform.position.y;
            arrow.transform.position = clampedWorldPos;
            arrow.transform.LookAt(npc.transform);
            var rotation = arrow.transform.rotation;
            var euler = rotation.eulerAngles;
            euler.x = 90;
            euler.z = 0;
            rotation.eulerAngles = euler;
            arrow.transform.rotation = rotation;
        }
    }
}
