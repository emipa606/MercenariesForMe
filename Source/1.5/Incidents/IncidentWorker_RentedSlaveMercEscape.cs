using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_RentedSlaveMercEscape : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Utils.GCMFM.playerHaveRentedSlaveMerc();
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!Utils.GCMFM.playerHaveRentedSlaveMerc())
                return false;

            Pawn merc = Utils.GCMFM.getRandomRentedSlaveMerc();

            try
            {
                //Random selection of a Slavic mercenary
                Utils.GCMFM.popRentedMercenary(merc);
                Utils.GCMFM.popRentedPawn(merc);

                IntVec3 dropCellNear;
                int salary = merc.TryGetComp<Comp_USFM>().salary*2;
                List<Thing> toDeliver = new List<Thing>();
                Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);
                thing.stackCount = (int)(salary);

                int nb = thing.stackCount;
                toDeliver.Add(thing);
                Map map = Utils.getRandomMapOfPlayer();


                if (Utils.modernUSFM() || HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
                {
                    dropCellNear = DropCellFinder.TradeDropSpot(map);

                    if (!dropCellNear.IsValid)
                        dropCellNear = CellFinder.RandomEdgeCell(map);

                    DropPodUtility.DropThingsNear(dropCellNear, map, toDeliver.Cast<Thing>(), 100, false, false, false);
                }
                else
                    Utils.spawnMedievalCaravan(map, toDeliver, out dropCellNear);

                Find.LetterStack.ReceiveLetter("MFM_LetterRentedSlaveMercEscape".Translate(), "MFM_LetterRentedSlaveMercEscapeDesc".Translate(merc.Label, Utils.getReadableType(merc.TryGetComp<Comp_USFM>().type), salary, Utils.getUSFMLabel()), LetterDefOf.NegativeEvent);
                merc.Destroy();
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
