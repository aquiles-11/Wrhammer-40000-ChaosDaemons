using RimWorld;
using System.Linq;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse.Sound;

public class MentalState_TargetHostileOnly : MentalState
{
    private const int MaxAttackRange = 20;
    private const float MeleeCooldownMultiplier = 0.5f; // Множитель для удвоенной скорости атаки
    private const int MentalStateDuration = 1200; // Длительность психоза в тиках (600 тиков = 10 секунд)

    private int ticksUntilEnd;

    public override void PostStart(string reason)
    {
        base.PostStart(reason);

        // Принудительно драфтуем колониста, если он не драфтован
        if (!pawn.Drafted)
        {
            pawn.drafter.Drafted = true; // Принудительно драфтуем колониста
        }

        // Воспроизведение звука при начале психоза
        SoundDef psychoticSound = SoundDef.Named("Enraged");
        psychoticSound.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));

        // Инициализация длительности психоза
        ticksUntilEnd = MentalStateDuration;

        // Применяем модификатор множителя перезарядки ближнего боя
        ApplyMeleeCooldownModifier();
    }

    public override void PostEnd()
    {
        base.PostEnd();

        // Удаляем модификатор множителя перезарядки ближнего боя
        RemoveMeleeCooldownModifier();

        // Устанавливаем колониста в режим драфта
        pawn.drafter.Drafted = true;
    }

    private void ApplyMeleeCooldownModifier()
    {
        var hediff = HediffMaker.MakeHediff(HediffDef.Named("MeleeWeaponCooldownModifier"), pawn);
        hediff.Severity = MeleeCooldownMultiplier;
        pawn.health.AddHediff(hediff);
    }

    private void RemoveMeleeCooldownModifier()
    {
        var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("MeleeWeaponCooldownModifier"));
        if (hediff != null)
        {
            pawn.health.RemoveHediff(hediff);
        }
    }


    public override void MentalStateTick()
    {
        base.MentalStateTick();

        if (ticksUntilEnd > 0)
        {
            ticksUntilEnd--;
        }

        if (ticksUntilEnd <= 0)
        {
            Log.Message($"{pawn.Name} выходит из состояния психоза.");
            RecoverFromState();
            return;
        }

        if (!pawn.Downed && !pawn.stances.FullBodyBusy)
        {
            // Игнорируем поваленных врагов при поиске цели
            Thing hostileTarget = FindClosestHostileTarget();

            // Если текущая цель недоступна, ищем другую
            if (hostileTarget != null && CanAttackTarget(hostileTarget))
            {
                Job newJob = JobMaker.MakeJob(JobDefOf.AttackMelee, hostileTarget);
                pawn.jobs.StartJob(newJob, JobCondition.InterruptForced);
            }
            else
            {
                // Проверяем, есть ли вокруг другие враги
                List<Thing> otherTargets = FindOtherHostileTargetsAround(pawn.Position, MaxAttackRange);
                if (otherTargets.Count == 0) // Если врагов нет
                {
                    // Находим случайное место для блуждания
                    IntVec3 wanderTarget = RCellFinder.RandomWanderDestFor(pawn, pawn.Position, 7f, null, Danger.None, false);
                    Job wanderJob = JobMaker.MakeJob(JobDefOf.GotoWander, wanderTarget);
                    pawn.jobs.StartJob(wanderJob, JobCondition.InterruptForced);
                }
                else
                {
                    // Пытаемся найти новую цель
                    hostileTarget = FindAnotherHostileTarget(otherTargets);
                    if (hostileTarget != null && CanAttackTarget(hostileTarget))
                    {
                        Job newJob = JobMaker.MakeJob(JobDefOf.AttackMelee, hostileTarget);
                        pawn.jobs.StartJob(newJob, JobCondition.InterruptForced);
                    }
                }
            }
        }
    }



    public override RandomSocialMode SocialModeMax()
    {
        return RandomSocialMode.Off;
    }

    private Thing FindClosestHostileTarget()
    {
        Thing result = null;
        float closestDistSquared = MaxAttackRange * MaxAttackRange;

        foreach (Thing potentialTarget in pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn).Cast<Thing>())
        {
            if (potentialTarget.HostileTo(pawn.Faction) && IsValidTarget(potentialTarget) && !IsDowned(potentialTarget))
            {
                float distSquared = (potentialTarget.Position - pawn.Position).LengthHorizontalSquared;
                if (distSquared < closestDistSquared)
                {
                    closestDistSquared = distSquared;
                    result = potentialTarget;
                }
            }
        }
        return result;
    }

    private Thing FindAnotherHostileTarget(List<Thing> otherTargets)
    {
        foreach (Thing potentialTarget in otherTargets)
        {
            if (potentialTarget.HostileTo(pawn.Faction) && IsValidTarget(potentialTarget) && !IsDowned(potentialTarget))
            {
                return potentialTarget;
            }
        }
        return null;
    }

    private bool IsDowned(Thing target)
    {
        Pawn enemyPawn = target as Pawn;
        return enemyPawn != null && enemyPawn.Downed;
    }

    private bool IsValidTarget(Thing target)
    {
        return !target.Destroyed && target.Spawned && !target.IsForbidden(pawn);
    }

    private bool CanAttackTarget(Thing target)
    {
        if (target == null || target.Destroyed || !target.Spawned)
        {
            return false;
        }

        if ((pawn.Position - target.Position).LengthHorizontal > MaxAttackRange)
        {
            return false;
        }
        return true;
    }

    private List<Thing> FindOtherHostileTargetsAround(IntVec3 position, float radius)
    {
        List<Thing> hostileTargets = new List<Thing>();
        foreach (Thing potentialTarget in position.GetThingList(pawn.Map))
        {
            if (potentialTarget.HostileTo(pawn.Faction) && IsValidTarget(potentialTarget) && !IsDowned(potentialTarget)) // Проверка на Downed
            {
                float distance = (potentialTarget.Position - position).LengthHorizontal;
                if (distance <= radius)
                {
                    hostileTargets.Add(potentialTarget);
                }
            }
        }
        return hostileTargets;
    }

    public override TaggedString GetBeginLetterText()
    {
        return null; // Отключает текст уведомления
    }
}
