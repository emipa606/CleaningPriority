using HarmonyLib;
using Verse;

namespace CleaningPriority.ListerFilthPrioritizedNotifiers
{
	[HarmonyPatch(typeof(Area))]
	[HarmonyPatch("Invert")]
	class AreaInverted
	{
		static void Prefix(Area __instance)
		{
			ListerFilthInAreas_MapComponent lister = __instance.Map.GetListerFilthInAreas();
			foreach (IntVec3 cell in __instance.Map.AllCells)
			{
				lister.OnAreaChange(cell, !__instance[cell], __instance);
			}
			__instance.Map.GetCleaningManager().MarkNeedToRecalculate();
		}
	}
}
