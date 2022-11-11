using HarmonyLib;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(Area))]
[HarmonyPatch("Set")]
internal class AreaChange
{
    private static void Postfix(Area __instance, AreaManager ___areaManager, IntVec3 c, bool val)
    {
        ___areaManager?.map?.GetListerFilthInAreas()?.OnAreaChange(c, val, __instance);
        ___areaManager?.map?.GetCleaningManager()?.MarkNeedToRecalculate();
    }
}