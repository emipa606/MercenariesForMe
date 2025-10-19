using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

public class Bill : Window
{
    private readonly Caravan caravanOverSOP;
    private readonly ChoiceLetter_Bill cl;
    private readonly bool summarize;
    private Map backMap;
    private Map backMapStuffAndGuarantee;
    private Vector2 scrollPosition = Vector2.zero;

    public Bill(ChoiceLetter_Bill cl, bool summarize = false)
    {
        this.cl = cl;
        forcePause = true;
        doCloseX = false;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
        if (summarize)
        {
            closeOnCancel = true;
            closeOnClickedOutside = true;
        }
        else
        {
            closeOnCancel = false;
            closeOnClickedOutside = false;
        }

        backMap = Find.CurrentMap;
        backMapStuffAndGuarantee = Find.CurrentMap;
        this.summarize = summarize;

        if (!Utils.modernUSFM())
        {
            caravanOverSOP = Utils.caravanOfPlayerOverSiteOfPayment();
        }
    }

    public override Vector2 InitialSize => new(820f, 700f);


    public override void DoWindowContents(Rect inRect)
    {
        //List<Pawn> toDel = null;
        inRect.yMin += 15f;
        inRect.yMax -= 15f;
        var defaultColumnWidth = inRect.width - 20;
        var list = new Listing_Standard { ColumnWidth = defaultColumnWidth };

        //Image logo
        if (summarize)
        {
            Widgets.ButtonImage(new Rect(0, 0, 800, 170), Utils.modernUSFM() ? Tex.summary : Tex.medievalSummary,
                Color.white, Color.white);
        }
        else
        {
            Widgets.ButtonImage(new Rect(0, 0, 800, 170),
                Utils.modernUSFM() ? Tex.quadrumBill : Tex.medievalQuadrumBill, Color.white, Color.white);
        }

        var outRect = new Rect(inRect.x, inRect.y + 210, inRect.width, inRect.height - 230);
        var scrollRect = new Rect(0f, 180f, inRect.width - 16f, (inRect.height * 10f) + 50);
        outRect.height -= 60;

        //Display selector
        if (cl.showMode == 0)
        {
            GUI.color = Color.gray;
        }

        if (Widgets.ButtonText(new Rect(0f, 180f, 395f, 30f), "MFM_QuadBillShowCost".Translate()))
        {
            cl.showMode = 0;
        }

        GUI.color = Color.white;
        if (cl.showMode == 1)
        {
            GUI.color = Color.gray;
        }

        if (Widgets.ButtonText(new Rect(395f, 180f, 395f, 30f), "MFM_QuadBillShowIncome".Translate()))
        {
            cl.showMode = 1;
        }

        GUI.color = Color.white;

        Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect);
        list.Begin(scrollRect);

        bool tmp;
        var cost = 0;
        var income = 0;
        var nb = 0;
        var nbr = 0;
        float cdiscountVisual = 0;
        var recoverableGuarantee = 0;
        var totalGuarantee = 0;

        //If orders in progress, they are displayed in order to be able to cancel them
        if (cl.showMode == 0 && cl.pendingMercOrders.Count != 0)
        {
            list.ButtonImage(Tex.catCurrentOrders, 820, 50);

            var pending = Utils.GCMFM.getPendingMercOrder();

            foreach (var entry in pending)
            {
                if (!cl.pendingMercOrders.ContainsKey(entry.Key))
                {
                    continue;
                }

                var poWanted = Utils.unserializeDSI(pending[entry.Key]);
                Utils.getWantedInfo(poWanted, out var nbMerc, out var price);
                tmp = cl.pendingMercOrders[entry.Key];
                if (summarize)
                {
                    list.Label("MFM_QuadBillMercListHiredPendingOrder".Translate(nbMerc, price));
                }
                else
                {
                    list.CheckboxLabeled("MFM_QuadBillMercListHiredPendingOrder".Translate(nbMerc, price), ref tmp);
                }

                cl.pendingMercOrders[entry.Key] = tmp;
                if (tmp)
                {
                    cost += price;
                }
            }
        }

        //Display option allowing to select the return map of the stuffs and deposits
        if (cl.showMode == 0 && !summarize)
        {
            list.ButtonImage(Tex.catStuffAndGuarantee, 820, 50);
            list.Gap();
            list.Gap();
            string mapText;
            if (backMapStuffAndGuarantee == Find.CurrentMap)
            {
                mapText = "MFM_ThisCurrentMap".Translate(backMapStuffAndGuarantee.GetUniqueLoadID());
            }
            else
            {
                mapText = backMapStuffAndGuarantee.GetUniqueLoadID();
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
                    if (m == backMapStuffAndGuarantee)
                    {
                        continue;
                    }

                    if (m == Find.CurrentMap)
                    {
                        mapText2 = "MFM_ThisCurrentMap".Translate(m.GetUniqueLoadID());
                    }

                    opts.Add(new FloatMenuOption(mapText2, delegate { backMapStuffAndGuarantee = m; }));
                }

                if (opts.Count != 0)
                {
                    var floatMenuMap = new FloatMenu(opts);
                    Find.WindowStack.Add(floatMenuMap);
                }
            }
        }

        var sel = new Dictionary<Pawn, bool>();
        var thereIsRentedMercWhichNeedStop = false;

        foreach (var m in cl.rented)
        {
            if (!m.Value)
            {
                thereIsRentedMercWhichNeedStop = true;
            }
        }


        if (cl.showMode == 1 && thereIsRentedMercWhichNeedStop && !summarize)
        {
            //Shipping section
            list.ButtonImage(Tex.catShipping, 820, 50);
            list.Gap(6);

            if (list.RadioButton("MFM_ImmediateDelivery".Translate(Settings.transportImmediatePrice), cl.delivery == 1))
            {
                cl.delivery = 1;
            }

            if (list.RadioButton(
                    "MFM_QuickDelivery".Translate(Settings.transportQuickPrice, Settings.transportQuickMinHour,
                        Settings.transportQuickMaxHour), cl.delivery == 2))
            {
                cl.delivery = 2;
            }

            if (list.RadioButton(
                    "MFM_StandardDelivery".Translate(Settings.transportStandardMinHour,
                        Settings.transportStandardMaxHour), cl.delivery == 3))
            {
                cl.delivery = 3;
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

        foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
        {
            //Display of costs
            sel.Clear();
            foreach (var m in cl.wanted)
            {
                if (m.Key.TryGetComp<Comp_USFM>().type == type)
                {
                    sel[m.Key] = m.Value;
                }
            }

            //If category exists, we list the mercenaries with checkbox
            if (sel.Count > 0)
            {
                if (cl.showMode == 0)
                {
                    list.ButtonImage(Utils.getMercenaryCategoryCover(type), 820, 50);
                }

                foreach (var m in sel)
                {
                    var comp = m.Key.TryGetComp<Comp_USFM>();
                    var newScore = Utils.getPawnScore(m.Key);
                    int cGuarantee;
                    if (comp.origScore > newScore)
                    {
                        cGuarantee = comp.guarantee -
                                     (int)(comp.guarantee * ((comp.origScore - newScore) / comp.origScore));
                    }
                    else
                    {
                        cGuarantee = comp.guarantee;
                    }

                    recoverableGuarantee += cGuarantee;

                    totalGuarantee += comp.guarantee;

                    var skill = Utils.getAssociatedSkill(m.Key, type);
                    var skillValue = 0;
                    if (skill != null)
                    {
                        skillValue = skill.levelInt;
                    }

                    tmp = m.Value;
                    if (cl.showMode == 0)
                    {
                        var guaranteeTxt = "";
                        if (summarize)
                        {
                            guaranteeTxt = "MFM_QuadBillGuaranteePart".Translate($"{cGuarantee}/{comp.guarantee}");
                        }

                        var label = $"{m.Key.LabelCap} {guaranteeTxt}";

                        if (comp.quadNbHourMoodBad >= Settings.badMoodNbhPerQuadFloor)
                        {
                            if (summarize)
                            {
                                list.Label("MFM_QuadBillMercListHiredNotHappy".Translate(label,
                                    Utils.getReadableLevel(comp.Level), skillValue,
                                    comp.salary * (1 + Settings.badMoodCoefIncCost), comp.salary,
                                    (int)(Settings.badMoodCoefIncCost * 100)));
                            }
                            else
                            {
                                list.CheckboxLabeled(
                                    "MFM_QuadBillMercListHiredNotHappy".Translate(label,
                                        Utils.getReadableLevel(comp.Level), skillValue,
                                        comp.salary * (1 + Settings.badMoodCoefIncCost), comp.salary,
                                        (int)(Settings.badMoodCoefIncCost * 100)), ref tmp);
                            }
                        }
                        else if (comp.quadNbHourMoodOK >= Settings.goodMoodNbhPerQuadFloor)
                        {
                            if (summarize)
                            {
                                list.Label("MFM_QuadBillMercListHiredHappy".Translate(label,
                                    Utils.getReadableLevel(comp.Level), skillValue,
                                    comp.salary - (Settings.goodMoodCoefDecCost * comp.salary), comp.salary,
                                    (int)(Settings.goodMoodCoefDecCost * 100)));
                            }
                            else
                            {
                                list.CheckboxLabeled(
                                    "MFM_QuadBillMercListHiredHappy".Translate(label,
                                        Utils.getReadableLevel(comp.Level), skillValue,
                                        comp.salary - (Settings.goodMoodCoefDecCost * comp.salary), comp.salary,
                                        (int)(Settings.goodMoodCoefDecCost * 100)), ref tmp);
                            }
                        }
                        else
                        {
                            var isSlave = Utils.isSS_Slave(m.Key);
                            var slavePart = "";
                            var salary = comp.salary;

                            if (isSlave)
                            {
                                if (comp.slaveDecreaseIncome == 0.0f)
                                {
                                    comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
                                }

                                slavePart = $"( -{(int)(comp.slaveDecreaseIncome * 100)}% " +
                                            "MFM_SlaveMercenary".Translate() + ")";
                                salary -= (int)(comp.salary * comp.slaveDecreaseIncome);
                            }

                            if (summarize)
                            {
                                list.Label("MFM_QuadBillMercListHired".Translate(label,
                                    Utils.getReadableLevel(comp.Level), skillValue, salary, slavePart));
                            }
                            else
                            {
                                list.CheckboxLabeled(
                                    "MFM_QuadBillMercListHired".Translate(label, Utils.getReadableLevel(comp.Level),
                                        skillValue, salary, slavePart), ref tmp);
                            }
                        }
                    }

                    cl.wanted[m.Key] = tmp;
                    if (!tmp)
                    {
                        continue;
                    }

                    if (comp.quadNbHourMoodBad >= Settings.badMoodNbhPerQuadFloor)
                    {
                        cost += comp.salary + (int)(comp.salary * Settings.badMoodCoefIncCost);
                    }
                    else if (comp.quadNbHourMoodOK >= Settings.goodMoodNbhPerQuadFloor)
                    {
                        cost += comp.salary - (int)(comp.salary * Settings.goodMoodCoefDecCost);
                    }
                    else
                    {
                        cost += comp.salary;
                    }

                    nb++;
                }
            }

            sel.Clear();

            //Income display
            foreach (var m in cl.rented)
            {
                if (m.Key.TryGetComp<Comp_USFM>().type == type)
                {
                    sel[m.Key] = m.Value;
                }

                //if (!m.Value)
                //  thereIsRentedMercWhichNeedStop = true;
            }

            //If category exists, we list the mercenaries with checkbox
            if (sel.Count <= 0)
            {
                continue;
            }

            if (cl.showMode == 1)
            {
                list.ButtonImage(Utils.getMercenaryCategoryCover(type), 820, 50);
            }

            foreach (var m in sel)
            {
                var isSlave = Utils.isSS_Slave(m.Key);
                var slavePart = "";
                var comp = m.Key.TryGetComp<Comp_USFM>();
                var salary = comp.salary;

                if (isSlave)
                {
                    if (comp.slaveDecreaseIncome == 0.0f)
                    {
                        comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
                    }

                    slavePart = $"( -{(int)(comp.slaveDecreaseIncome * 100)}% " +
                                "MFM_SlaveMercenary".Translate() + ")";
                    salary -= (int)(comp.salary * comp.slaveDecreaseIncome);
                }

                var skill = Utils.getAssociatedSkill(m.Key, type);
                var skillValue = 0;
                if (skill != null)
                {
                    skillValue = skill.levelInt;
                }

                tmp = m.Value;
                if (cl.showMode == 1)
                {
                    if (summarize)
                    {
                        list.Label("MFM_QuadBillMercListHired".Translate(m.Key.LabelCap,
                            Utils.getReadableLevel(comp.Level), skillValue, salary, slavePart));
                    }
                    else
                    {
                        list.CheckboxLabeled(
                            "MFM_QuadBillMercListHired".Translate(m.Key.LabelCap,
                                Utils.getReadableLevel(comp.Level), skillValue, salary, slavePart), ref tmp);
                    }
                }

                cl.rented[m.Key] = tmp;
                if (!tmp)
                {
                    continue;
                }

                income += salary;
                nbr++;
            }
        }


        list.End();
        Widgets.EndScrollView();

        //Calculation of the mercenaries of the player who must return to calculate the shipping Cost if applicable
        var nbPawnComeBack = cl.rented.Count - nbr;
        var shipCost = 0;
        switch (cl.delivery)
        {
            case 2:
                shipCost = Settings.transportQuickPrice;
                break;
            case 1:
                shipCost = Settings.transportImmediatePrice;
                break;
        }

        shipCost *= nbPawnComeBack;
        var total = income - (cost + shipCost);
        if (Utils.GCMFM.getDiscount(cl.discount, out var cdiscount))
        {
            cdiscountVisual = (int)(cdiscount * 100);
        }

        if (cdiscount != 0)
        {
            //Applicable only if negative invoice
            if (total < 0)
            {
                total += (int)Math.Abs(total * cdiscount);
            }
        }

        GUI.color = Color.cyan;
        Widgets.Label(new Rect(0f, 580f, 800f, 30f), "MFM_QuadBillCountHiredMerc".Translate(nb));
        Widgets.Label(new Rect(200f, 580f, 800f, 30f), "MFM_QuadBillCountRentedMerc".Translate(nbr));
        if (!summarize)
        {
            Widgets.Label(new Rect(400f, 580f, 800f, 30f), "MFM_Shipping".Translate(shipCost));
            GUI.color = Color.yellow;
            Widgets.Label(new Rect(600f, 580f, 800f, 30f), "MFM_Discount".Translate(cdiscountVisual));
            GUI.color = Color.white;

            var discountTxt = "-";
            if (!string.IsNullOrEmpty(cl.discount))
            {
                discountTxt = cl.discount;
            }

            if (Widgets.ButtonText(new Rect(600f, 600f, 180f, 30f), discountTxt))
            {
                Utils.showDiscountCodeList(delegate(string code) { cl.discount = code; });
            }

            //cl.discount = Widgets.TextField(new Rect(600f, 600f, 180f, 30f), cl.discount);
        }
        else
        {
            GUI.color = Color.yellow;
            Widgets.Label(new Rect(400f, 580f, 800f, 30f),
                "MFM_RecoverableGuarantee".Translate($"{recoverableGuarantee}/{totalGuarantee}"));
        }

        GUI.color = Color.cyan;

        Widgets.Label(new Rect(0f, 600f, 800f, 30f), "MFM_QuadBillCost".Translate(cost));
        Widgets.Label(new Rect(200f, 600f, 800f, 30f), "MFM_QuadBillIncome".Translate(income));
        GUI.color = total < 0 ? new Color(0.960f, 0.490f, 0.039f, 1.0f) : Color.green;

        Widgets.Label(new Rect(400f, 600f, 800f, 30f),
            summarize ? "MFM_QuadBillTotalSummarize".Translate(total) : "MFM_QuadBillTotal".Translate(total));

        GUI.color = Color.green;


        if (summarize)
        {
            GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(0f, 630f, 820f, 30f), "OK".Translate()))
            {
                Find.WindowStack.TryRemove(this);
            }
        }
        else
        {
            if (Widgets.ButtonText(new Rect(0f, 630f, 395f, 30f), "OK".Translate()))
            {
                var mapHasEnoughtSilver = false;
                if (Utils.modernUSFM())
                {
                    mapHasEnoughtSilver = TradeUtility.ColonyHasEnoughSilver(backMapStuffAndGuarantee, Math.Abs(total));
                }
                else
                {
                    if (caravanOverSOP != null)
                    {
                        if (Utils.moneyInCaravan(caravanOverSOP) >= Math.Abs(total))
                        {
                            mapHasEnoughtSilver = true;
                        }
                    }
                    else
                    {
                        if (total >= 0)
                        {
                            mapHasEnoughtSilver = true;
                        }
                    }
                }

                if (total < 0 && !mapHasEnoughtSilver)
                {
                    GUI.color = Color.gray;
                }

                //If the player has enough money in one of his colonies
                if (total >= 0 || mapHasEnoughtSilver)
                {
                    //If medieval mode in progress check that caravan above payment box if something to pay
                    if (!Utils.modernUSFM() && total < 0)
                    {
                        if (caravanOverSOP == null)
                        {
                            Messages.Message("MFM_MsgToPayBillNeedCaravanOnSiteOfPayment".Translate(),
                                MessageTypeDefOf.NegativeEvent);
                            return;
                        }
                    }

                    //incRentedMercsSkillXP();
                    //Planning return to the player of its hired mercenaries, if applicable
                    processNotRenewedRentedMerc();

                    //If applicable, cancellation of orders in progress
                    processCancelCommands();

                    //Reset quadrum mood counters
                    resetMercMoodCounter();

                    processNotRenewedMerc();

                    processSilver(backMapStuffAndGuarantee, total);

                    if (!Utils.modernUSFM())
                    {
                        Utils.clearAllMedievalSiteOfPayment();
                    }

                    //Deletion of promo code if applicable
                    if (total < 0 && Utils.GCMFM.getDiscount(cl.discount, out cdiscount))
                    {
                        Utils.GCMFM.removeDiscount(cl.discount);
                    }

                    cl.wanted = new Dictionary<Pawn, bool>();
                    cl.rented = new Dictionary<Pawn, bool>();

                    //Closing the letter
                    Find.LetterStack.RemoveLetter(cl);

                    if (!Utils.modernUSFM())
                    {
                        removeSOP();
                    }

                    Find.WindowStack.TryRemove(this);
                }
                else
                {
                    if (!mapHasEnoughtSilver)
                    {
                        Messages.Message(
                            Utils.modernUSFM()
                                ? "NeedSilverLaunchable".Translate(Math.Abs(total).ToString())
                                : "MFM_NeedSilverInCaravanPayBill".Translate(Math.Abs(total).ToString()),
                            MessageTypeDefOf.NegativeEvent);
                    }
                }
            }

            GUI.color = Color.red;
            if (Widgets.ButtonText(new Rect(395f, 630f, 395f, 30f), "GoBack".Translate()))
            {
                var self = this;
                //If cancellation is past timeout => its equivalents to nothing to accept
                if (Find.TickManager.TicksGame + 1 >= cl.disappearAtTick)
                {
                    Find.WindowStack.Add(new Dialog_Msg("MFM_DialogConfirmCancelBill".Translate(),
                        "MFM_DialogConfirmCancelBillDesc".Translate(Utils.getUSFMLabel()), delegate
                        {
                            foreach (var m in cl.rented.ToList())
                            {
                                cl.rented[m.Key] = false;
                            }

                            foreach (var m in cl.wanted.ToList())
                            {
                                cl.wanted[m.Key] = false;
                            }

                            foreach (var m in cl.pendingMercOrders.ToList())
                            {
                                cl.pendingMercOrders[m.Key] = false;
                            }

                            //incRentedMercsSkillXP();
                            //Reset quadrum mood counters
                            resetMercMoodCounter();
                            processNotRenewedRentedMerc();
                            //If applicable, cancellation of orders in progress
                            processCancelCommands();
                            processNotRenewedRentedMerc();
                            processNotRenewedMerc();

                            if (!Utils.modernUSFM())
                            {
                                Utils.clearAllMedievalSiteOfPayment();
                            }

                            cl.wanted = new Dictionary<Pawn, bool>();
                            cl.rented = new Dictionary<Pawn, bool>();

                            Find.LetterStack.RemoveLetter(cl);

                            if (!Utils.modernUSFM())
                            {
                                removeSOP();
                            }

                            Find.WindowStack.TryRemove(self);
                            //processSilver(backMapStuffAndGuarantee, total);
                        }));
                }
                else
                {
                    Find.WindowStack.TryRemove(this);
                }
            }
        }

        GUI.color = Color.white;
    }

    private static void removeSOP()
    {
        Utils.GCMFM.BillInProgress = false;
        Utils.clearAllMedievalSiteOfPayment();
    }


    private void processCancelCommands()
    {
        foreach (var entry in cl.pendingMercOrders.ToList())
        {
            //We asked to withdraw the mentioned mercenary order
            if (!entry.Value)
            {
                Utils.GCMFM.removePendingMercOrder(entry.Key);
            }
        }
    }

    private void processNotRenewedRentedMerc()
    {
        var lst = new List<Pawn>();
        foreach (var entry in cl.rented)
        {
            if (!entry.Value)
            {
                lst.Add(entry.Key);
            }
        }

        if (lst.Count != 0)
        {
            Utils.GCMFM.addPendingRendMercComeBack(Utils.getDeliveryGT(cl.delivery), lst, backMap);
        }
    }

    private void processNotRenewedMerc()
    {
        //Collection and despawn of mercenaries not renewed for restitution of deposit and stuff later
        var lstNotRenewedMerc = new List<Pawn>();
        foreach (var entry in cl.wanted)
        {
            if (entry.Value)
            {
                continue;
            }

            lstNotRenewedMerc.Add(entry.Key);
            if (entry.Key.Spawned)
            {
                entry.Key.DeSpawn();
            }
            else
            {
                if (Find.WorldPawns.AllPawnsAlive.Contains(entry.Key))
                {
                    foreach (var caravan in Find.WorldObjects.Caravans)
                    {
                        if (!caravan.ContainsPawn(entry.Key))
                        {
                            continue;
                        }

                        caravan.RemovePawn(entry.Key);
                        caravan.Notify_PawnRemoved(entry.Key);
                        break;
                    }

                    Find.WorldPawns.RemovePawn(entry.Key);
                }
            }

            Utils.GCMFM.pushRentedPawn(entry.Key);
        }

        List<Caravan> toDel = null;

        //Check if the caravan is empty, if necessary, we delete it
        foreach (var caravan in Find.WorldObjects.Caravans)
        {
            if (caravan.pawns.Count != 0)
            {
                continue;
            }

            toDel ??= [];

            toDel.Add(caravan);
        }

        if (toDel != null)
        {
            foreach (var c in toDel)
            {
                Find.WorldObjects.Remove(c);
            }
        }

        //Settler bar acquisition
        Find.ColonistBar.MarkColonistsDirty();

        if (lstNotRenewedMerc.Count == 0)
        {
            return;
        }

        var GT = Find.TickManager.TicksGame + (Rand.Range(Settings.minHourDeliverStuffAndGuarantee,
            Settings.maxHourDeliverStuffAndGuarantee + 1) * 2500);
        Utils.GCMFM.addPendingStuffAndGuarantee(GT, lstNotRenewedMerc, backMapStuffAndGuarantee);
    }

    private void resetMercMoodCounter()
    {
        foreach (var entry in cl.wanted)
        {
            var comp = entry.Key.TryGetComp<Comp_USFM>();

            //Minimum 5 days to consider the quadrum as counting in nbQUad
            if (comp.quadNbHour >= 120)
            {
                comp.nbQuad++;
            }

            if (comp.quadNbHourMoodBad >= Settings.badMoodNbhPerQuadFloor)
            {
                comp.nbQuadUnHappy++;
            }
            else if (comp.quadNbHourMoodOK >= Settings.goodMoodNbhPerQuadFloor)
            {
                comp.nbQuadHappy++;
            }

            comp.quadNbHourMoodBad = 0;
            comp.quadNbHourMoodOK = 0;
            comp.quadNbHour = 0;
        }
    }

    private void processSilver(Map map, int price)
    {
        //Cash withdrawal
        if (price < 0)
        {
            if (!Utils.modernUSFM())
            {
                Utils.caravanPayCost(caravanOverSOP, price);
            }
            else
            {
                TradeUtility.LaunchSilver(map, Math.Abs(price));
            }
        }
        //Money return
        else if (price > 0)
        {
            if (!Utils.modernUSFM())
            {
                var wanted = new Dictionary<string, int>
                {
                    ["map"] = Utils.getRandomMapOfPlayer().uniqueID,
                    ["silver"] = price
                };
                Utils.GCMFM.addPendingMedievalCaravan(0, wanted,
                    "MFM_LetterRentedMercPaidDesc".Translate(price, Utils.getUSFMLabel()));
            }
            else
            {
                var thing = ThingMaker.MakeThing(ThingDefOf.Silver);
                thing.stackCount = price;

                var dropSpot = DropCellFinder.TradeDropSpot(map);
                if (!dropSpot.IsValid)
                {
                    dropSpot = DropCellFinder.FindRaidDropCenterDistant(map);
                }

                Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercPaid".Translate(Utils.getUSFMLabel()),
                    "MFM_LetterRentedMercPaidDesc".Translate(price, Utils.getUSFMLabel()), LetterDefOf.PositiveEvent,
                    new LookTargets(dropSpot, map));
                TradeUtility.SpawnDropPod(dropSpot, map, thing);
            }
        }
    }
}