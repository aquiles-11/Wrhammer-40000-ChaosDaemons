using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

public class HediffComp_Poison : HediffComp
{
    private const int TicksBetweenDamage = 150; // количество тиков между каждым нанесением урона
    private int ticksSinceLastDamage;
    private bool recentlyExtended = false;

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);

        ticksSinceLastDamage++;

        // Применяем урон через определённое количество тиков
        if (ticksSinceLastDamage >= TicksBetweenDamage && parent.Severity > 0.001f)
        {
            ApplyDamage();
            ticksSinceLastDamage = 0; // сбрасываем таймер
        }

        // Уменьшаем тяжесть только если яд не был продлён
        if (!recentlyExtended)
        {
            severityAdjustment = -1f / 2400f;
        }

        // Проверка на удаление яда
        if (parent.Severity <= 0.001f)
        {
            parent.Severity = 0f;
            Pawn pawn = parent.pawn;
            if (pawn != null && !pawn.Dead)
            {
                pawn.health.RemoveHediff(parent);
            }
        }

        recentlyExtended = false; // Сбрасываем флаг, так как ход завершён
    }

    public void ResetPoisonTimerAndIncreaseSeverity(float addedSeverity)
    {
        ticksSinceLastDamage = 0; // сбрасываем таймер яда
        parent.Severity += addedSeverity; // увеличиваем тяжесть
        recentlyExtended = true; // Устанавливаем флаг, что яд был продлён
    }

    private void ApplyDamage()
    {
        Pawn pawn = parent.pawn; // Получаем жертву

        if (pawn != null && !pawn.Dead)
        {
            int poisonDamage = CalculatePoisonDamage(parent.Severity);
            DamageInfo dinfo = new DamageInfo(DefDatabase<DamageDef>.GetNamed("SF_PoisonDamage"), poisonDamage, 0f, -1f, pawn);
            pawn.TakeDamage(dinfo);
        }
    }

    private int CalculatePoisonDamage(float severity)
    {
        if (severity < 0.3f)
        {
            return 2; // Легкая степень
        }
        else if (severity < 0.7f)
        {
            return 4; // Средняя степень
        }
        else
        {
            return 6; // Тяжелая степень
        }
    }

    public override bool CompShouldRemove
    {
        get
        {
            // Удаляем яд, если была оказана первая помощь
            if (parent.Severity <= 0.001f || CanBeTreatedByFirstAid())
            {
                return true;
            }
            return base.CompShouldRemove;
        }
    }

    private bool CanBeTreatedByFirstAid()
    {
        // Ограничиваем возможность лечения яда только индустриальной и ультратехнологической медициной
        return false; // По умолчанию - нельзя
    }

    public void AttemptToCurePoison(Pawn healer, Medicine _medicine)
    {
        // Проверка, может ли данная медицина лечить яд
        if (_medicine != null && _medicine.def.IsMedicine)
        {
            // Проверяем, является ли медицина индустриальной или ультратехнологической
            if (_medicine.def.techLevel == TechLevel.Industrial || _medicine.def.techLevel == TechLevel.Spacer)
            {
                // Убираем эффект яда при использовании медицинского предмета
                parent.Severity = 0;
                if (parent.pawn != null && !parent.pawn.Dead)
                {
                    parent.pawn.health.RemoveHediff(parent); // Удаляем яд
                }
            }
            else
            {
            }
        }
    }
}
