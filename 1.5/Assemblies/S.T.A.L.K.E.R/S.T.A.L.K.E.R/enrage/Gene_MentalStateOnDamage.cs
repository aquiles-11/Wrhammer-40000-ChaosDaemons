using RimWorld;
using Verse;

public class Gene_MentalStateOnDamage : Gene
{
    public float ChanceToTrigger = 0.4f; // Вероятность активации психоза

    // Этот метод вызывается при активации гена
    public override void PostAdd()
    {
        base.PostAdd();
        // Добавляем Hediff к пешке, если его нет
        HediffWithComps hediff = pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("MentalStateOnDamage")) as HediffWithComps;
        if (hediff == null)
        {
            Hediff hediffToAdd = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("MentalStateOnDamage"), pawn);
            pawn.health.AddHediff(hediffToAdd);

            // Передаем вероятность в HediffComp
            var hediffComp = hediffToAdd.TryGetComp<HediffComp_MentalStateOnDamage>();
            if (hediffComp != null)
            {
                hediffComp.ChanceToTrigger = this.ChanceToTrigger; // Передача вероятности
            }
        }
    }

    // Удаление Hediff при удалении гена
    public override void PostRemove()
    {
        base.PostRemove();
        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("MentalStateOnDamage"));
        if (hediff != null)
        {
            pawn.health.RemoveHediff(hediff);
        }
    }
}