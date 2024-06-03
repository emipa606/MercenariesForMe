using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_RentedMercEarnMoney : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Utils.GCMFM.playerHaveRentedMerc();
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!Utils.GCMFM.playerHaveRentedMerc())
                return false;

            //Random selection of a mercenary
            Pawn merc = Utils.GCMFM.getRandomRentedMerc();

            //Deduction amount earned
            float mul = Rand.Range(0.5f, 3.5f);

            List<Thing> toDeliver = new List<Thing>();
            Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);
            thing.stackCount = (int)(merc.TryGetComp<Comp_USFM>().salary * mul);

            int nb = thing.stackCount;
            toDeliver.Add(thing);

            Map map = Utils.getRandomMapOfPlayer();
            IntVec3 dropCellNear;

            if (Utils.modernUSFM())
            {
                dropCellNear = DropCellFinder.TradeDropSpot(map);

                if (!dropCellNear.IsValid)
                    dropCellNear = CellFinder.RandomEdgeCell(map);

                DropPodUtility.DropThingsNear(dropCellNear, map, toDeliver.Cast<Thing>(), 100, false, false, false);
            }
            else
                Utils.spawnMedievalCaravan(map, toDeliver, out dropCellNear);
            
            if (dropCellNear.IsValid)
            {
                Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercEarnMoney".Translate(), "MFM_LetterRentedMercEarnMoneyDesc".Translate(merc.Label, nb, Utils.getUSFMLabel()), LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, map));
            }
            else
                return false;

            return true;
        }
    }
}
