using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

public class CompProperties_AbilityBloodSucking : CompProperties_AbilityEffect
{
    public float hemogenGain;

    public ThoughtDef thoughtDefToGiveTarget;

    public ThoughtDef opinionThoughtDefToGiveTarget;

    public float resistanceGain;

    public float nutritionGain = 0.1f;

    public float targetBloodLoss = 0.4499f;

    public IntRange bloodFilthToSpawnRange;

    public CompProperties_AbilityBloodSucking()
    {
        compClass = typeof(CompAbilityEffect_BloodSucking);
    }

    public override IEnumerable<string> ExtraStatSummary()
    {
        yield return "AbilityHemogenGain".Translate() + ": " + (hemogenGain * 100f).ToString("F0");
    }
}

public class CompAbilityEffect_CorpsefeederBite : CompAbilityEffect
{
    private List<HediffDef> hediffList = new List<HediffDef>
    {
        UsingHediffDef.STLKR_ConsumedAnimalHemogen,
        UsingHediffDef.STLKR_ConsumedSanguophageHemogen
    };

    public new CompProperties_AbilityCorpsefeederBite Props => (CompProperties_AbilityCorpsefeederBite)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
        if (!(target.Thing is Corpse corpse))
        {
            return;
        }
        DoBite(parent.pawn, corpse, Props.hemogenGain, Props.nutritionGain, Props.targetBloodLoss, Props.resistanceGain, Props.bloodFilthToSpawnRange);
        foreach (HediffDef hediff in hediffList)
        {
            HediffSet hediffSet = parent.pawn.health.hediffSet;
            if (hediffSet != null && hediffSet.HasHediff(hediff))
            {
                Hediff firstHediffOfDef = parent.pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                if (firstHediffOfDef != null)
                {
                    parent.pawn.health.RemoveHediff(firstHediffOfDef);
                }
            }
        }
        parent.pawn.health.AddHediff(UsingHediffDef.STLKR_ConsumedCorpseHemogen);
        corpse.Destroy();
    }

    public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
    {
        return Valid(target);
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        if (!(target.Thing is Corpse corpse))
        {
            return false;
        }
        if (!AbilityUtility.ValidateMustBeHuman(corpse.InnerPawn, throwMessages, parent))
        {
            return false;
        }
        return true;
    }

    public static void DoBite(Pawn biter, Corpse corpse, float targetHemogenGain, float nutritionGain, float targetBloodLoss, float victimResistanceGain, IntRange bloodFilthToSpawnRange)
    {
        if (!ModLister.CheckBiotech("Sanguophage bite"))
        {
            return;
        }
        float offset = targetHemogenGain * corpse.InnerPawn.BodySize;
        GeneUtility.OffsetHemogen(biter, offset);
        if (biter.needs?.food != null)
        {
            biter.needs.food.CurLevel += nutritionGain;
        }
        int randomInRange = bloodFilthToSpawnRange.RandomInRange;
        for (int i = 0; i < randomInRange; i++)
        {
            IntVec3 c = corpse.Position;
            if (randomInRange > 1 && Rand.Chance(0.8888f))
            {
                c = corpse.Position.RandomAdjacentCell8Way();
            }
            if (c.InBounds(corpse.MapHeld))
            {
                FilthMaker.TryMakeFilth(c, corpse.MapHeld, ThingDefOf.Filth_CorpseBile);
            }
        }
    }
}

public class CompProperties_AbilityCorpsefeederBite : CompProperties_AbilityEffect
{
    public float hemogenGain;

    public float resistanceGain;

    public float nutritionGain = 0.1f;

    public float targetBloodLoss = 0.4499f;

    public IntRange bloodFilthToSpawnRange;

    public CompProperties_AbilityCorpsefeederBite()
    {
        compClass = typeof(CompAbilityEffect_CorpsefeederBite);
    }

    public override IEnumerable<string> ExtraStatSummary()
    {
        yield return "AbilityHemogenGain".Translate() + ": " + (hemogenGain * 100f).ToString("F0");
    }
}

public class CompAbilityEffect_AnimalfeederBite : CompAbilityEffect
{
    private List<HediffDef> hediffList = new List<HediffDef>
    {
        UsingHediffDef.STLKR_ConsumedCorpseHemogen,
        UsingHediffDef.STLKR_ConsumedSanguophageHemogen
    };

    public new CompProperties_AbilityAnimalfeederBite Props => (CompProperties_AbilityAnimalfeederBite)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
        Pawn pawn = target.Pawn;
        if (pawn == null)
        {
            return;
        }
        SanguophageUtility.DoBite(parent.pawn, pawn, Props.hemogenGain, Props.nutritionGain, BiteAmount(pawn), Props.resistanceGain, Props.bloodFilthToSpawnRange);
        foreach (HediffDef hediff in hediffList)
        {
            HediffSet hediffSet = parent.pawn.health.hediffSet;
            if (hediffSet != null && hediffSet.HasHediff(hediff))
            {
                Hediff firstHediffOfDef = parent.pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                if (firstHediffOfDef != null)
                {
                    parent.pawn.health.RemoveHediff(firstHediffOfDef);
                }
            }
        }
        parent.pawn.health.AddHediff(UsingHediffDef.STLKR_ConsumedAnimalHemogen);
        if (!pawn.Dead)
        {
            float manhunterOnDamageChance = PawnUtility.GetManhunterOnDamageChance(pawn);
            if (!pawn.mindState.mentalStateHandler.InMentalState && Rand.Chance(PawnUtility.GetManhunterOnDamageChance(pawn, parent.pawn)))
            {
                StartManhunterBecauseOfPawnAction(pawn, parent.pawn, "AnimalManhunterFromDamage", causedByDamage: true);
            }
        }
    }

    public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
    {
        return Valid(target);
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        Pawn pawn = target.Pawn;
        if (pawn == null)
        {
            return false;
        }
        if (!AbilityUtility.ValidateMustBeAnimal(pawn, throwMessages, parent))
        {
            return false;
        }
        return true;
    }

    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        Pawn pawn = target.Pawn;
        if (pawn != null)
        {
            string text = null;
            float num = BloodlossAfterBite(pawn);
            if (num >= HediffDefOf.BloodLoss.lethalSeverity)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }
                text += "WillKill".Translate();
            }
            else if (HediffDefOf.BloodLoss.stages[HediffDefOf.BloodLoss.StageAtSeverity(num)].lifeThreatening)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }
                text += "WillCauseSeriousBloodloss".Translate();
            }
            return text;
        }
        return base.ExtraLabelMouseAttachment(target);
    }

    public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
    {
        Pawn pawn = target.Pawn;
        if (pawn != null)
        {
            float num = BloodlossAfterBite(pawn);
            if (num >= HediffDefOf.BloodLoss.lethalSeverity)
            {
                return Dialog_MessageBox.CreateConfirmation("WarningPawnWillDieFromBloodfeeding".Translate(pawn.Named("PAWN")), confirmAction, destructive: true);
            }
            if (HediffDefOf.BloodLoss.stages[HediffDefOf.BloodLoss.StageAtSeverity(num)].lifeThreatening)
            {
                return Dialog_MessageBox.CreateConfirmation("WarningPawnWillHaveSeriousBloodlossFromBloodfeeding".Translate(pawn.Named("PAWN")), confirmAction, destructive: true);
            }
        }
        return null;
    }

    private float BiteAmount(Pawn target)
    {
        float num = target.RaceProps.baseBodySize;
        if ((double)num < 0.2)
        {
            num = 0.2f;
        }
        if (num > 2f)
        {
            num = 2f;
        }
        return -0.5f * (num - 0.2f) + 1f;
    }

    private float BloodlossAfterBite(Pawn target)
    {
        if (target.Dead || !target.RaceProps.IsFlesh)
        {
            return 0f;
        }
        float num = BiteAmount(target);
        Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
        if (firstHediffOfDef != null)
        {
            num += firstHediffOfDef.Severity;
        }
        return num;
    }

    public void StartManhunterBecauseOfPawnAction(Pawn pawn, Pawn instigator, string letterTextKey, bool causedByDamage = false)
    {
        if (!pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter))
        {
            return;
        }
        string text = letterTextKey.Translate(pawn.Label, pawn.Named("PAWN")).AdjustedFor(pawn);
        GlobalTargetInfo globalTargetInfo = pawn;
        float num = 0.5f;
        num *= PawnUtility.GetManhunterChanceFactorForInstigator(instigator);
        int num2 = 1;
        if (Find.Storyteller.difficulty.allowBigThreats && Rand.Value < num)
        {
            foreach (Pawn packmate in GetPackmates(pawn, 24f))
            {
                if (packmate.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, forced: false, forceWake: false, causedByMood: false, null, transitionSilently: false, causedByDamage))
                {
                    num2++;
                }
            }
            if (num2 > 1)
            {
                globalTargetInfo = new TargetInfo(pawn.Position, pawn.Map);
                text += "\n\n";
                text += "AnimalManhunterOthers".Translate(pawn.kindDef.GetLabelPlural(), pawn);
            }
        }
        string text2 = (pawn.RaceProps.Animal ? pawn.Label : pawn.def.label);
        string text3 = "LetterLabelAnimalManhunterRevenge".Translate(text2).CapitalizeFirst();
        Find.LetterStack.ReceiveLetter(text3, text, (num2 == 1) ? LetterDefOf.ThreatSmall : LetterDefOf.ThreatBig, globalTargetInfo);
    }

    public IEnumerable<Pawn> GetPackmates(Pawn pawn, float radius)
    {
        District pawnRoom = pawn.GetDistrict();
        IReadOnlyList<Pawn> raceMates = pawn.Map.mapPawns.AllPawnsSpawned;
        for (int i = 0; i < raceMates.Count; i++)
        {
            if (pawn != raceMates[i] && raceMates[i].def == pawn.def && raceMates[i].Faction == pawn.Faction && raceMates[i].Position.InHorDistOf(pawn.Position, radius) && raceMates[i].GetDistrict() == pawnRoom)
            {
                yield return raceMates[i];
            }
        }
    }

    public class CompProperties_AbilityAnimalfeederBite : CompProperties_AbilityEffect
    {
        public float hemogenGain;

        public float resistanceGain;

        public float nutritionGain = 0.1f;

        public float targetBloodLoss = 0.4499f;

        public IntRange bloodFilthToSpawnRange;

        public CompProperties_AbilityAnimalfeederBite()
        {
            compClass = typeof(CompAbilityEffect_AnimalfeederBite);
        }

        public override IEnumerable<string> ExtraStatSummary()
        {
            yield return "AbilityHemogenGain".Translate() + ": " + (hemogenGain * 100f).ToString("F0");
        }
    }
}

public class CompProperties_AbilityAnimalfeederBite : CompProperties_AbilityEffect
{
    public float hemogenGain;

    public float resistanceGain;

    public float nutritionGain = 0.1f;

    public float targetBloodLoss = 0.4499f;

    public IntRange bloodFilthToSpawnRange;

    public CompProperties_AbilityAnimalfeederBite()
    {
        compClass = typeof(CompAbilityEffect_AnimalfeederBite);
    }

    public override IEnumerable<string> ExtraStatSummary()
    {
        yield return "AbilityHemogenGain".Translate() + ": " + (hemogenGain * 100f).ToString("F0");
    }
}

public class CompAbilityEffect_SanguofeederBite : CompAbilityEffect
{
    private List<HediffDef> hediffList = new List<HediffDef>
    {
        UsingHediffDef.STLKR_ConsumedCorpseHemogen,
        UsingHediffDef.STLKR_ConsumedAnimalHemogen
    };

    public new CompProperties_AbilityBloodSuckingBite Props => (CompProperties_AbilityBloodSuckingBite)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
        Pawn pawn = target.Pawn;
        if (pawn == null)
        {
            return;
        }
        SanguophageUtility.DoBite(parent.pawn, pawn, Props.hemogenGain, Props.nutritionGain, Props.targetBloodLoss, Props.resistanceGain, Props.bloodFilthToSpawnRange, Props.thoughtDefToGiveTarget, Props.opinionThoughtDefToGiveTarget);
        if (!pawn.Dead)
        {
            GeneUtility.OffsetHemogen(pawn, -0.2f);
        }
        foreach (HediffDef hediff in hediffList)
        {
            HediffSet hediffSet = parent.pawn.health.hediffSet;
            if (hediffSet != null && hediffSet.HasHediff(hediff))
            {
                Hediff firstHediffOfDef = parent.pawn.health.hediffSet.GetFirstHediffOfDef(hediff);
                if (firstHediffOfDef != null)
                {
                    parent.pawn.health.RemoveHediff(firstHediffOfDef);
                }
            }
        }
        parent.pawn.health.AddHediff(UsingHediffDef.STLKR_ConsumedSanguophageHemogen);
        Gene_HemogenDrain gene_HemogenDrain = parent.pawn.genes?.GetFirstGeneOfType<Gene_HemogenDrain>();
    }

    public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
    {
        return Valid(target);
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        Pawn pawn = target.Pawn;
        if (pawn == null)
        {
            return false;
        }
        if (!AbilityUtility.ValidateMustBeHumanOrWildMan(pawn, throwMessages, parent))
        {
            return false;
        }
        Pawn_GeneTracker genes = pawn.genes;
        if (genes == null || !genes.HasActiveGene(GeneDefOf.Hemogenic))
        {
            if (throwMessages)
            {
                Messages.Message("STLKR_MessageCantUseOnNonSanguophage".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
            }
            return false;
        }
        Gene_HemogenDrain gene_HemogenDrain = pawn.genes?.GetFirstGeneOfType<Gene_HemogenDrain>();
        if (gene_HemogenDrain != null && gene_HemogenDrain.Resource.Value < 0.1f)
        {
            if (throwMessages)
            {
                Messages.Message("STLKR_MessageCantUseOnDrainedSanguophage".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
            }
            return false;
        }
        if (pawn.Faction != null && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony)
        {
            if (pawn.Faction.HostileTo(parent.pawn.Faction))
            {
                if (!pawn.Downed)
                {
                    if (throwMessages)
                    {
                        Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    return false;
                }
            }
            else if (pawn.IsQuestLodger() || pawn.Faction != parent.pawn.Faction)
            {
                if (throwMessages)
                {
                    Messages.Message("MessageCannotUseOnOtherFactions".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
                }
                return false;
            }
        }
        if (pawn.IsWildMan() && !pawn.IsPrisonerOfColony && !pawn.Downed)
        {
            if (throwMessages)
            {
                Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
            }
            return false;
        }
        return true;
    }

    public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
    {
        Pawn pawn = target.Pawn;
        if (pawn != null)
        {
            string text = null;
            float num = BloodlossAfterBite(pawn);
            if (num >= HediffDefOf.BloodLoss.lethalSeverity)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }
                text += "WillKill".Translate();
            }
            else if (HediffDefOf.BloodLoss.stages[HediffDefOf.BloodLoss.StageAtSeverity(num)].lifeThreatening)
            {
                if (!text.NullOrEmpty())
                {
                    text += "\n";
                }
                text += "WillCauseSeriousBloodloss".Translate();
            }
            return text;
        }
        return base.ExtraLabelMouseAttachment(target);
    }

    public override Window ConfirmationDialog(LocalTargetInfo target, Action confirmAction)
    {
        Pawn pawn = target.Pawn;
        if (pawn != null)
        {
            float severity = BloodlossAfterBite(pawn);
            if (HediffDefOf.BloodLoss.stages[HediffDefOf.BloodLoss.StageAtSeverity(severity)].lifeThreatening)
            {
                return Dialog_MessageBox.CreateConfirmation("WarningPawnWillHaveSeriousBloodlossFromBloodfeeding".Translate(pawn.Named("PAWN")), confirmAction, destructive: true);
            }
        }
        return null;
    }

    private float BloodlossAfterBite(Pawn target)
    {
        if (target.Dead || !target.RaceProps.IsFlesh)
        {
            return 0f;
        }
        float num = Props.targetBloodLoss;
        Hediff firstHediffOfDef = target.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);
        if (firstHediffOfDef != null)
        {
            num += firstHediffOfDef.Severity;
        }
        return num;
    }
}
