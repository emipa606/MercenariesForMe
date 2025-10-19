using System.Collections.Generic;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class IncidentWorker_RentedMercDeath : IncidentWorker
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
        Utils.GCMFM.popRentedMercenary(merc);
        Utils.GCMFM.popRentedPawn(merc);

        merc.SetFactionDirect(Faction.OfPlayer);

        if (Utils.modernUSFM())
        {
            var comp = merc.TryGetComp<Comp_USFM>();
            if (comp != null)
            {
                comp.killedDuringSrv = true;
                comp.rentedMercAdvanceBioAge();
            }

            var toDeliver = new List<Thing> { merc };

            var map = Utils.getRandomMapOfPlayer();
            var dropCellNear = DropCellFinder.RandomDropSpot(map);

            Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercDeath".Translate(),
                "MFM_LetterRentedMercDeathDesc".Translate(merc.Label, "MFM_LetterRentedMercDeathDescComp".Translate()),
                LetterDefOf.NegativeEvent, new LookTargets(dropCellNear, map));

            //Mercenary corp return via droppod
            DropPodUtility.DropThingsNear(dropCellNear, map, toDeliver, 100, false, false, false);
        }
        else
        {
            HealthUtility.DamageUntilDead(merc);
            Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercDeath".Translate(),
                "MFM_LetterRentedMercDeathDescComp".Translate(merc.Label, ""), LetterDefOf.NegativeEvent);
        }

        return true;
    }
}