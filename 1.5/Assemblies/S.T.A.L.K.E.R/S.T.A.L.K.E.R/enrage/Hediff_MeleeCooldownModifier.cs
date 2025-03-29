using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

public class Hediff_MeleeCooldownModifier : Hediff
{
    private const float DefaultCooldownMultiplier = 1f; // Стандартное значение

    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);

        if (pawn != null)
        {
            // Устанавливаем значение скорости атаки при добавлении хеддиффа
            this.Severity = DefaultCooldownMultiplier * 0.5f; // Используем переменную
        }
    }

    public override void Notify_PawnKilled()
    {
        base.Notify_PawnKilled();

        if (pawn != null)
        {
            // Удаляем хеддифф, когда пешка погибает
            pawn.health.RemoveHediff(this);
        }
    }

    public override string TipStringExtra => "Ускоряет перезарядку ближнего боя.";
}