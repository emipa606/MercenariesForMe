using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_RentedMercFired : IncidentWorker
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
            Utils.GCMFM.popRentedMercenary(merc);
            Utils.GCMFM.popRentedPawn(merc);

            try
            {
                int GT = Find.TickManager.TicksGame;

                merc.SetFactionDirect(Faction.OfPlayer);
                merc.TryGetComp<Comp_USFM>().firedGT = GT + Rand.Range(Settings.minTimeUSFMSuspended, Settings.maxTimeUSFMSuspended);
                List<Pawn> toDeliver = new List<Pawn>();
                toDeliver.Add(merc);

                //Prorated Mercenary XP increment
                Comp_USFM comp = merc.TryGetComp<Comp_USFM>();
                comp.prepareBringBackRentedMerc();

                //Mercenary return via droppod
                Map map = Utils.getRandomMapOfPlayer();
                IntVec3 dropCellNear = Utils.spawnMercOnMap(map, toDeliver);

                Find.LetterStack.ReceiveLetter("MFM_LetterMercFired".Translate(), "MFM_LetterMercFiredDesc".Translate(merc.Label, Utils.getUSFMLabel()), LetterDefOf.NegativeEvent, new LookTargets(dropCellNear, map));
            }
            catch (Exception)
            {
                //If error reintegration of the spawning mercenary
                if (merc != null)
                {
                    Utils.GCMFM.pushRentedMercenary(merc);
                    Utils.GCMFM.pushRentedPawn(merc);
                }
                return false;
            }

            return true;
        }
    }
}
