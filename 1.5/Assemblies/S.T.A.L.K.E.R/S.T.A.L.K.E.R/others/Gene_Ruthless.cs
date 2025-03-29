using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

public class Gene_Ruthless : Gene
{
    public override void PostAdd()
    {
        base.PostAdd();
        RemoveNegativeThoughts();
    }

    public override void Tick()
    {
        base.Tick();
        if (this.pawn.IsHashIntervalTick(200)) // Проверка раз в некоторое время
        {
            RemoveNegativeThoughts();
        }
    }

    private void RemoveNegativeThoughts()
    {
        // Список мыслей для подавления
        List<ThoughtDef> suppressedThoughts = new List<ThoughtDef>
        {
            DefDatabase<ThoughtDef>.GetNamed("KnowPrisonerSold"),
            DefDatabase<ThoughtDef>.GetNamed("ReleasedHealthyPrisoner"),
            DefDatabase<ThoughtDef>.GetNamed("KnowGuestOrganHarvested"),
            DefDatabase<ThoughtDef>.GetNamed("KnowColonistOrganHarvested"),
            DefDatabase<ThoughtDef>.GetNamed("Nuzzled"),
            DefDatabase<ThoughtDef>.GetNamed("ButcheredHumanlikeCorpse"),
            DefDatabase<ThoughtDef>.GetNamed("KnowButcheredHumanlikeCorpse"),
            DefDatabase<ThoughtDef>.GetNamed("ButcheredHumanlikeCorpseOpinion"),
            DefDatabase<ThoughtDef>.GetNamed("ObservedLayingCorpse"),
            DefDatabase<ThoughtDef>.GetNamed("ObservedLayingRottingCorpse"),
            DefDatabase<ThoughtDef>.GetNamed("WitnessedDeathAlly"),
            DefDatabase<ThoughtDef>.GetNamed("WitnessedDeathNonAlly"),
            DefDatabase<ThoughtDef>.GetNamed("WitnessedDeathFamily"),
            DefDatabase<ThoughtDef>.GetNamed("DeniedJoining"),
            DefDatabase<ThoughtDef>.GetNamed("ColonistBanished"),
            DefDatabase<ThoughtDef>.GetNamed("ColonistBanishedToDie"),
            DefDatabase<ThoughtDef>.GetNamed("PrisonerBanishedToDie"),
            DefDatabase<ThoughtDef>.GetNamed("ColonyPrisonerEscaped"),
            DefDatabase<ThoughtDef>.GetNamed("BondedAnimalBanished"),
            DefDatabase<ThoughtDef>.GetNamed("FailedToRescueRelative"),
            DefDatabase<ThoughtDef>.GetNamed("RescuedRelative"),
            DefDatabase<ThoughtDef>.GetNamed("DeadMansApparel"),
            DefDatabase<ThoughtDef>.GetNamed("HumanLeatherApparelSad"),
            DefDatabase<ThoughtDef>.GetNamed("SoldPrisoner"),
            DefDatabase<ThoughtDef>.GetNamed("BondedAnimalMaster"),
            DefDatabase<ThoughtDef>.GetNamed("NotBondedAnimalMaster"),
            DefDatabase<ThoughtDef>.GetNamed("ColonistLeftUnburied"),
            // Мысли, связанные с чертой Psychopath
            DefDatabase<ThoughtDef>.GetNamed("KnowGuestExecuted"),
            DefDatabase<ThoughtDef>.GetNamed("KnowColonistExecuted"),
            DefDatabase<ThoughtDef>.GetNamed("KnowPrisonerDiedInnocent"),
            DefDatabase<ThoughtDef>.GetNamed("KnowColonistDied"),
            DefDatabase<ThoughtDef>.GetNamed("BondedAnimalDied"),
            DefDatabase<ThoughtDef>.GetNamed("PawnWithGoodOpinionDied"),
            DefDatabase<ThoughtDef>.GetNamed("PawnWithBadOpinionDied"),
            DefDatabase<ThoughtDef>.GetNamed("MySonDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyDaughterDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyHusbandDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyWifeDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyFianceDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyFianceeDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyLoverDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyBrotherDied"),
            DefDatabase<ThoughtDef>.GetNamed("MySisterDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyGrandchildDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyFatherDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyMotherDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyNieceDied"),
            DefDatabase<ThoughtDef>.GetNamed("MyNephewDied"),
            // Мысли о потерях
            DefDatabase<ThoughtDef>.GetNamed("ColonistLost"), // Мысль о потере колониста
            DefDatabase<ThoughtDef>.GetNamed("BondedAnimalReleased"), // Мысль о потере связанного животного
            DefDatabase<ThoughtDef>.GetNamed("BondedAnimalLost"), // Мысль о потере связанного животного
            DefDatabase<ThoughtDef>.GetNamed("PawnWithGoodOpinionLost"), // Мысль о потере друга
            DefDatabase<ThoughtDef>.GetNamed("PawnWithBadOpinionLost"), // Мысль о потере врага
            DefDatabase<ThoughtDef>.GetNamed("MySonLost"), // Мысль о потере сына
            DefDatabase<ThoughtDef>.GetNamed("MyDaughterLost"), // Мысль о потере дочери
            DefDatabase<ThoughtDef>.GetNamed("MyHusbandLost"), // Мысль о потере мужа
            DefDatabase<ThoughtDef>.GetNamed("MyWifeLost"), // Мысль о потере жены
            DefDatabase<ThoughtDef>.GetNamed("MyFianceLost"), // Мысль о потере жениха
            DefDatabase<ThoughtDef>.GetNamed("MyFianceeLost"), // Мысль о потере невесты
            DefDatabase<ThoughtDef>.GetNamed("MyLoverLost"), // Мысль о потере любимого человека
            DefDatabase<ThoughtDef>.GetNamed("MyBrotherLost"), // Мысль о потере брата
            DefDatabase<ThoughtDef>.GetNamed("MySisterLost"), // Мысль о потере сестры
            DefDatabase<ThoughtDef>.GetNamed("MyGrandchildLost"), // Мысль о потере внука
            DefDatabase<ThoughtDef>.GetNamed("MyFatherLost"), // Мысль о потере отца
            DefDatabase<ThoughtDef>.GetNamed("MyMotherLost"), // Мысль о потере матери
            DefDatabase<ThoughtDef>.GetNamed("MyNieceLost"), // Мысль о потере племянницы
            DefDatabase<ThoughtDef>.GetNamed("MyNephewLost"), // Мысль о потере племянника
            DefDatabase<ThoughtDef>.GetNamed("MyHalfSiblingLost"), // Мысль о потере сводного брата/сестры
            DefDatabase<ThoughtDef>.GetNamed("MyAuntLost"), // Мысль о потере тети
            DefDatabase<ThoughtDef>.GetNamed("MyUncleLost"), // Мысль о потере дяди
            DefDatabase<ThoughtDef>.GetNamed("MyGrandparentLost"), // Мысль о потере деда/бабушки
            DefDatabase<ThoughtDef>.GetNamed("MyCousinLost"), // Мысль о потере двоюродного брата/сестры
            DefDatabase<ThoughtDef>.GetNamed("MyKinLost") // Мысль о потере родни
        };
        // Проверка наличия DLC Royalty
        if (ModsConfig.IsActive("Ludeon.RimWorld.Royalty"))
        {
            suppressedThoughts.Add(DefDatabase<ThoughtDef>.GetNamed("OtherTravelerDied"));
        }
        if (ModsConfig.IsActive("Ludeon.RimWorld.Ideology"))
        {
            suppressedThoughts.Add(DefDatabase<ThoughtDef>.GetNamed("DryadDied"));
        }

        // Удаление этих мыслей у пешки
        foreach (var thought in suppressedThoughts)
        {
            var memory = this.pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(thought);
            if (memory != null)
            {
                this.pawn.needs.mood.thoughts.memories.RemoveMemory(memory);
            }
        }
    }
}

