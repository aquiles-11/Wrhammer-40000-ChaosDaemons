using RimWorld;
using Verse;
using Verse.Sound;


public class CompAbilityEffect_ShadowDance : CompAbilityEffect_GiveHediff
{
    public override void Apply(LocalTargetInfo target, LocalTargetInfo caster)
    {
        var pawn = target.Thing as Pawn; // Получаем цель как персонажа

        // Проверяем, что pawn не равен null
        if (pawn != null)
        {

            // Применяем хеддиф через HediffComp_DisappearOnAttack
            var hediffDef = HediffDef.Named("SDance");
            if (!pawn.health.hediffSet.HasHediff(hediffDef))
            {
                pawn.health.AddHediff(hediffDef);
            }
            else
            {
            }
        }
        else
        {
        }
    }
}
