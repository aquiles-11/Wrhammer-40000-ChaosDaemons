using System.Collections.Generic;
using Verse;
using Verse.AI;

public class JobDriver_ApproachObelisk : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(TargetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        // Задача подойти к обелиску
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

        // Задача изменяет ксенотип пешки после подхода к обелиску
        Toil changeXenotypeToil = new Toil
        {
            initAction = () =>
        {
            var obeliskComp = TargetA.Thing.TryGetComp<CompObelisk_Xenochanger>();
            obeliskComp?.ConvertPawnToHemosucker(pawn);
        }
        };
        yield return changeXenotypeToil;
    }
}
