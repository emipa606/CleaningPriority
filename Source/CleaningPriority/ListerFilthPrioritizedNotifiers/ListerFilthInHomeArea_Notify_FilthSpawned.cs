using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(ListerFilthInHomeArea), nameof(ListerFilthInHomeArea.Notify_FilthSpawned))]
internal class ListerFilthInHomeArea_Notify_FilthSpawned
{
    private static void Postfix(Map ___map, Filth f)
    {
        ___map?.GetComponent<ListerFilthInAreas_MapComponent>()?.OnFilthSpawned(f);
        ___map?.GetComponent<CleaningManager_MapComponent>()?.MarkNeedToRecalculate();
    }
}