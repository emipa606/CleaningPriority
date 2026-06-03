using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(Gravship), "CopyAreas")]
internal class Gravship_CopyAreas
{
    private static void Prefix(Map oldMap, HashSet<IntVec3> engineFloors)
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

[HarmonyPatch(typeof(GravshipPlacementUtility), "CopyAreasIntoMap")]
internal class GravshipPlacementUtility_CopyAreasIntoMap
{
    private static void Postfix(Map map)
    {
        if (!ModsConfig.OdysseyActive || map == null)
        {
            return;
        }

        var transfer = Current.Game?.World?.GetComponent<GravshipPriorityTransferState>();
        if (transfer == null || !transfer.HasCapturedPriorities)
        {
            return;
        }

        var manager = map.GetCleaningManager();
        if (manager == null)
        {
            return;
        }

        var labels = transfer.ConsumePriorities();
        var orderedAreas = new List<Area>();
        foreach (var label in labels)
        {
            var area = map.areaManager.AllAreas.FirstOrDefault(candidate => candidate?.Label == label);
            if (area != null)
            {
                orderedAreas.Add(area);
            }
        }

        if (orderedAreas.Any())
        {
            manager.SetPriorityAreas(orderedAreas);
        }
    }
}
