using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace CleaningPriority;

internal class JobDriver_CleanPrioritizedFilth : JobDriver_CleanFilth
{
    private const TargetIndex FilthInd = TargetIndex.A;
    private float cleaningWorkDone;
    private float totalCleaningWorkDone;
    private float totalCleaningWorkRequired;

    private Filth Filth => (Filth)job.GetTarget(FilthInd).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        pawn.ReserveAsManyAsPossible(job.GetTargetQueue(FilthInd), job);
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var initExtractTargetFromQueue = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(FilthInd);
        yield return initExtractTargetFromQueue;
        yield return Toils_JobTransforms.SucceedOnNoTargetInQueue(FilthInd);
        yield return Toils_JobTransforms.ExtractNextTargetFromQueue(FilthInd);
        yield return Toils_Goto.GotoThing(FilthInd, PathEndMode.Touch)
            .JumpIfDespawnedOrNullOrForbidden(FilthInd, initExtractTargetFromQueue);
        var clean = new Toil
        {
            initAction = delegate
            {
                cleaningWorkDone = 0f;
                totalCleaningWorkDone = 0f;
                totalCleaningWorkRequired = Filth.def.filth.cleaningWorkToReduceThickness * Filth.thickness;
            }
        };
        clean.tickAction = delegate
        {
            var filth = Filth;
            cleaningWorkDone += 1f;
            totalCleaningWorkDone += 1f;
            if (!(cleaningWorkDone > filth.def.filth.cleaningWorkToReduceThickness))
            {
                return;
            }

            filth.ThinFilth();
            cleaningWorkDone = 0f;
            if (!filth.Destroyed)
            {
                return;
            }

            clean.actor.records.Increment(RecordDefOf.MessesCleaned);
            ReadyForNextToil();
        };
        clean.defaultCompleteMode = ToilCompleteMode.Never;
        clean.WithEffect(EffecterDefOf.Clean, FilthInd);
        clean.WithProgressBar(FilthInd, () => totalCleaningWorkDone / totalCleaningWorkRequired, true);
        clean.PlaySustainerOrSound(() => SoundDefOf.Interact_CleanFilth);
        clean.JumpIfDespawnedOrNullOrForbidden(FilthInd, initExtractTargetFromQueue);
        yield return clean;
        yield return Toils_Jump.Jump(initExtractTargetFromQueue);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref cleaningWorkDone, "cleaningWorkDone");
        Scribe_Values.Look(ref totalCleaningWorkDone, "totalCleaningWorkDone");
        Scribe_Values.Look(ref totalCleaningWorkRequired, "totalCleaningWorkRequired");
    }
}