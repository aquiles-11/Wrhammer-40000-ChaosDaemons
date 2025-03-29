using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

public class CompAbilityEffect_BloodSucking : CompAbilityEffect
{
    public new CompProperties_AbilityBloodSucking Props => (CompProperties_AbilityBloodSucking)props;

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);

        Pawn pawn = target.Pawn;
        Corpse corpse = target.Thing as Corpse; // Проверяем, является ли цель трупом

        if (corpse != null) // Если цель - труп
        {
            var corpseFeedEffect = new CompAbilityEffect_CorpsefeederBite
            {
                parent = this.parent, // Передаем родителя
                props = new CompProperties_AbilityCorpsefeederBite() // Инициализация соответствующего CompProperties
            };

            if (corpseFeedEffect != null)
            {
                corpseFeedEffect.Apply(target, dest); // Применяем эффект к трупу
            }
            else
            {
            }
            return;
        }

        if (pawn != null) // Если цель - пешка
        {

            // Проверка на гемофага
            if (pawn.genes != null && pawn.genes.HasActiveGene(GeneDefOf.Hemogenic))
            {
                var sanguoFeedEffect = new CompAbilityEffect_SanguofeederBite
                {
                    parent = this.parent,
                    props = new CompProperties_AbilityBloodSuckingBite() // Инициализация props
                };

                if (sanguoFeedEffect != null)
                {
                    sanguoFeedEffect.Apply(target, dest); // Применяем эффект для гемофага
                    return;
                }
                else
                {
                    return;
                }
            }

            // Применяем укус для обычного человека
            if (pawn.RaceProps.Humanlike)
            {
                var bloodFeedEffect = new CompAbilityEffect_BloodfeederBite
                {
                    parent = this.parent, // Передаем родителя
                    props = new CompProperties_AbilityBloodfeederBite() // Инициализация props
                };

                if (bloodFeedEffect != null)
                {
                    bloodFeedEffect.Apply(target, dest); // Применяем эффект для человека
                    return;
                }
                else
                {
                    return;
                }
            }

            // Применяем укус для животного
            if (pawn.RaceProps.Animal)
            {
                var animalFeedEffect = new CompAbilityEffect_AnimalfeederBite
                {
                    parent = this.parent,
                    props = new CompProperties_AbilityAnimalfeederBite() // Инициализация props
                };

                if (animalFeedEffect != null)
                {
                    animalFeedEffect.Apply(target, dest); // Применяем эффект для животного
                    return;
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
        }
    }

    public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
    {
        return target.Pawn != null || target.Thing is Corpse;
    }

    public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
    {
        if (target.Pawn == null && !(target.Thing is Corpse))
        {
            if (throwMessages)
            {
                Messages.Message("Target must be a valid pawn or corpse.", MessageTypeDefOf.RejectInput);
            }
            return false;
        }
        return true;
    }
}