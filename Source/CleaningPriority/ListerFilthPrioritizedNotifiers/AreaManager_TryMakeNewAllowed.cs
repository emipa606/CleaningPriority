using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(AreaManager), nameof(AreaManager.TryMakeNewAllowed))]
internal static class AreaManager_TryMakeNewAllowed
{
    public static void Postfix(Map ___map, bool __result, Area_Allowed area)
    {
        if (!__result)
        {
            return;
        }

        ___map?.GetListerFilthInAreas()?.EnsureAreaHasKey(area);
        ___map?.GetCleaningManager()?.MarkAddablesOutdated();
    }
}