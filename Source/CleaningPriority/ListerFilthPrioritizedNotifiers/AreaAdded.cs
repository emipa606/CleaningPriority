using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(AreaManager))]
[HarmonyPatch("TryMakeNewAllowed")]
internal class AreaAdded
{
    private static void Postfix(Map ___map, bool __result, Area_Allowed area)
    {
        if (!__result)
        {
            return;
        }

        ___map.GetListerFilthInAreas().EnsureAreaHasKey(area);
        ___map.GetCleaningManager().MarkAddablesOutdated();
    }
}