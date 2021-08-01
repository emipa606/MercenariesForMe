using System;
using Verse;
using RimWorld;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class CentralHubRent : Window
    {
        public Dictionary<Pawn, MercenaryType> wanted = new Dictionary<Pawn, MercenaryType>();
        public List<Pawn> recall = new List<Pawn>();

        public Vector2 scrollPosition = Vector2.zero;
        public Map map;
        public Pawn actor;
        public Caravan caravan;
        public ITrader trader;
        public int delivery=3;
        public Map backMap = null;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(820f, 700f);
            }
        }

        public CentralHubRent(Pawn actor,Map map, Caravan caravan, ITrader trader = null)
        {
            this.trader = trader;
            this.actor = actor;
            this.caravan = caravan;
            if (map == null)
            {
                if(Utils.modernUSFM())
                    map = Utils.getRandomMapOfPlayer();
                else{
                    if (actor != null)
                        map = actor.Map;
                    else
                        map = Utils.getRandomMapOfPlayer();
                }
                
            }
            this.backMap = Find.CurrentMap;
            this.map = map;
            this.forcePause = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            int recallCost = 0;
            int shipCost = 0;
            List<Pawn> toDel = null;
            inRect.yMin += 15f;
            inRect.yMax -= 15f;
            var defaultColumnWidth = (inRect.width - 20);
            Listing_Standard list = new Listing_Standard() { ColumnWidth = defaultColumnWidth };


            //Image logo
            if (Utils.modernUSFM())
                Widgets.ButtonImage(new Rect(0, 0, 800, 170), Tex.centralHubRent, Color.white, Color.white);
            else
                Widgets.ButtonImage(new Rect(0, 0, 800, 170), Tex.medievalCentralHubRent, Color.white, Color.white);

            var outRect = new Rect(inRect.x, inRect.y + 180, inRect.width, inRect.height - 200);
            var scrollRect = new Rect(0f, 180f, inRect.width - 16f, inRect.height + (60*wanted.Count) + 50);
            outRect.height -= 60;

            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect, true);
            list.Begin(scrollRect);

            list.ButtonImage(Tex.catRent, 820, 50);

            if (list.ButtonText("MFM_AddPawn".Translate()))
            {
                showPawnList();
            }

            int salary = 0;

            foreach(var entry in wanted)
            {
                Comp_USFM comp = entry.Key.TryGetComp<Comp_USFM>();
                if (comp == null)
                    continue;
                bool isSlave = Utils.isSS_Slave(entry.Key);
                string slavePart = "";

                if (isSlave)
                {
                    if (comp.slaveDecreaseIncome == 0.0f)
                    {
                        comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
                    }

                    slavePart = " ( -" + ((int)(comp.slaveDecreaseIncome * 100)) + "% " + ("MFM_SlaveMercenary".Translate()) + ")";
                }

                list.Label(entry.Key.LabelCap + slavePart);

                string typeText = "";
                string textComp = "";
                if ((int)entry.Value == 9999)
                    typeText = "-";
                else
                    typeText = Utils.getReadableType(entry.Value);

                if((int)entry.Value != 9999)
                    textComp = " ( " + Utils.getReadableLevel(Utils.getLevelFromSkill(entry.Key, entry.Value)) + " ) ";

                if (list.ButtonText( typeText +textComp))
                {
                    showTypeSelector(entry.Key);
                }
                GUI.color = Color.red;
                if (list.ButtonText("MFM_RemovePawn".Translate()))
                {
                    if (toDel == null)
                        toDel = new List<Pawn>();
                    toDel.Add(entry.Key);
                }
                GUI.color = Color.white;
                list.GapLine();

                float rate = (1 - Settings.rateDecreaseAppliedToRentedMercSalary);
                switch (Utils.getLevelFromSkill(entry.Key, entry.Value))
                {
                    case MercenaryLevel.Recruit:
                        salary += (int)(Settings.priceRecruit * rate);
                        break;
                    case MercenaryLevel.Confirmed:
                        salary += (int)(Settings.priceConfirmed * rate);
                        break;
                    case MercenaryLevel.Veteran:
                        salary += (int)(Settings.priceVeteran * rate);
                        break;
                    case MercenaryLevel.Elite:
                        salary += (int)(Settings.priceElite * rate);
                        break;
                    case MercenaryLevel.Cyborg:
                        salary += (int)(Settings.priceCyborg * rate);
                        break;
                }

                if (isSlave)
                {
                    salary -= ((int) (Settings.percentIncomeDecreaseSlave*salary));
                }
            }

            if(toDel != null)
            {
                foreach(var p in toDel)
                {
                    if (wanted.ContainsKey(p))
                        wanted.Remove(p);
                }
            }

            //Call-back mercs
            list.Gap();
            list.Gap();
            list.Gap();
            list.ButtonImage(Tex.catRecall, 820, 50);

            if (list.ButtonText("MFM_AddPawnRecall".Translate()))
            {
                showPawnListToRecall();
            }

            if (recall.Count != 0)
            {
                //Shipping section
                list.ButtonImage(Tex.catShipping, 820, 50);
                list.Gap(6);

                if (list.RadioButton("MFM_ImmediateDelivery".Translate(Settings.transportImmediatePrice), (delivery == 1)))
                    delivery = 1;
                if (list.RadioButton("MFM_QuickDelivery".Translate(Settings.transportQuickPrice, Settings.transportQuickMinHour, Settings.transportQuickMaxHour), (delivery == 2)))
                    delivery = 2;
                if (list.RadioButton("MFM_StandardDelivery".Translate(Settings.transportStandardMinHour, Settings.transportStandardMaxHour), (delivery == 3)))
                    delivery = 3;

                string mapText = "";
                if (backMap == Find.CurrentMap)
                    mapText = "MFM_ThisCurrentMap".Translate(backMap.GetUniqueLoadID());
                else
                    mapText = backMap.GetUniqueLoadID();

                //Addition of the Mercenary Arrival Map selector
                if (list.ButtonText(mapText))
                {
                    List<FloatMenuOption> opts = new List<FloatMenuOption>();

                    foreach (var m in Find.Maps)
                    {
                        string mapText2 = m.GetUniqueLoadID();
                        if (m != backMap)
                        {
                            if (m == Find.CurrentMap)
                                mapText2 = "MFM_ThisCurrentMap".Translate(m.GetUniqueLoadID());
                            opts.Add(new FloatMenuOption(mapText2, delegate { backMap = m; }, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                    }
                    if (opts.Count != 0)
                    {
                        FloatMenu floatMenuMap = new FloatMenu(opts);
                        Find.WindowStack.Add(floatMenuMap);
                    }
                }
            }

            foreach (var entry in recall)
            {
                Comp_USFM comp = entry.TryGetComp<Comp_USFM>();
                list.Label(entry.LabelCap);
                string typeText = "";
                string textComp = "";
                typeText = Utils.getReadableType(comp.type);
                textComp = " ( " + Utils.getReadableLevel(Utils.getLevelFromSkill(entry, comp.type)) + " ) ";

                list.ButtonText(typeText + textComp);
                GUI.color = Color.red;
                if (list.ButtonText("MFM_RemovePawn".Translate()))
                {
                    if (toDel == null)
                        toDel = new List<Pawn>();
                    toDel.Add(entry);
                }
                GUI.color = Color.white;
                list.GapLine();

                bool isSlave = Utils.isSS_Slave(entry);

                if (isSlave)
                {
                    if (comp.slaveDecreaseIncome == 0.0f)
                    {
                        comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
                    }
                }

                int decIfSlave = 0;
                if (isSlave)
                {
                    decIfSlave = ((int)(comp.salary * comp.slaveDecreaseIncome));
                }

                float rate = 1 - Settings.rateDecreaseAppliedToRentedMercSalary;

                switch (Utils.getLevelFromSkill(entry, comp.type))
                {
                    case MercenaryLevel.Recruit:
                        recallCost += (int)(Settings.priceRecruit * rate) - decIfSlave;
                        break;
                    case MercenaryLevel.Confirmed:
                        recallCost += (int)(Settings.priceConfirmed * rate) - decIfSlave;
                        break;
                    case MercenaryLevel.Veteran:
                        recallCost += (int)(Settings.priceVeteran * rate) - decIfSlave;
                        break;
                    case MercenaryLevel.Elite:
                        recallCost += (int)(Settings.priceElite * rate) - decIfSlave;
                        break;
                    case MercenaryLevel.Cyborg:
                        recallCost += (int)(Settings.priceCyborg * rate) - decIfSlave;
                        break;
                }

                
                switch (delivery)
                {
                    case 2:
                        shipCost = Settings.transportQuickPrice;
                        break;
                    case 1:
                        shipCost = Settings.transportImmediatePrice;
                        break;
                }
            }

            if (toDel != null)
            {
                foreach (var p in toDel)
                {
                    if (recall.Contains(p))
                        recall.Remove(p);
                }
            }

            list.End();
            Widgets.EndScrollView();

            //Calculation of non-consumed days
            Map smap = Find.CurrentMap;
            if (smap == null)
                smap = Utils.getRandomMapOfPlayer();

            int salaryNonConsumed = (int)(((float)salary) * ((float)(GenDate.DayOfQuadrum(Find.TickManager.TicksAbs, Find.WorldGrid.LongLatOf(smap.Tile).x) / (float)15)));

            int toPay = salary - salaryNonConsumed - recallCost - shipCost;

            bool mapHasEnoughtSilver = false;

            if (toPay < 0)
            {
                //medieval mode
                if (caravan != null)
                {
                    int sum = Utils.moneyInCaravan(caravan);

                    if (sum >= toPay)
                        mapHasEnoughtSilver = true;
                }
                else
                {
                    if (Utils.modernUSFM())
                    {
                        if(map != null)
                            mapHasEnoughtSilver = TradeUtility.ColonyHasEnoughSilver(map, Math.Abs(toPay));
                    }
                    else
                    {
                        if (map != null && trader != null)
                            mapHasEnoughtSilver = Utils.ColonyHasEnoughLocalSilver(map, Math.Abs(toPay));
                    }
                }
            }
            else
                mapHasEnoughtSilver = true;

            if (wanted.Count != 0 || recall.Count != 0)
            {
                if (recall.Count != 0)
                {
                    GUI.color = Color.red;
                    Widgets.Label(new Rect(0f, 570f, 800f, 30f), "MFM_Recall".Translate("-" + recallCost));
                    Widgets.Label(new Rect(250f, 570f, 800f, 30f), "MFM_Shipping".Translate("-" + shipCost));
                }

                GUI.color = Color.yellow;
                Widgets.Label(new Rect(0f, 600f, 800f, 30f), "MFM_Prorata".Translate("-"+salaryNonConsumed));
                GUI.color = Color.cyan;
                Widgets.Label(new Rect(250f, 600f, 800f, 30f), "MFM_IncomeThisMonth".Translate(toPay));
                //Calculation of monthly salaries
                GUI.color = Color.green;
                Widgets.Label(new Rect(500f, 600f, 800f, 30f), "MFM_Income".Translate(salary ));
                GUI.color = Color.white;
            }

            if ( (wanted.Count == 0 && recall.Count == 0) || thereAreInvalidData())
                GUI.color = Color.gray;
            else
                GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(0f, 630f, 390f, 30f), "OK".Translate()))
            {
                if ((wanted.Count != 0 || recall.Count != 0) && !thereAreInvalidData() && mapHasEnoughtSilver)
                {
                    //Ordering the player's mercenary rental
                    foreach (var m in wanted)
                    {
                        Comp_USFM comp = m.Key.TryGetComp<Comp_USFM>();
                        comp.type = m.Value;

                        Utils.setRentedMercSalary(m.Key);

                        //Starting date of the renting to calculate the prorated XP
                        Map smap2 = m.Key.Map;
                        if (smap2 == null)
                            smap2 = Utils.getRandomMapOfPlayer();

                        Utils.GCMFM.pushRentedMercenary(m.Key);
                        if (m.Key.Spawned)
                            m.Key.DeSpawn();
                        if(caravan != null)
                        {
                            if(caravan.pawns.Contains(m.Key))
                                caravan.pawns.Remove(m.Key);

                            //If the caravan is empty, we delete it
                            if (caravan.pawns.Count == 0)
                            {
                                Find.WorldObjects.Remove(caravan);
                            }
                        }
                        Utils.GCMFM.pushRentedPawn(m.Key);
                    }
                    //Catering hired mercenaries
                    if (recall.Count != 0)
                    {
                        Utils.GCMFM.addPendingRendMercComeBack(Utils.getDeliveryGT(delivery), recall, backMap);
                    }

                    //Money shipments
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);
                    //Money payment
                    if (toPay < 0)
                    {
                        if (caravan == null)
                        {
                            if (Utils.modernUSFM())
                            {
                                TradeUtility.LaunchSilver(map, Math.Abs(toPay));
                            }
                            else
                            {
                                Utils.payLocalSilver(Math.Abs(toPay), map, trader);
                            }
                        }
                        else
                        {
                            Utils.caravanPayCost(caravan, Math.Abs(toPay));
                        }
                    }
                    //Money shipments
                    else if (toPay > 0)
                    {
                        if (Utils.modernUSFM())
                        {
                            thing.stackCount = salary - salaryNonConsumed;
                            IntVec3 dropSpot = DropCellFinder.TradeDropSpot(map);
                            if (!dropSpot.IsValid)
                            {
                                dropSpot = DropCellFinder.FindRaidDropCenterDistant(map);
                            }
                            TradeUtility.SpawnDropPod(dropSpot, map, thing);
                            Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercPaid".Translate(Utils.getUSFMLabel()), "MFM_LetterRentedMercPaidDesc".Translate(salary - salaryNonConsumed, Utils.getUSFMLabel()), LetterDefOf.PositiveEvent, new LookTargets(dropSpot, map));

                        }
                        else
                        {
                            //Planning caravan delivery
                            Dictionary<string, int> data = new Dictionary<string, int>();
                            data["map"] = map.uniqueID;
                            data["silver"] = salary - salaryNonConsumed;

                            Utils.GCMFM.addPendingMedievalCaravan(0, data, "MFM_LetterArrivedMedievalCaravanDeliveryRentedMercSilver".Translate());
                        }
                    }

                    Find.WindowStack.TryRemove(this);
                }
                else
                {
                    if (thereAreInvalidData())
                        Messages.Message("MFM_MsgMissingJobOnSomeColonistToRent".Translate(), MessageTypeDefOf.NegativeEvent);
                    else if(toPay < 0 && !mapHasEnoughtSilver)
                        Messages.Message("NotEnoughSilver".Translate(), MessageTypeDefOf.NegativeEvent);

                }
            }
            GUI.color = Color.red;
            if (Widgets.ButtonText(new Rect(390f, 630f, 390f, 30f), "GoBack".Translate()))
            {
                Find.WindowStack.TryRemove(this);
            }
            GUI.color = Color.white;
        }

        private void showPawnList()
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            IEnumerable<Pawn> pawnSource = null;

            if (actor == null && caravan == null)
                return;

            if(actor == null && caravan != null)
            {
                pawnSource = caravan.pawns;
            }
            else
            {
                pawnSource = map.mapPawns.FreeColonistsSpawned;
            }

            foreach (var pawn in pawnSource.OrderBy(p => p.LabelCap))
            {
                Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();

                if (!pawn.IsColonist || pawn.AnimalOrWildMan())
                    continue;

                //We squeeze the mercenaries ==> just the colonists of the player and the negotiator
                if (actor != pawn && !wanted.ContainsKey(pawn) && ( comp != null && !comp.isMercenary ))
                {
                    opts.Add(new FloatMenuOption(pawn.LabelCap, delegate
                    {
                        string msg = "";
                        if (!Utils.canBeMercenary(pawn, out msg))
                        {
                            Messages.Message(msg, MessageTypeDefOf.NegativeEvent);
                            return;
                        }
                        wanted[pawn] = (MercenaryType)9999;
                    }, MenuOptionPriority.Default, null, null, 29f,(Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, pawn), null));
                }
            }
            if (opts.Count != 0)
            {
                FloatMenu floatMenuMap = new FloatMenu(opts);
                Find.WindowStack.Add(floatMenuMap);
            }
        }


        private void showPawnListToRecall()
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            IEnumerable<Pawn> pawnSource = Utils.GCMFM.getRentedMercenaries();

            foreach (var pawn in pawnSource.OrderBy(p => p.LabelCap))
            {
                if (recall.Contains(pawn))
                    continue;


                //Exclusion of pawns already being returned to the planned
                if (Utils.GCMFM.mercIsInPendingRentedMercComeBack(pawn.GetUniqueLoadID()))
                    continue;

                Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();

                opts.Add(new FloatMenuOption(pawn.LabelCap, delegate
                {
                    if(!recall.Contains(pawn))
                        recall.Add(pawn);

                }, MenuOptionPriority.Default, null, null, 29f, (Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, pawn), null));
            }
            if (opts.Count != 0)
            {
                FloatMenu floatMenuMap = new FloatMenu(opts);
                Find.WindowStack.Add(floatMenuMap);
            }
        }

        private void showTypeSelector(Pawn pawn)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
            {
                opts.Add(new FloatMenuOption(Utils.getReadableType(type), delegate
                {
                    //Check if pawn can be rerouted for this guy (not incapable)
                    SkillRecord skill = Utils.getAssociatedSkill(pawn, type);
                    if (skill == null)
                        return;
                    if (skill.TotallyDisabled)
                    {
                        Messages.Message("MFM_MsgPawnBeRentedAsMercInvalidType".Translate(pawn.LabelCap), MessageTypeDefOf.NegativeEvent);
                        return;
                    }
                    wanted[pawn] = type;
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            if (opts.Count != 0)
            {
                FloatMenu floatMenuMap = new FloatMenu(opts);
                Find.WindowStack.Add(floatMenuMap);
            }
        }


        private bool thereAreInvalidData()
        {
            foreach(var entry in wanted)
            {
                if ((int)entry.Value == 9999)
                    return true;
            }
            return false;
        }
    
    }
}