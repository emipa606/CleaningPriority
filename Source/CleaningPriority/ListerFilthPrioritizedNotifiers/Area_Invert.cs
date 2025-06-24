using HarmonyLib;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(Area), nameof(Area.Invert))]
internal class Area_Invert
{
    private static void Prefix(Area __instance)
    {
        var lister = __instance.Map.GetListerFilthInAreas();
        foreach (var cell in __instance.Map.AllCells)
        {
            lister.OnAreaChange(cell, !__instance[cell], __instance);
        }

        __instance.Map.GetCleaningManager().MarkNeedToRecalculate();
    }
}