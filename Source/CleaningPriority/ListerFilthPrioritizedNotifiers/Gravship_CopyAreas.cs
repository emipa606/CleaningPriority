using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(Gravship), "CopyAreas")]
internal static class Gravship_CopyAreas
{
    public static void Prefix(Map oldMap, HashSet<IntVec3> engineFloors)
    {
        if (!ModsConfig.OdysseyActive || oldMap == null || engineFloors == null)
        {
            return;
        }

        var manager = oldMap.GetCleaningManager();
        if (manager == null)
        {
            return;
        }

        var labels = manager.PrioritizedAreas
            .Where(area => area != null && area.ActiveCells.Any(engineFloors.Contains))
            .Select(area => area.Label)
            .Distinct()
            .ToList();

        Current.Game?.World?.GetComponent<GravshipPriorityTransferState>()?.CapturePriorities(labels);
    }
}