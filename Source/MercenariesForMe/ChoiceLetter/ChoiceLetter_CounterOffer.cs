using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace aRandomKiwi.MFM;

public class ChoiceLetter_CounterOffer : ChoiceLetter_UnRemovable
{
    public Faction faction;
    public int price;
    public List<Pawn> rogueMercs;

    public override IEnumerable<DiaOption> Choices
    {
        get
        {
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
                        //We must remove from the ToPay list
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
                            if (caravan != null)
                            {
                                Utils.caravanPayCost(caravan, price);
                            }
                        }

                        removeSOP();
                        Find.LetterStack.RemoveLetter(this);
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
                        if (faction.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile)
                        {
                            Find.LetterStack.RemoveLetter(this);
                            return;
                        }

                        var lordJobs = new Dictionary<Map, LordJob_AssaultColony>();
                        var lords = new Dictionary<Map, Lord>();

                        //Penalty of the player, we turn the mercenaries against him
                        foreach (var entry in rogueMercs)
                        {
                            if (entry == null || entry.Dead)
                            {
                                continue;
                            }

                            entry.jobs.StopAll();
                            entry.jobs.ClearQueuedJobs();
                            entry.ClearAllReservations();
                            entry.ClearMind_NewTemp();
                            entry.SetFaction(faction);

                            if (!lords.ContainsKey(entry.Map))
                            {
                                lordJobs[entry.Map] = new LordJob_AssaultColony(faction, true, false);
                                if (lordJobs[entry.Map] != null)
                                {
                                    lords[entry.Map] = LordMaker.MakeNewLord(faction, lordJobs[entry.Map],
                                        Current.Game.CurrentMap);
                                }
                            }

                            lords[entry.Map].AddPawn(entry);
                        }

                        removeSOP();
                        Find.LetterStack.RemoveLetter(this);

                        Find.LetterStack.ReceiveLetter("MFM_LetterMercSalaryIncreaseRejected".Translate(),
                            "MFM_LetterMercSalaryIncreaseRejectedDesc".Translate(rogueMercs.Count, faction.Name),
                            LetterDefOf.ThreatBig);
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
        Utils.GCMFM.CounterOfferInProgress = false;
        Utils.clearAllMedievalSiteOfPayment();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref faction, "faction");
        Scribe_Collections.Look(ref rogueMercs, "rogueMercs", LookMode.Reference);
        Scribe_Values.Look(ref price, "price");
    }
}