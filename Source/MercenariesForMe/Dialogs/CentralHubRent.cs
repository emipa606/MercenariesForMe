using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

public class CentralHubRent : Window
{
    private readonly Pawn actor;
    private readonly Caravan caravan;
    private readonly Map map;
    private readonly List<Pawn> recall = [];
    private readonly ITrader trader;
    private readonly Dictionary<Pawn, MercenaryType> wanted = new();
    private Map backMap;
    private int delivery = 3;

    private Vector2 scrollPosition = Vector2.zero;

    public CentralHubRent(Pawn actor, Map map, Caravan caravan, ITrader trader = null)
    {
        this.trader = trader;
        this.actor = actor;
        this.caravan = caravan;
        if (map == null)
        {
            if (Utils.modernUSFM())
            {
                map = Utils.getRandomMapOfPlayer();
            }
            else
            {
                map = actor != null ? actor.Map : Utils.getRandomMapOfPlayer();
            }
        }

        backMap = Find.CurrentMap;
        this.map = map;
        forcePause = true;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
        closeOnClickedOutside = true;
    }

    public override Vector2 InitialSize => new(820f, 700f);

    public override void DoWindowContents(Rect inRect)
    {
        var recallCost = 0;
        var shipCost = 0;
        List<Pawn> toDel = null;
        inRect.yMin += 15f;
        inRect.yMax -= 15f;
        var defaultColumnWidth = inRect.width - 20;
        var list = new Listing_Standard { ColumnWidth = defaultColumnWidth };


        //Image logo
        Widgets.ButtonImage(new Rect(0, 0, 800, 170),
            Utils.modernUSFM() ? Tex.centralHubRent : Tex.medievalCentralHubRent, Color.white, Color.white);

        var outRect = new Rect(inRect.x, inRect.y + 180, inRect.width, inRect.height - 200);
        var scrollRect = new Rect(0f, 180f, inRect.width - 16f, inRect.height + (60 * wanted.Count) + 50);
        outRect.height -= 60;

        Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect);
        list.Begin(scrollRect);

        list.ButtonImage(Tex.catRent, 820, 50);

        if (list.ButtonText("MFM_AddPawn".Translate()))
        {
            showPawnList();
        }

        var salary = 0;

        foreach (var entry in wanted)
        {
            var comp = entry.Key.TryGetComp<Comp_USFM>();
            if (comp == null)
            {
                continue;
            }

            var isSlave = Utils.isSS_Slave(entry.Key);
            var slavePart = "";

            if (isSlave)
            {
                if (comp.slaveDecreaseIncome == 0.0f)
                {
                    comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
                }

                slavePart = $" ( -{(int)(comp.slaveDecreaseIncome * 100)}% " + "MFM_SlaveMercenary".Translate() +
                            ")";
            }

            list.Label(entry.Key.LabelCap + slavePart);

            var textComp = "";
            var typeText = (int)entry.Value == 9999 ? "-" : Utils.getReadableType(entry.Value);

            if ((int)entry.Value != 9999)
            {
                textComp = $" ( {Utils.getReadableLevel(Utils.getLevelFromSkill(entry.Key, entry.Value))} ) ";
            }

            if (list.ButtonText(typeText + textComp))
            {
                showTypeSelector(entry.Key);
            }

            GUI.color = Color.red;
            if (list.ButtonText("MFM_RemovePawn".Translate()))
            {
                toDel ??= [];

                toDel.Add(entry.Key);
            }

            GUI.color = Color.white;
            list.GapLine();

            var rate = 1 - Settings.rateDecreaseAppliedToRentedMercSalary;
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
                salary -= (int)(Settings.percentIncomeDecreaseSlave * salary);
            }
        }

        if (toDel != null)
        {
            foreach (var p in toDel)
            {
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

            if (list.RadioButton("MFM_ImmediateDelivery".Translate(Settings.transportImmediatePrice), delivery == 1))
            {
                delivery = 1;
            }

            if (list.RadioButton(
                    "MFM_QuickDelivery".Translate(Settings.transportQuickPrice, Settings.transportQuickMinHour,
                        Settings.transportQuickMaxHour), delivery == 2))
            {
                delivery = 2;
            }

            if (list.RadioButton(
                    "MFM_StandardDelivery".Translate(Settings.transportStandardMinHour,
                        Settings.transportStandardMaxHour), delivery == 3))
            {
                delivery = 3;
            }

            string mapText;
            if (backMap == Find.CurrentMap)
            {
                mapText = "MFM_ThisCurrentMap".Translate(backMap.GetUniqueLoadID());
            }
            else
            {
                mapText = backMap.GetUniqueLoadID();
            }

            //Addition of the Mercenary Arrival Map selector
            if (list.ButtonText(mapText))
            {
                var opts = new List<FloatMenuOption>();

                foreach (var m in Find.Maps)
                {
                    if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(m))
                    {
                        continue;
                    }

                    var mapText2 = m.GetUniqueLoadID();
                    if (m == backMap)
                    {
                        continue;
                    }

                    if (m == Find.CurrentMap)
                    {
                        mapText2 = "MFM_ThisCurrentMap".Translate(m.GetUniqueLoadID());
                    }

                    opts.Add(new FloatMenuOption(mapText2, delegate { backMap = m; }));
                }

                if (opts.Count != 0)
                {
                    var floatMenuMap = new FloatMenu(opts);
                    Find.WindowStack.Add(floatMenuMap);
                }
            }
        }

        foreach (var entry in recall)
        {
            var comp = entry.TryGetComp<Comp_USFM>();
            list.Label(entry.LabelCap);
            var typeText = Utils.getReadableType(comp.type);
            var textComp = $" ( {Utils.getReadableLevel(Utils.getLevelFromSkill(entry, comp.type))} ) ";

            list.ButtonText(typeText + textComp);
            GUI.color = Color.red;
            if (list.ButtonText("MFM_RemovePawn".Translate()))
            {
                toDel ??= [];

                toDel.Add(entry);
            }

            GUI.color = Color.white;
            list.GapLine();

            var isSlave = Utils.isSS_Slave(entry);

            if (isSlave)
            {
                if (comp.slaveDecreaseIncome == 0.0f)
                {
                    comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
                }
            }

            var decIfSlave = 0;
            if (isSlave)
            {
                decIfSlave = (int)(comp.salary * comp.slaveDecreaseIncome);
            }

            var rate = 1 - Settings.rateDecreaseAppliedToRentedMercSalary;

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
                {
                    recall.Remove(p);
                }
            }
        }

        list.End();
        Widgets.EndScrollView();

        //Calculation of non-consumed days
        var smap = Find.CurrentMap ?? Utils.getRandomMapOfPlayer();

        var salaryNonConsumed = (int)(salary *
                                      (GenDate.DayOfQuadrum(Find.TickManager.TicksAbs,
                                          Find.WorldGrid.LongLatOf(smap.Tile).x) / (float)15));

        var toPay = salary - salaryNonConsumed - recallCost - shipCost;


        if (wanted.Count != 0 || recall.Count != 0)
        {
            if (recall.Count != 0)
            {
                GUI.color = Color.red;
                Widgets.Label(new Rect(0f, 570f, 800f, 30f), "MFM_Recall".Translate($"-{recallCost}"));
                Widgets.Label(new Rect(250f, 570f, 800f, 30f), "MFM_Shipping".Translate($"-{shipCost}"));
            }

            GUI.color = Color.yellow;
            Widgets.Label(new Rect(0f, 600f, 800f, 30f), "MFM_Prorata".Translate($"-{salaryNonConsumed}"));
            GUI.color = Color.cyan;
            Widgets.Label(new Rect(250f, 600f, 800f, 30f), "MFM_IncomeThisMonth".Translate(toPay));
            //Calculation of monthly salaries
            GUI.color = Color.green;
            Widgets.Label(new Rect(500f, 600f, 800f, 30f), "MFM_Income".Translate(salary));
            GUI.color = Color.white;
        }

        if (wanted.Count == 0 && recall.Count == 0 || thereAreInvalidData())
        {
            GUI.color = Color.gray;
        }
        else
        {
            GUI.color = Color.green;
        }

        if (Widgets.ButtonText(new Rect(0f, 630f, 390f, 30f), "OK".Translate()))
        {
            var mapHasEnoughtSilver = false;

            if (toPay < 0)
            {
                //medieval mode
                if (caravan != null)
                {
                    var sum = Utils.moneyInCaravan(caravan);

                    if (sum >= toPay)
                    {
                        mapHasEnoughtSilver = true;
                    }
                }
                else
                {
                    if (Utils.modernUSFM())
                    {
                        if (map != null)
                        {
                            mapHasEnoughtSilver = TradeUtility.ColonyHasEnoughSilver(map, Math.Abs(toPay));
                        }
                    }
                    else
                    {
                        if (map != null && trader != null)
                        {
                            mapHasEnoughtSilver = Utils.ColonyHasEnoughLocalSilver(map, Math.Abs(toPay));
                        }
                    }
                }
            }
            else
            {
                mapHasEnoughtSilver = true;
            }

            if ((wanted.Count != 0 || recall.Count != 0) && !thereAreInvalidData() && mapHasEnoughtSilver)
            {
                var CGT = Find.TickManager.TicksGame;
                //Ordering the player's mercenary rental
                foreach (var m in wanted)
                {
                    var comp = m.Key.TryGetComp<Comp_USFM>();
                    comp.type = m.Value;
                    //Allowing later to increase the pawn's bio age
                    comp.startingRentGT = CGT;
                    Utils.setRentedMercSalary(m.Key);

                    //Starting date of the renting to calculate the prorated XP
                    _ = m.Key.Map ?? Utils.getRandomMapOfPlayer();

                    Utils.GCMFM.pushRentedMercenary(m.Key);
                    if (m.Key.Spawned)
                    {
                        m.Key.DeSpawn();
                    }

                    if (caravan != null)
                    {
                        if (caravan.pawns.Contains(m.Key))
                        {
                            caravan.pawns.Remove(m.Key);
                        }

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
                var thing = ThingMaker.MakeThing(ThingDefOf.Silver);
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
                        var dropSpot = DropCellFinder.TradeDropSpot(map);
                        if (!dropSpot.IsValid)
                        {
                            dropSpot = DropCellFinder.FindRaidDropCenterDistant(map);
                        }

                        TradeUtility.SpawnDropPod(dropSpot, map, thing);
                        Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercPaid".Translate(Utils.getUSFMLabel()),
                            "MFM_LetterRentedMercPaidDesc".Translate(salary - salaryNonConsumed, Utils.getUSFMLabel()),
                            LetterDefOf.PositiveEvent, new LookTargets(dropSpot, map));
                    }
                    else
                    {
                        //Planning caravan delivery
                        var data = new Dictionary<string, int>
                        {
                            ["map"] = map.uniqueID,
                            ["silver"] = salary - salaryNonConsumed
                        };

                        Utils.GCMFM.addPendingMedievalCaravan(0, data,
                            "MFM_LetterArrivedMedievalCaravanDeliveryRentedMercSilver".Translate());
                    }
                }

                Find.WindowStack.TryRemove(this);
            }
            else
            {
                if (thereAreInvalidData())
                {
                    Messages.Message("MFM_MsgMissingJobOnSomeColonistToRent".Translate(),
                        MessageTypeDefOf.NegativeEvent);
                }
                else if (toPay < 0 && !mapHasEnoughtSilver)
                {
                    Messages.Message("NotEnoughSilver".Translate(), MessageTypeDefOf.NegativeEvent);
                }
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
        var opts = new List<FloatMenuOption>();
        IEnumerable<Pawn> pawnSource;

        if (actor == null && caravan == null)
        {
            return;
        }

        if (actor == null && caravan != null)
        {
            pawnSource = caravan.pawns;
        }
        else
        {
            pawnSource = map.mapPawns.FreeColonistsSpawned;
        }

        foreach (var pawn in pawnSource.OrderBy(p => p.LabelCap))
        {
            var comp = pawn.TryGetComp<Comp_USFM>();

            if (!pawn.IsColonist || pawn.AnimalOrWildMan())
            {
                continue;
            }

            //We squeeze the mercenaries ==> just the colonists of the player and the negotiator
            if (actor != pawn && !wanted.ContainsKey(pawn) && comp is { isMercenary: false })
            {
                opts.Add(new FloatMenuOption(pawn.LabelCap, delegate
                    {
                        if (!Utils.canBeMercenary(pawn, out var msg))
                        {
                            Messages.Message(msg, MessageTypeDefOf.NegativeEvent);
                            return;
                        }

                        wanted[pawn] = (MercenaryType)9999;
                    }, MenuOptionPriority.Default, null, null, 29f,
                    rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f), pawn)));
            }
        }

        if (opts.Count == 0)
        {
            return;
        }

        var floatMenuMap = new FloatMenu(opts);
        Find.WindowStack.Add(floatMenuMap);
    }


    private void showPawnListToRecall()
    {
        var opts = new List<FloatMenuOption>();
        IEnumerable<Pawn> pawnSource = Utils.GCMFM.getRentedMercenaries();

        foreach (var pawn in pawnSource.OrderBy(p => p.LabelCap))
        {
            if (recall.Contains(pawn))
            {
                continue;
            }


            //Exclusion of pawns already being returned to the planned
            if (Utils.GCMFM.mercIsInPendingRentedMercComeBack(pawn.GetUniqueLoadID()))
            {
                continue;
            }

            pawn.TryGetComp<Comp_USFM>();

            opts.Add(new FloatMenuOption(pawn.LabelCap, delegate
                {
                    if (!recall.Contains(pawn))
                    {
                        recall.Add(pawn);
                    }
                }, MenuOptionPriority.Default, null, null, 29f,
                rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f), pawn)));
        }

        if (opts.Count == 0)
        {
            return;
        }

        var floatMenuMap = new FloatMenu(opts);
        Find.WindowStack.Add(floatMenuMap);
    }

    private void showTypeSelector(Pawn pawn)
    {
        var opts = new List<FloatMenuOption>();
        foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
        {
            opts.Add(new FloatMenuOption(Utils.getReadableType(type), delegate
            {
                //Check if pawn can be rerouted for this guy (not incapable)
                var skill = Utils.getAssociatedSkill(pawn, type);
                if (skill == null)
                {
                    return;
                }

                if (skill.TotallyDisabled)
                {
                    Messages.Message("MFM_MsgPawnBeRentedAsMercInvalidType".Translate(pawn.LabelCap),
                        MessageTypeDefOf.NegativeEvent);
                    return;
                }

                wanted[pawn] = type;
            }));
        }

        if (opts.Count == 0)
        {
            return;
        }

        var floatMenuMap = new FloatMenu(opts);
        Find.WindowStack.Add(floatMenuMap);
    }


    private bool thereAreInvalidData()
    {
        foreach (var entry in wanted)
        {
            if ((int)entry.Value == 9999)
            {
                return true;
            }
        }

        return false;
    }
}