using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NPCManager
{
    public IReadOnlyCollection<NPC> BlackNpcs => blackNpcs;
    public IReadOnlyCollection<NPC> WhiteNpcs => whiteNpcs;

    private readonly HashSet<NPC> blackNpcs = new HashSet<NPC>();
    private readonly HashSet<NPC> whiteNpcs = new HashSet<NPC>();
    
    private static readonly Lazy<NPCManager> instance = new Lazy<NPCManager>(() => new NPCManager());
    public static NPCManager Instance => instance.Value;

    public IReadOnlyCollection<NPC> GetNPCsByColor(GameColor color)
    {
        switch (color) {
            case GameColor.Black:
                return BlackNpcs;
            case GameColor.White:
                return WhiteNpcs;
            default:
                throw new InvalidOperationException("Unexpected GameColor");
        }
    }

    public void RegisterNPC(NPC npc)
    {
        switch (npc.Color) {
                case GameColor.Black:
                    blackNpcs.Add(npc);
                    break;
                case GameColor.White:
                    whiteNpcs.Add(npc);
                    break;
        }
    }
    
    public void UnregisterNPC(NPC npc) {
        switch (npc.Color) {
                case GameColor.Black:
                    blackNpcs.Remove(npc);
                    break;
                case GameColor.White:
                    whiteNpcs.Remove(npc);
                    break;
        }
    }
}
