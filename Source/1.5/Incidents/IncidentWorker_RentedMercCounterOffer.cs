using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_RentedMercCounterOffer : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return !Utils.GCMFM.CounterOfferInProgress && Utils.GCMFM.playerHaveMerc() && Utils.getPlayerMercenaries().Count >= 1 && Settings.enableIncidentCounterOffer;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (Utils.GCMFM.CounterOfferInProgress || !Utils.GCMFM.playerHaveMerc() || Utils.getPlayerMercenaries().Count < 1 || !Settings.enableIncidentCounterOffer)
                return false;

            int price = 0;
            Faction faction;

            //Enemy faction deduction at the origin of the counter offer
            faction = Find.FactionManager.RandomEnemyFaction();

            //Rogue Mercenaries Deduction
            List<Pawn> rogueMercs = Utils.GCMFM.getRandomRogueMerc();

            //Deduction amount
            foreach (var merc in rogueMercs)
            {
                Comp_USFM comp = merc.TryGetComp<Comp_USFM>();
                price += (int)(comp.salary * Rand.Range(Settings.minRateIncSalaryIncidentCounterOffer, Settings.maxRateIncSalaryIncidentCounterOffer));
            }

            //If in middle age mode creation site of payment
            if (!Utils.modernUSFM() && !Utils.anySiteOfPayment())
            {
                foreach(var map in Find.Maps)
                {
                    if(map.IsPlayerHome && !HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
                        Utils.createSiteOfPayment(map);
                }
            }

            ChoiceLetter_CounterOffer cl = (ChoiceLetter_CounterOffer)LetterMaker.MakeLetter(DefDatabase<LetterDef>.GetNamed("MFM_CLCounterOffer"));
            cl.Label = "MFM_ChoiceLetterCounterOfferTitle".Translate();
            cl.Text = "MFM_ChoiceLetterCounterOffer".Translate(faction.Name, price, 3);
            cl.radioMode = true;
            cl.rogueMercs = rogueMercs;
            cl.faction = faction;
            cl.price = 0;
            if(!Utils.modernUSFM())
                cl.StartTimeout(30000);
            else
                cl.StartTimeout(7500);

            Utils.GCMFM.CounterOfferInProgress = true;

            Find.LetterStack.ReceiveLetter(cl, null);
            return true;
        }
    }
}
