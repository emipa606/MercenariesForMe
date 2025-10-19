using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace aRandomKiwi.MFM;

public class JobDriver_UseCommsConsoleUSFM : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var localPawn = pawn;
        var targetA = job.targetA;
        var localJob = job;
        return localPawn.Reserve(targetA, localJob, 1, -1, null, errorOnFailed);
    }

    [DebuggerHidden]
    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedOrNull(TargetIndex.A);
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn(delegate(Toil to)
        {
            var building_CommsConsole = (Building_CommsConsole)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
            return !building_CommsConsole.CanUseCommsNow;
        });
        var openComms = new Toil();
        openComms.initAction = delegate
        {
            var actor = openComms.actor;
            var building_CommsConsole = (Building_CommsConsole)actor.jobs.curJob.GetTarget(TargetIndex.A).Thing;
            if (building_CommsConsole.CanUseCommsNow)
            {
                Find.WindowStack.Add(new Dispatcher(actor, actor.Map, null));
            }
        };
        yield return openComms;
    }
}