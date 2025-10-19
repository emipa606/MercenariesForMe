using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class IncidentWorker_RentedMercFired : IncidentWorker
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

        try
        {
            var GT = Find.TickManager.TicksGame;

            merc.SetFactionDirect(Faction.OfPlayer);
            merc.TryGetComp<Comp_USFM>().firedGT =
                GT + Rand.Range(Settings.minTimeUSFMSuspended, Settings.maxTimeUSFMSuspended);
            var toDeliver = new List<Pawn> { merc };

            //Prorated Mercenary XP increment
            var comp = merc.TryGetComp<Comp_USFM>();
            Comp_USFM.prepareBringBackRentedMerc();
            comp.rentedMercAdvanceBioAge();

            //Mercenary return via droppod
            var map = Utils.getRandomMapOfPlayer();

            //Patch avec SOS2: Si c'est une map spatiale SOS2 / Rimnauts 2, on prendra une autre --Par Ionfrigate12345:
            if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
            {
                map = HarmonyUtils.GetPlayerMainColonyMap(true, false);
            }

            if (map == null) //Si on trouve toujours pas (le joueur n'a pas de map planétaire)
            {
                Log.Warning(
                    "Cannot find a map to spawn mercenaries. The player seems to have only SOS2 space maps where the spawning is prevented.");
                return false;
            }

            var dropCellNear = Utils.spawnMercOnMap(map, toDeliver);

            Find.LetterStack.ReceiveLetter("MFM_LetterMercFired".Translate(),
                "MFM_LetterMercFiredDesc".Translate(merc.Label, Utils.getUSFMLabel()), LetterDefOf.NegativeEvent,
                new LookTargets(dropCellNear, map));
        }
        catch (Exception)
        {
            //If error reintegration of the spawning mercenary
            if (merc == null)
            {
                return false;
            }

            Utils.GCMFM.pushRentedMercenary(merc);
            Utils.GCMFM.pushRentedPawn(merc);

            return false;
        }

        return true;
    }
}