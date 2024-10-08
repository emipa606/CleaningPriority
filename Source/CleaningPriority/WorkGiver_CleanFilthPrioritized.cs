﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace CleaningPriority;

internal class WorkGiver_CleanFilthPrioritized : WorkGiver_Scanner
{
    public static readonly int MinTicksSinceThickened = 600;

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Filth);

    public override int MaxRegionsToScanBeforeGlobalSearch => 4;

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.GetCleaningManager().FilthInCleaningAreas();
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !pawn.Map.GetCleaningManager().FilthInCleaningAreas().Any();
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (pawn.Faction != Faction.OfPlayer && !pawn.IsPrisonerOfColony)
        {
            return false;
        }

        if (t is not Filth filth)
        {
            return false;
        }

        if (!pawn.Map.GetCleaningManager().FilthInCleaningAreas().Contains(filth))
        {
            return false;
        }

        Area effectiveAreaRestriction = null;
        if (pawn.playerSettings?.EffectiveAreaRestrictionInPawnCurrentMap is { TrueCount: > 0 } &&
            pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap.Map == filth.Map)
        {
            effectiveAreaRestriction = pawn.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap;
        }

        if (!pawn.Map.GetCleaningManager().FilthIsInPriorityAreaSafe(filth) &&
            (!forced || !pawn.Map.GetCleaningManager().FilthIsInCleaningArea(filth)) &&
            (effectiveAreaRestriction == null || !effectiveAreaRestriction[filth.Position]))
        {
            return false;
        }

        LocalTargetInfo target = t;
        var canReserve = pawn.CanReserve(target, 1, -1, null, forced);
        if (canReserve)
        {
            return filth.TicksSinceThickened >= MinTicksSinceThickened;
        }

        filth.Map.GetComponent<CleaningManager_MapComponent>().MarkNeedToRecalculate(filth);
        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!HasJobOnThing(pawn, t, forced))
        {
            return null;
        }

        var job = new Job(DefDatabase<JobDef>.GetNamed("Clean_Prioritized"));
        job.AddQueuedTarget(TargetIndex.A, t);

        var map = t.Map;
        var maxQueued = 15;
        var room = t.GetRoom();

        for (var i = 0; i < 100; i++)
        {
            var intVec = t.Position + GenRadial.RadialPattern[i];
            if (!intVec.InBounds(map) || intVec.GetRoom(map) != room)
            {
                continue;
            }

            var thingList = intVec.GetThingList(map);
            foreach (var thing in thingList)
            {
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

        if (job.targetQueueA is { Count: >= 5 })
        {
            job.targetQueueA.SortBy(targ => targ.Cell.DistanceToSquared(pawn.Position));
        }

        return job;
    }
}