using System.Collections.Generic;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class ChoiceLetter_MercWantJoin : ChoiceLetter
{
    public Pawn merc;

    public override IEnumerable<DiaOption> Choices
    {
        get
        {
            var price = merc.TryGetComp<Comp_USFM>().salary * Settings.mercJoinNbSalaryToPay;

            if (ArchivedOnly)
            {
                yield return Option_Close;
            }
            else
            {
                //Acceptance of payment
                var accept = new DiaOption("RansomDemand_Accept".Translate())
                {
                    action = delegate
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
                                if (!map.IsPlayerHome || !TradeUtility.ColonyHasEnoughSilver(map, price))
                                {
                                    continue;
                                }

                                TradeUtility.LaunchSilver(map, price);
                                break;
                            }
                        }
                        else
                        {
                            var caravan = Utils.caravanOfPlayerOverSiteOfPayment();
                            Utils.caravanPayCost(caravan, price);
                        }

                        Find.LetterStack.RemoveLetter(this);
                        removeSOP();

                        merc.SetFaction(Faction.OfPlayer);
                        merc.TryGetComp<Comp_USFM>().isMercenary = false;
                    },
                    resolveTree = true
                };
                var hasEnoughMoney = false;
                if (Utils.modernUSFM())
                {
                    foreach (var map in Find.Maps)
                    {
                        if (!map.IsPlayerHome || !TradeUtility.ColonyHasEnoughSilver(map, price))
                        {
                            continue;
                        }

                        hasEnoughMoney = true;
                        break;
                    }
                }
                else
                {
                    var caravan = Utils.caravanOfPlayerOverSiteOfPayment();
                    if (caravan != null)
                    {
                        if (Utils.moneyInCaravan(caravan) >= price)
                        {
                            hasEnoughMoney = true;
                        }
                    }
                }

                if (!hasEnoughMoney)
                {
                    accept.Disable(Utils.modernUSFM()
                        ? "NeedSilverLaunchable".Translate(price.ToString())
                        : "MFM_NeedSilverInCaravanPayBill".Translate(price.ToString()));
                }

                //Rejection of payment
                var reject = new DiaOption("RansomDemand_Reject".Translate())
                {
                    action = () =>
                    {
                        removeSOP();
                        Find.LetterStack.RemoveLetter(this);
                    },
                    resolveTree = true
                };

                yield return accept;
                yield return reject;
                yield return Option_Postpone;
            }
        }
    }

    public override void Removed()
    {
        base.Removed();

        removeSOP();
    }

    private static void removeSOP()
    {
        Utils.GCMFM.MercWantJoinInProgress = false;
        Utils.clearAllMedievalSiteOfPayment();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref merc, "merc");
    }
}