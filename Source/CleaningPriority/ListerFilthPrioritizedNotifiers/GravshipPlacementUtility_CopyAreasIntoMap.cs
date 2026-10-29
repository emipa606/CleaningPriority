using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(GravshipPlacementUtility), "CopyAreasIntoMap")]
internal static class GravshipPlacementUtility_CopyAreasIntoMap
{
    public static void Postfix(Map map)
    {
        if (!ModsConfig.OdysseyActive || map == null)
        {
            return;
        }

        var transfer = Current.Game?.World?.GetComponent<GravshipPriorityTransferState>();
        if (transfer is not { HasCapturedPriorities: true })
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