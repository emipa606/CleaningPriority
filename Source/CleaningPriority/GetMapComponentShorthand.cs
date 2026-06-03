using Verse;

namespace CleaningPriority;

internal static class GetMapComponentShorthand
{
    extension(Map map)
    {
        public CleaningManager_MapComponent GetCleaningManager()
        {
            return map.GetComponent<CleaningManager_MapComponent>();
        }

        public ListerFilthInAreas_MapComponent GetListerFilthInAreas()
        {
            return map.GetComponent<ListerFilthInAreas_MapComponent>();
        }
    }
}