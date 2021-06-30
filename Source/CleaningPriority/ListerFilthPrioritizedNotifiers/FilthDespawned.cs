using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers
{
    [HarmonyPatch(typeof(ListerFilthInHomeArea))]
    [HarmonyPatch("Notify_FilthDespawned")]
    internal class FilthDespawned
    {
        private static void Postfix(Map ___map, Filth f)
        {
            ___map.GetComponent<ListerFilthInAreas_MapComponent>().OnFilthDespawned(f);
            ___map.GetComponent<CleaningManager_MapComponent>().MarkNeedToRecalculate();
        }
    }
}