using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(ListerFilthInHomeArea), nameof(ListerFilthInHomeArea.Notify_FilthDespawned))]
internal static class ListerFilthInHomeArea_Notify_FilthDespawned
{
    public static void Postfix(Map ___map, Filth f)
    {
        ___map?.GetComponent<ListerFilthInAreas_MapComponent>()?.OnFilthDespawned(f);
        ___map?.GetComponent<CleaningManager_MapComponent>()?.MarkNeedToRecalculate();
    }
}