using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace CleaningPriority
{
	class WorkGiver_CleanFilthPrioritized : WorkGiver_Scanner
	{
		public static readonly int MinTicksSinceThickened = 600;

		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Filth);
			}
		}

		public override int MaxRegionsToScanBeforeGlobalSearch
        {
			get
			{
				return 4;
			}
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.GetCleaningManager().FilthInCleaningAreas();
		}

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return pawn.Map.GetCleaningManager().FilthInCleaningAreas().EnumerableCount() == 0;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
            if (pawn.Faction != Faction.OfPlayer) return false;
            if (!(t is Filth filth)) return false;
            Area effectiveAreaRestriction = null;
            if(pawn.playerSettings != null && pawn.playerSettings.EffectiveAreaRestriction != null && pawn.playerSettings.EffectiveAreaRestriction.TrueCount > 0 && pawn.playerSettings.EffectiveAreaRestriction.Map == filth.Map)
			{
                effectiveAreaRestriction = pawn.playerSettings.EffectiveAreaRestriction;
            }
			if (pawn.Map.GetCleaningManager().FilthIsInPriorityAreaSafe(filth) || (forced && pawn.Map.GetCleaningManager().FilthIsInCleaningArea(filth)) || (effectiveAreaRestriction != null && effectiveAreaRestriction[filth.Position]))
			{
				LocalTargetInfo target = t;
                var canReserve = pawn.CanReserve(target, 1, -1, null, forced);
                if(!canReserve)
                {
                    filth.Map.GetComponent<CleaningManager_MapComponent>().MarkNeedToRecalculate(filth);
                    return false;
                }

                return filth.TicksSinceThickened >= MinTicksSinceThickened;
			}
			return false;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job job = new Job(DefDatabase<JobDef>.GetNamed("Clean_Prioritized"));
			job.AddQueuedTarget(TargetIndex.A, t);

			Map map = t.Map;
			int maxQueued = 15;
			Room room = t.GetRoom(RegionType.Set_Passable);
			for (int i = 0; i < 100; i++)
			{
				IntVec3 intVec = t.Position + GenRadial.RadialPattern[i];
				if (intVec.InBounds(map) && intVec.GetRoom(map, RegionType.Set_Passable) == room)
				{
					List<Thing> thingList = intVec.GetThingList(map);
					for (int j = 0; j < thingList.Count; j++)
					{
						Thing thing = thingList[j];
						if (HasJobOnThing(pawn, thing, forced) && thing != t)
						{
							job.AddQueuedTarget(TargetIndex.A, thing);
						}
					}
					if (job.GetTargetQueue(TargetIndex.A).Count >= maxQueued)
					{
						break;
					}
				}
			}
			if (job.targetQueueA != null && job.targetQueueA.Count >= 5)
			{
				job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
			}
			return job;
		}
	}
}