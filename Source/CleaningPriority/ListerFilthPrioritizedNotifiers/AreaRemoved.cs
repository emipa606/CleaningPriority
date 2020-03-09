using HarmonyLib;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers
{
	[HarmonyPatch(typeof(AreaManager))]
	[HarmonyPatch("NotifyEveryoneAreaRemoved")]
	class AreaRemoved
	{
		static void Postfix(Map ___map, Area area)
		{
			___map.GetComponent<ListerFilthInAreas_MapComponent>().OnAreaDeleted(area);
			___map.GetComponent<CleaningManager_MapComponent>().OnAreaDeleted(area);
		}
	}
}