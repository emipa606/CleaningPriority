using HarmonyLib;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(AreaManager), "NotifyEveryoneAreaRemoved")]
internal static class AreaManager_NotifyEveryoneAreaRemoved
{
    public static void Postfix(Map ___map, Area area)
    {
        ___map?.GetComponent<ListerFilthInAreas_MapComponent>()?.OnAreaDeleted(area);
        ___map?.GetComponent<CleaningManager_MapComponent>()?.OnAreaDeleted(area);
    }
}