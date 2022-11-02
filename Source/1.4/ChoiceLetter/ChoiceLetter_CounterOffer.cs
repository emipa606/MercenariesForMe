using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using Verse.AI.Group;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class ChoiceLetter_CounterOffer : ChoiceLetter_UnRemovable
    {
        public Faction faction;
        public int price;
        public List<Pawn> rogueMercs;

        public override void Removed()
        {
            base.Removed();

            removeSOP();
        }

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
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
                        //We must remove from the ToPay list
                        //Send money
                        if (Utils.modernUSFM())
                        {
                            foreach (var map in Find.Maps)
                            {
                                if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, this.price))
                                {
                                    TradeUtility.LaunchSilver(map, this.price);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Caravan caravan = Utils.caravanOfPlayerOverSiteOfPayment();
                            if(caravan != null)
                                Utils.caravanPayCost(caravan, this.price);
                        }
                        removeSOP();
                        Find.LetterStack.RemoveLetter(this);
                    };
                    accept.resolveTree = true;
                    bool hasEnoughMoney = false;
                    if (Utils.modernUSFM())
                    {
                        foreach (var map in Find.Maps)
                        {
                            if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, this.price))
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
                            if (Utils.moneyInCaravan(caravan) >= this.price)
                                hasEnoughMoney = true;
                        }
                    }

                    if (!hasEnoughMoney)
                    {
                        if (Utils.modernUSFM())
                            accept.Disable("NeedSilverLaunchable".Translate(this.price.ToString()));
                        else
                            accept.Disable("MFM_NeedSilverInCaravanPayBill".Translate(this.price.ToString().ToString()));
                    }

                    //Rejection of payment
                    DiaOption reject = new DiaOption(text: "RansomDemand_Reject".Translate())
                    {
                        action = () =>
                        {
                            if (faction.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile)
                            {
                                Find.LetterStack.RemoveLetter(this);
                                return;
                            }

                            Dictionary<Map,LordJob_AssaultColony> lordJobs = new Dictionary<Map,LordJob_AssaultColony>();
                            Dictionary<Map, Lord> lords = new Dictionary<Map, Lord>();

                            //Penalty of the player, we turn the mercenaries against him
                            foreach (var entry in rogueMercs)
                            {
                                if (entry == null || entry.Dead)
                                    continue;

                                entry.jobs.StopAll();
                                entry.jobs.ClearQueuedJobs();
                                entry.ClearAllReservations();
                                entry.ClearMind();
                                entry.SetFaction(faction,null);

                                if (!lords.ContainsKey(entry.Map))
                                {
                                    lordJobs[entry.Map] = new LordJob_AssaultColony(faction, true, false, false, false, true);
                                    if (lordJobs[entry.Map] != null)
                                        lords[entry.Map] = LordMaker.MakeNewLord(faction, lordJobs[entry.Map], Current.Game.CurrentMap, null);
                                }

                                lords[entry.Map].AddPawn(entry);
                            }

                            removeSOP();
                            Find.LetterStack.RemoveLetter(this);

                            Find.LetterStack.ReceiveLetter("MFM_LetterMercSalaryIncreaseRejected".Translate(), "MFM_LetterMercSalaryIncreaseRejectedDesc".Translate(rogueMercs.Count,faction.Name), LetterDefOf.ThreatBig);
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
            Utils.GCMFM.CounterOfferInProgress = false;
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
            Scribe_References.Look<Faction>(ref this.faction, "faction", false);
            Scribe_Collections.Look<Pawn>(ref this.rogueMercs, "rogueMercs", LookMode.Reference);
            Scribe_Values.Look<int>(ref this.price, "price", 0, false);
        }
    }
}