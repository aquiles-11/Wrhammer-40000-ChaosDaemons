using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

public class HediffComp_MentalStateOnDamage : HediffComp
{
    public float ChanceToTrigger = 0.4f; // По умолчанию вероятность 40%

    public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);

        // Проверка на случайное срабатывание
        if (Rand.Value < ChanceToTrigger && base.Pawn != null && !base.Pawn.Dead)
        {
            // Проверка, чтобы избежать активации психоза у лежащей пешки или в состоянии Deathrest
            if (!base.Pawn.Downed && !base.Pawn.health.hediffSet.HasHediff(HediffDefOf.Deathrest))
            {
                // Проверка, чтобы избежать повторного активации
                if (base.Pawn.mindState.mentalStateHandler.CurStateDef != DefDatabase<MentalStateDef>.GetNamed("MentalStateTargetHostileOnly"))
                {
                    // Запуск психоза
                    base.Pawn.mindState.mentalStateHandler.TryStartMentalState(DefDatabase<MentalStateDef>.GetNamed("MentalStateTargetHostileOnly"), "Took damage", forced: true);

                    // Воспроизведение звука при начале психоза
                    SoundDef psychoticSound = SoundDef.Named("Enraged");
                    psychoticSound.PlayOneShot(new TargetInfo(base.Pawn.Position, base.Pawn.Map));
                }
            }
        }
    }
}
