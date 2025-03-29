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
    public class IncidentWorker_FeralZombieAssault : IncidentWorker_RaidEnemy
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            parms.faction = Faction.OfEntities;
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.STLKR_FeralZombies, parms);
            float num = Faction.OfEntities.def.MinPointsToGeneratePawnGroup(PawnGroupKindDefOf.STLKR_FeralZombies);
            if (parms.points < num)
            {
                parms.points = (defaultPawnGroupMakerParms.points = num * 2f);
            }
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                return false;
            }
            if (AnomalyIncidentUtility.IncidentShardChance(parms.points))
            {
                AnomalyIncidentUtility.PawnShardOnDeath(list.RandomElement());
            }
            parms.raidArrivalMode.Worker.Arrive(list, parms);
            LordMaker.MakeNewLord(Faction.OfEntities, new LordJob_GorehulkAssault(), parms.target as Map, list);
            SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, list);
            return true;
        }
    }
}