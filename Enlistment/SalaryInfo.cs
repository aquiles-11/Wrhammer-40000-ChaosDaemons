using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace  ChickenCore.Enlistment
{
    public class  SalaryInfo: IExposable
    {
        public int lastPaidTick;
        public bool CanPayMoney(FactionEnlistOptionsDef options)
        {
            return Find.TickManager.TicksGame >= lastPaidTick + (options.salaryPeriodDays * GenDate.TicksPerDay);
        }
        public void GiveMoney(FactionEnlistOptionsDef options, Caravan caravan)
        {
            if (options == null || caravan == null)
                return;

            int tickDiff = Find.TickManager.TicksGame - lastPaidTick;
            int salaryPeriodTicks = options.salaryPeriodDays * GenDate.TicksPerDay;
            if (salaryPeriodTicks <= 0)
                return;

            while (tickDiff > salaryPeriodTicks)
            {
                float curBatch = options.salaryRange.RandomInRange;
                ThingDef salaryDef = options.salaryDef ?? ThingDefOf.Silver;
                Thing silver = ThingMaker.MakeThing(salaryDef);
                if (silver == null)
                    return;

                silver.stackCount = (int)curBatch;
                CaravanInventoryUtility.GiveThing(caravan, silver);
                tickDiff -= salaryPeriodTicks;
            }
            lastPaidTick = Find.TickManager.TicksGame;
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref lastPaidTick, "lastPaidTick");
        }
    }


    public class FavorInfo : IExposable
    {
        public int lastPaidTick;
        public bool CanPayFavor(FactionEnlistOptionsDef options)
        {
            return Find.TickManager.TicksGame >= lastPaidTick + (options.favorPeriodDays * GenDate.TicksPerDay);
        }
        public void GiveFavor(FactionEnlistOptionsDef options, Caravan caravan, Faction faction)
        {
            int tickDiff = Find.TickManager.TicksGame - lastPaidTick;
            int favorPeriodTicks = options.favorPeriodDays * GenDate.TicksPerDay;
            List<Pawn> eligiblePawns = caravan.PawnsListForReading
                .Where(pawn => pawn.royalty != null)
                .ToList();

            List<FloatMenuOption> optionsList = new List<FloatMenuOption>();
            foreach (Pawn pawn in eligiblePawns)
            {
                optionsList.Add(new FloatMenuOption(pawn.LabelShort, () =>
                {
                    // Grant favor to the selected pawn
                    while (tickDiff > favorPeriodTicks)
                    {
                        int curBatch = (int)options.favorRange.RandomInRange;
                        pawn.royalty.GainFavor(faction, curBatch);
                        Messages.Message(
                            "ChickenCore.FavorGrantedToPawn".Translate(pawn.LabelShort, curBatch, faction),
                            pawn,
                            MessageTypeDefOf.PositiveEvent);
                        tickDiff -= favorPeriodTicks;
                    }

                    lastPaidTick = Find.TickManager.TicksGame;
                }));
            }
            Find.WindowStack.Add(new FloatMenu(optionsList));
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref lastPaidTick, "lastFavorTick");
        }
    }
}
