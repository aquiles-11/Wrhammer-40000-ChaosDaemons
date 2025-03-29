using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

public class CompDiscoverable : ThingComp
{
    // Метод для проверки обнаружения невидимых колонистов
    public void CheckForInvisibilityDetection()
    {
        // Получаем всех колонистов в радиусе 10 клеток
        List<Pawn> nearbyPawns = this.parent.Map.mapPawns.AllPawnsSpawned
            .Where(p => p.Position.InHorDistOf(this.parent.Position, 10f) && p != this.parent)
            .ToList();

        foreach (var targetPawn in nearbyPawns)
        {
            // Проверяем наличие устройства, способного обнаружить невидимость
            if (HasDetectionDevice(targetPawn))
            {
                // Если устройство обнаружения есть, можно "обнаружить" невидимого колониста
                if (targetPawn.health.hediffSet.HasHediff(HediffDef.Named("SDance")))
                {
                    Log.Message($"Колонист {targetPawn.Name} обнаружен!");
                    // Делаем колониста видимым
                    RemoveInvisibility(targetPawn);
                }
            }
        }
    }

    // Метод для удаления невидимости
    private void RemoveInvisibility(Pawn pawn)
    {
        // Удаляем хеддиф невидимости
        if (pawn.health.hediffSet.HasHediff(HediffDef.Named("SDance")))
        {
            pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs.FirstOrDefault(h => h.def == HediffDef.Named("SDance")));
            Log.Message($"Колонист {pawn.Name} теперь видим!");
        }
    }

    // Метод для проверки наличия устройства обнаружения
    private bool HasDetectionDevice(Pawn pawn)
    {
        // Проверяем инвентарь колониста на наличие устройств
        foreach (var item in pawn.inventory.innerContainer)
        {
            // Например, можно проверить наличие определенного типа предмета
            if (item.def == ThingDef.Named("YourDetectionDevice")) // замените на ваш предмет
            {
                return true;
            }
        }
        return false;
    }

    public override void CompTick()
    {
        base.CompTick();

        // Вызываем метод обнаружения каждую такт
        CheckForInvisibilityDetection();
    }
}
