using HarmonyLib;
using RimWorld;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers
{
	[HarmonyPatch(typeof(AreaManager))]
	[HarmonyPatch("TryMakeNewAllowed")]
	class AreaAdded
	{
		static void Postfix(Map ___map, bool __result, Area_Allowed area)
		{
			if (__result)
			{
				___map.GetListerFilthInAreas().EnsureAreaHasKey(area);
				___map.GetCleaningManager().MarkAddablesOutdated();
			}
		}
	}
}