using System.Collections.Generic;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class IncidentWorker_RentedMercEarnMoney : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return Utils.GCMFM.playerHaveRentedMerc();
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!Utils.GCMFM.playerHaveRentedMerc())
        {
            return false;
        }

        //Random selection of a mercenary
        var merc = Utils.GCMFM.getRandomRentedMerc();

        //Deduction amount earned
        var mul = Rand.Range(0.5f, 3.5f);

        var toDeliver = new List<Thing>();
        var thing = ThingMaker.MakeThing(ThingDefOf.Silver);
        thing.stackCount = (int)(merc.TryGetComp<Comp_USFM>().salary * mul);

        var nb = thing.stackCount;
        toDeliver.Add(thing);

        var map = Utils.getRandomMapOfPlayer();
        IntVec3 dropCellNear;

        if (Utils.modernUSFM() || HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
        {
            dropCellNear = DropCellFinder.TradeDropSpot(map);

            if (!dropCellNear.IsValid)
            {
                dropCellNear = CellFinder.RandomEdgeCell(map);
            }

            DropPodUtility.DropThingsNear(dropCellNear, map, toDeliver, 100, false, false, false);
        }
        else
        {
            Utils.spawnMedievalCaravan(map, toDeliver, out dropCellNear);
        }

        if (dropCellNear.IsValid)
        {
            Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercEarnMoney".Translate(),
                "MFM_LetterRentedMercEarnMoneyDesc".Translate(merc.Label, nb, Utils.getUSFMLabel()),
                LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, map));
        }
        else
        {
            return false;
        }

        return true;
    }
}