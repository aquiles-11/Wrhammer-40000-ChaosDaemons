using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using static Verse.DamageWorker;


public class Verb_MeleeAttackPoison : Verb_MeleeAttack
{
    protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
    {
        if (!target.IsValid || !(target.Thing is Pawn targetPawn))
        {
            return new DamageWorker.DamageResult();
        }

        // Получаем базовый урон от режущего удара из настроек
        float cutDamage = this.verbProps.AdjustedMeleeDamageAmount(this, targetPawn);
        Log.Message($"Attempting to deal cut damage: {cutDamage} to {targetPawn.Name}.");

        // Создаем информацию о режущем ударе
        DamageInfo cutDinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("SF_PoisonedCut"), cutDamage, 0f, -1f, this.Caster, null, this.EquipmentSource?.def); // Исправлено

        // Применяем режущий урон
        DamageWorker.DamageResult cutDamageResult = DefDatabase<DamageDef>.GetNamed("SF_PoisonedCut").Worker.Apply(cutDinfo, targetPawn);

        // Если цель является механойдом, яд не наносим
        if (targetPawn.RaceProps.IsMechanoid)
        {
            Log.Message($"{targetPawn.Name} is a mechanoid. No poison will be applied.");
            return cutDamageResult; // Возвращаем результат пореза, яд не накладывается
        }

        // Если это не механойд, проверяем наличие уже существующего эффекта яда
        Hediff existingPoisonHediff = targetPawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("SF_PoisonHediff"));
        if (existingPoisonHediff == null)
        {
            // Если эффекта яда нет, создаем новый
            Hediff poisonHediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("SF_PoisonHediff"), targetPawn);
            poisonHediff.Severity = cutDamage * 0.02f; // Устанавливаем начальную тяжесть яда в зависимости от урона порезом
            targetPawn.health.AddHediff(poisonHediff);
            Log.Message($"Applied new poison to {targetPawn.Name} with severity {poisonHediff.Severity}.");
        }
        else
        {
            // Если яд уже есть, обновляем его тяжесть
            existingPoisonHediff.Severity += cutDamage * 0.02f; // Увеличиваем тяжесть яда
            if (existingPoisonHediff.Severity > existingPoisonHediff.def.maxSeverity)
            {
                existingPoisonHediff.Severity = existingPoisonHediff.def.maxSeverity; // Ограничиваем максимальную тяжесть
            }
            Log.Message($"Updated poison severity for {targetPawn.Name} to {existingPoisonHediff.Severity}.");
        }

        // Логируем урон с указанием оружия
        string weaponName = this.EquipmentSource?.LabelCap ?? "Unknown weapon";
        Log.Message($"{targetPawn.Name} received a cut from {weaponName} ({cutDamage} damage).");

        return cutDamageResult;
    }
}