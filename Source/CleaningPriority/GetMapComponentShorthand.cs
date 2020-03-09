using Verse;

namespace CleaningPriority
{
	static class GetMapComponentShorthand
	{
		public static CleaningManager_MapComponent GetCleaningManager(this Map map)
		{
			return map.GetComponent<CleaningManager_MapComponent>();
		}

		public static ListerFilthInAreas_MapComponent GetListerFilthInAreas(this Map map)
		{
			return map.GetComponent<ListerFilthInAreas_MapComponent>();
		}
	}
}