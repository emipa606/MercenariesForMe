using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI.Group;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class ChoiceLetter_MercWantJoin : ChoiceLetter
    {
        public Pawn merc;

        public override void Removed()
        {
            base.Removed();

            removeSOP();
        }

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                int price = merc.TryGetComp<Comp_USFM>().salary * Settings.mercJoinNbSalaryToPay;

                if (base.ArchivedOnly)
                {
                    yield return base.Option_Close;
                }
                else
                {
                    //Acceptance of payment
                    DiaOption accept = new DiaOption("RansomDemand_Accept".Translate());
                    accept.action = delegate
                    {
                        if (merc == null || merc.Dead)
                        {
                            Find.LetterStack.RemoveLetter(this);
                            removeSOP();
                            return;
                        }

                        //Send money
                        if (Utils.modernUSFM())
                        {
                            foreach (var map in Find.Maps)
                            {
                                if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, price))
                                {
                                    TradeUtility.LaunchSilver(map, price);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Caravan caravan = Utils.caravanOfPlayerOverSiteOfPayment();
                            Utils.caravanPayCost(caravan, price);
                        }
                        Find.LetterStack.RemoveLetter(this);
                        removeSOP();

                        merc.SetFaction(Faction.OfPlayer, null);
                        merc.TryGetComp<Comp_USFM>().isMercenary = false;
                    };
                    accept.resolveTree = true;
                    bool hasEnoughMoney = false;
                    if (Utils.modernUSFM())
                    {
                        foreach (var map in Find.Maps)
                        {
                            if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, price))
                            {
                                hasEnoughMoney = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Caravan caravan = Utils.caravanOfPlayerOverSiteOfPayment();
                        if (caravan != null)
                        {
                            if (Utils.moneyInCaravan(caravan) >= price)
                                hasEnoughMoney = true;
                        }
                    }

                    if (!hasEnoughMoney)
                    {
                        if (Utils.modernUSFM())
                            accept.Disable("NeedSilverLaunchable".Translate(price.ToString()));
                        else
                            accept.Disable("MFM_NeedSilverInCaravanPayBill".Translate(price.ToString().ToString()));
                    }

                    //Rejection of payment
                    DiaOption reject = new DiaOption(text: "RansomDemand_Reject".Translate())
                    {
                        action = () =>
                        {
                            removeSOP();
                            Find.LetterStack.RemoveLetter(this);
                        }
                    };
                    reject.resolveTree = true;

                    yield return accept;
                    yield return reject;
                    yield return base.Option_Postpone;
                }
            }
        }

        private void removeSOP()
        {
            Utils.GCMFM.MercWantJoinInProgress = false;
            Utils.clearAllMedievalSiteOfPayment();
        }

        public override bool CanShowInLetterStack
        {
            get
            {
                return base.CanShowInLetterStack;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.merc, "merc", false);
        }
    }
}