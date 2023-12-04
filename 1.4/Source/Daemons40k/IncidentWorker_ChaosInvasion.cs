using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using static Daemons40k.Daemons40kUtils;
using static System.Net.Mime.MediaTypeNames;

namespace Daemons40k
{
    public class IncidentWorker_ChaosInvasion : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!def.HasModExtension<DefModExtension_ChaosInvasion>())
            {
                return false;
            }
            Faction faction = Find.FactionManager.FirstFactionOfDef(Daemons40kDefOf.BEWH_ChaosFactionHidden);
            if (faction == null || faction.defeated)
            {
                return false;
            }
            Map map = (Map)parms.target;
            return RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out var root, map, CellFinder.EdgeRoadChance_Hostile);
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(root, map, 10);
            Faction faction = Find.FactionManager.FirstFactionOfDef(Daemons40kDefOf.BEWH_ChaosFactionHidden);

            List<ChaosInvasionParams> invasionParams = def.GetModExtension<DefModExtension_ChaosInvasion>().InvasionParams;

            Random rand = new Random();

            List<Pawn> spawnedPawns = new List<Pawn>();

            foreach (ChaosInvasionParams param in invasionParams)
            {
                PawnKindDef pawnDefSelected;
                if (param.pawnKind.Count > 1)
                {
                    pawnDefSelected = param.pawnKind.RandomElement();
                }
                else
                {
                    pawnDefSelected = param.pawnKind.First();
                }

                int amount = rand.Next(param.amountRange.min, param.amountRange.max);

                for (int i = 0; i < amount; i++)
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(pawnDefSelected, faction);
                    GenSpawn.Spawn(pawn, loc.RandomAdjacentCell8Way(), map, Rot4.Random);
                    pawn.mindState.mentalStateHandler.TryStartMentalState(Daemons40kDefOf.BEWH_ManhunterDaemon);
                    spawnedPawns.Add(pawn);
                }
            }

            LordJob_AssaultColony lordJob = new LordJob_AssaultColony(faction, canKidnap: false, canTimeoutOrFlee: false, sappers: false, useAvoidGridSmart: true);
            Lord lord = LordMaker.MakeNewLord(faction, lordJob, map, spawnedPawns);
            lord.inSignalLeave = parms.inSignalEnd;
            QuestUtility.AddQuestTag(lord, parms.questTag);

            SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, spawnedPawns);

            return true;
        }
    }
}