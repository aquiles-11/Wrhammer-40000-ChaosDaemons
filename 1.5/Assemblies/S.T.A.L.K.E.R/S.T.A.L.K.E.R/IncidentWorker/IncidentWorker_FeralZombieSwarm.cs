using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace S.T.A.L.K.E.R
{
    public class IncidentWorker_FeralZombieSwarmSmall : IncidentWorker_ShamblerSwarm
    {
        private static readonly IntRange NumShamblersToSpawn = new IntRange(3, 8);

        protected override IntRange ShamblerLifespanTicksRange => new IntRange(25000, 45000);

        protected override List<Pawn> GenerateEntities(IncidentParms parms, float points)
        {
            int randomInRange = NumShamblersToSpawn.RandomInRange;
            List<Pawn> list = new List<Pawn>();
            for (int i = 0; i < randomInRange; i++)
            {
                Pawn item = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.STLKR_FeralZombie, Faction.OfEntities));
                list.Add(item);
            }
            SetupShamblerHediffs(list, ShamblerLifespanTicksRange);
            return list;
        }

        protected override void SendLetter(IncidentParms parms, List<Pawn> entities)
        {
            string letterLabel = def.letterLabel;
            TaggedString baseLetterText = def.letterText.Formatted(entities.Count);
            SendStandardLetter(letterLabel, baseLetterText, def.letterDef, parms, entities);
        }
    }

}