using Verse;
using Verse.AI;
using RimWorld;

public class CompObelisk_Xenochanger : ThingComp
{
    private const int AutoActivationChanceThreshold = 30;
    private const int AutoActivationChance = 10;
    private const int ObeliskDestructionDelay = 600;

    private int studyProgress;
    private bool isActivated;
    private int destructionCountdown = -1;
    private bool raidSuccessful;

    public CompProperties_ObeliskXenochanger Props => (CompProperties_ObeliskXenochanger)props;

    public override void CompTick()
    {
        base.CompTick();

        if (!isActivated && studyProgress >= AutoActivationChanceThreshold)
        {
            if (Rand.RangeInclusive(1, 100) <= AutoActivationChance)
            {
                ActivateObelisk();
            }
        }

        if (destructionCountdown > 0)
        {
            destructionCountdown--;
            if (destructionCountdown == 0)
            {
                DestroyObelisk();
            }
        }
    }

    public void UpdateStudyProgress(int progress)
    {
        studyProgress = progress;
        if (studyProgress >= AutoActivationChanceThreshold)
        {
            CreateRaidButton();
        }
    }

    private void CreateRaidButton()
    {
        if (!isActivated)
        {
            // Add code to create a button on the obelisk to manually trigger the Hemosucker raid
            // Implementation depends on RimWorld's UI system (not provided here for brevity)
        }
    }

    public void ActivateObelisk()
    {
        if (!isActivated)
        {
            isActivated = true;
            TriggerHemosuckerRaid();
        }
    }

    private void TriggerHemosuckerRaid()
    {
        IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, parent.Map);
        parms.faction = Find.FactionManager.FirstFactionOfDef(FactionDef.Named("WildHemosuckers"));
        IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);

        raidSuccessful = false;
        Find.TickManager.slower.SignalForceNormalSpeed();
    }

    public void OnRaidCompleted(bool success)
    {
        raidSuccessful = success;
        if (raidSuccessful)
        {
            // Вызвать диалоговое окно для выбора пешки после успешного отражения рейда
            Find.WindowStack.Add(new Dialog_SelectPawnToApproachObelisk(this));
        }
    }

    public void SendPawnToObelisk(Pawn pawn)
    {
        if (pawn != null && pawn.Spawned)
        {
            Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("ApproachObelisk"), parent);
            pawn.jobs.StartJob(job, JobCondition.InterruptForced);
        }
    }

    public void ConvertPawnToHemosucker(Pawn pawn)
    {
        if (pawn != null && pawn.story != null)
        {
            pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("Hemosucker"));
            destructionCountdown = ObeliskDestructionDelay;
        }
    }

    [DefOf]
    public static class SoundDefOf
    {
        static SoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SoundDefOf));
        }

        public static SoundDef CristallExplosion;
    }

    private void DestroyObelisk()
    {
        // Создать эффект взрыва на месте обелиска
        GenExplosion.DoExplosion(
            parent.Position,            // Позиция взрыва
            parent.Map,                 // Карта
            3.0f,                       // Радиус взрыва
            DamageDefOf.Bomb,           // Тип урона
            null,                       // Нет прямого инициатора взрыва
            -1,                         // Сила урона
            -1,                         // Armor Penetration
            SoundDefOf.CristallExplosion,  // Звук взрыва
            null,                       // Не показывать эффекты снарядов
            null,                       // Не показывать эффекты дымовых облаков
            null,                       // Не использовать пост-эффекты
            ThingDefOf.Filth_RubbleRock // Оставлять обломки после взрыва
        );


        // Выпадение осколков и биоферита
        int shardsCount = Rand.RangeInclusive(2, 5); // Количество осколков
        int bioferiteCount = Rand.RangeInclusive(1, 3); // Количество биоферита

        // Создаём осколки
        for (int i = 0; i < shardsCount; i++)
        {
            Thing shard = ThingMaker.MakeThing(ThingDef.Named("Shard"));
            GenPlace.TryPlaceThing(shard, parent.Position, parent.Map, ThingPlaceMode.Near);
        }

        // Создаём биоферит
        for (int i = 0; i < bioferiteCount; i++)
        {
            Thing bioferite = ThingMaker.MakeThing(ThingDef.Named("Bioferite"));
            GenPlace.TryPlaceThing(bioferite, parent.Position, parent.Map, ThingPlaceMode.Near);
        }

        // Уничтожение обелиска
        parent.Destroy(DestroyMode.Vanish);
    }


    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref studyProgress, "studyProgress", 0);
        Scribe_Values.Look(ref isActivated, "isActivated", false);
        Scribe_Values.Look(ref destructionCountdown, "destructionCountdown", -1);
        Scribe_Values.Look(ref raidSuccessful, "raidSuccessful", false);
    }
}

public class CompProperties_ObeliskXenochanger : CompProperties
{
    public CompProperties_ObeliskXenochanger()
    {
        compClass = typeof(CompObelisk_Xenochanger);
    }
}



