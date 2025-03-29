using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using static Daemons40k.Daemons40kUtils;

namespace Daemons40k
{
    public class MentalState_DaemonManhunter : MentalState
    {
        public override void PostStart(string reason)
        {
            base.PostStart(reason);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.AnimalsDontAttackDoors, OpportunityType.Critical);
        }

        public override bool ForceHostileTo(Thing t)
        {
            if (t is Pawn pawn && pawn.MentalState != null && pawn.MentalState.def != null && pawn.MentalState.def.HasModExtension<DefModExtension_DaemonManhunter>())
            {
                return false;
            }
            return true;
        }

        public override bool ForceHostileTo(Faction f)
        {
            if (f.IsPlayer)
            {
                return true;
            }
            return false;
        }

        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Off;
        }
    }
}