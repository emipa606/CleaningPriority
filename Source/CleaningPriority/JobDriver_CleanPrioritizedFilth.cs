using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace CleaningPriority
{
	class JobDriver_CleanPrioritizedFilth : JobDriver_CleanFilth
    {
        private const TargetIndex FilthInd = TargetIndex.A;
        private float cleaningWorkDone;
		private float totalCleaningWorkDone;
		private float totalCleaningWorkRequired;

		private Filth Filth
		{
			get
			{
				return (Filth)job.GetTarget(FilthInd).Thing;
			}
		}

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            this.pawn.ReserveAsManyAsPossible(this.job.GetTargetQueue(FilthInd), this.job, 1, -1, null);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil initExtractTargetFromQueue = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(FilthInd, null);
			yield return initExtractTargetFromQueue;
			yield return Toils_JobTransforms.SucceedOnNoTargetInQueue(FilthInd);
			yield return Toils_JobTransforms.ExtractNextTargetFromQueue(FilthInd, true);
			yield return Toils_Goto.GotoThing(FilthInd, PathEndMode.Touch).JumpIfDespawnedOrNullOrForbidden(FilthInd, initExtractTargetFromQueue);
			Toil clean = new Toil
			{
				initAction = delegate ()
				{
					cleaningWorkDone = 0f;
					totalCleaningWorkDone = 0f;
					totalCleaningWorkRequired = Filth.def.filth.cleaningWorkToReduceThickness * Filth.thickness;
				}
			};
			clean.tickAction = delegate ()
			{
				Filth filth = Filth;
				cleaningWorkDone += 1f;
				totalCleaningWorkDone += 1f;
				if (cleaningWorkDone > filth.def.filth.cleaningWorkToReduceThickness)
				{
					filth.ThinFilth();
					cleaningWorkDone = 0f;
					if (filth.Destroyed)
					{
						clean.actor.records.Increment(RecordDefOf.MessesCleaned);
						ReadyForNextToil();
						return;
					}
				}
			};
			clean.defaultCompleteMode = ToilCompleteMode.Never;
			clean.WithEffect(EffecterDefOf.Clean, FilthInd);
			clean.WithProgressBar(FilthInd, () => totalCleaningWorkDone / totalCleaningWorkRequired, true, -0.5f);
			clean.PlaySustainerOrSound(() => SoundDefOf.Interact_CleanFilth);
			clean.JumpIfDespawnedOrNullOrForbidden(FilthInd, initExtractTargetFromQueue);
			yield return clean;
			yield return Toils_Jump.Jump(initExtractTargetFromQueue);
			yield break;
		}

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.cleaningWorkDone, "cleaningWorkDone", 0f, false);
            Scribe_Values.Look<float>(ref this.totalCleaningWorkDone, "totalCleaningWorkDone", 0f, false);
            Scribe_Values.Look<float>(ref this.totalCleaningWorkRequired, "totalCleaningWorkRequired", 0f, false);
        }
    }
}