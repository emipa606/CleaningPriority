using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers;

[HarmonyPatch(typeof(ListerFilthInHomeArea))]
[HarmonyPatch("Notify_FilthSpawned")]
internal class FilthSpawned
{
    private static void Postfix(Map ___map, Filth f)
    {
        ___map.GetComponent<ListerFilthInAreas_MapComponent>().OnFilthSpawned(f);
        ___map.GetComponent<CleaningManager_MapComponent>().MarkNeedToRecalculate();
    }
}