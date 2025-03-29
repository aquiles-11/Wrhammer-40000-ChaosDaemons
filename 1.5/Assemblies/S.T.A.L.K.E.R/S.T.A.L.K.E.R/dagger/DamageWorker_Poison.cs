using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

public class DamageWorker_PoisonedCut : DamageWorker_AddInjury
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing)
    {
        // Применяем обычный урон от пореза
        DamageResult damageResult = base.Apply(dinfo, thing);

        // Можно добавить доп. логику для других эффектов (например, яд)
        return damageResult;
    }
}