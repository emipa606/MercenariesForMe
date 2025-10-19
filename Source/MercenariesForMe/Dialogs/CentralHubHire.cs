using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

public class CentralHubHire : Window
{
    private readonly Caravan caravan;
    private readonly Map map;

    private readonly List<ThingDef> racesAlreadyAdded = [];
    private readonly Map selMap;
    private readonly ITrader trader;
    private readonly Dictionary<string, int> wanted = new();
    private string codeDiscount = "";
    protected string curName;
    private int delivery = 3;
    private int gear;
    private int money;

    private Vector2 scrollPosition = Vector2.zero;
    private int weapon;

    public CentralHubHire(Pawn actor, Map map, Caravan caravan, ITrader trader = null)
    {
        this.trader = trader;
        this.caravan = caravan;
        this.map = map;
        forcePause = true;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
        closeOnClickedOutside = true;

        Utils.GCMFM.copyStockStructure(wanted);
        wanted["gearColor"] = (int)Colors.Black;
        if (map == null)
        {
            if (Utils.modernUSFM())
            {
                map = Utils.getRandomMapOfPlayer();
            }
            else
            {
                map = actor is { Map: not null } ? actor.Map : Utils.getRandomMapOfPlayer();
            }
        }

        selMap = map;
        if (map != null)
        {
            wanted["map"] = selMap.uniqueID;
        }

        if (Utils.GCMFM.preferedRace != "")
        {
            wanted[$"race_{Utils.GCMFM.preferedRace}"] = 1;
        }
    }

    public override Vector2 InitialSize => new(820f, 700f);

    public override void PostOpen()
    {
        base.PostOpen();
        CacheMoney();
    }

    public override void DoWindowContents(Rect inRect)
    {
        inRect.yMin += 15f;
        inRect.yMax -= 15f;
        var defaultColumnWidth = inRect.width - 20;
        var list = new Listing_Standard { ColumnWidth = defaultColumnWidth };

        //Image logo
        Widgets.ButtonImage(new Rect(0, 0, 800, 170),
            Utils.modernUSFM() ? Tex.centralHubHire : Tex.medievalCentralHubHire, Color.white, Color.white);

        var outRect = new Rect(inRect.x, inRect.y + 180, inRect.width, inRect.height - 200);
        var scrollRect = new Rect(0f, 180f, inRect.width - 16f, inRect.height * 7f);
        outRect.height -= 60;

        Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect);
        list.Begin(scrollRect);

        //Preference of mercenaries section
        list.ButtonImage(Tex.catPreferences, 820, 50);
        list.Gap(6);

        string curRace = "MFM_RandomRace".Translate();
        var curRaceDefName = Utils.getFirstKeyStartingWith(wanted, "race_");

        if (curRaceDefName != null)
        {
            curRaceDefName = curRaceDefName.ReplaceFirst("race_", "");
            var tmp = DefDatabase<PawnKindDef>.GetNamed(curRaceDefName);
            if (tmp != null)
            {
                curRace = tmp.LabelCap;
            }
        }

        if (list.ButtonText(curRace))
        {
            showRaceList();
        }

        //Option to buy the mercenaries
        var buy = false;
        if (wanted.TryGetValue("buy", out var value))
        {
            buy = value == 1;
        }

        list.CheckboxLabeled("MFM_BuyMerc".Translate(), ref buy);

        if (buy)
        {
            wanted["buy"] = 1;
        }
        else
        {
            wanted["buy"] = 0;
        }

        var wantBuy = wanted.ContainsKey("buy") && wanted["buy"] == 1;

        //Shipping section
        list.Gap(6);
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
                "MFM_StandardDelivery".Translate(Settings.transportStandardMinHour, Settings.transportStandardMaxHour),
                delivery == 3))
        {
            delivery = 3;
        }

        var mapFromUid = Utils.getMapFromUID(wanted["map"]);
        string mapText;
        if (mapFromUid == Find.CurrentMap)
        {
            mapText = "MFM_ThisCurrentMap".Translate(mapFromUid.GetUniqueLoadID());
        }
        else
        {
            mapText = mapFromUid.GetUniqueLoadID();
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

                if (m != mapFromUid)
                {
                    opts.Add(new FloatMenuOption(m.GetUniqueLoadID(), delegate { wanted["map"] = m.uniqueID; }));
                }
            }

            if (opts.Count != 0)
            {
                var floatMenuMap = new FloatMenu(opts);
                Find.WindowStack.Add(floatMenuMap);
            }
        }

        //Gears section
        list.Gap(6);
        list.ButtonImage(Tex.catGears, 820, 50);
        list.Gap(6);

        var compMedieval = "";
        if (!Utils.modernUSFM())
        {
            compMedieval = "Medieval";
        }

        if (list.RadioButton(("MFM_Gear3" + compMedieval).Translate(Settings.priceGear3), gear == 3))
        {
            gear = 3;
        }

        if (list.RadioButton(("MFM_Gear2" + compMedieval).Translate(Settings.priceGear2), gear == 2))
        {
            gear = 2;
        }

        if (list.RadioButton(("MFM_Gear1" + compMedieval).Translate(Settings.priceGear1), gear == 1))
        {
            gear = 1;
        }

        if (list.RadioButton(("MFM_Gear0" + compMedieval).Translate(), gear == 0))
        {
            gear = 0;
        }

        //Addition of the GEAR color selector
        if (list.ButtonText(Utils.getColorText(wanted["gearColor"])))
        {
            var opts = new List<FloatMenuOption>
            {
                new(Utils.getColorText((int)Colors.Black),
                    delegate { wanted["gearColor"] = (int)Colors.Black; }),
                new(Utils.getColorText((int)Colors.White),
                    delegate { wanted["gearColor"] = (int)Colors.White; }),
                new(Utils.getColorText((int)Colors.Gray),
                    delegate { wanted["gearColor"] = (int)Colors.Gray; }),
                new(Utils.getColorText((int)Colors.Blue),
                    delegate { wanted["gearColor"] = (int)Colors.Blue; }),
                new(Utils.getColorText((int)Colors.Cyan),
                    delegate { wanted["gearColor"] = (int)Colors.Cyan; }),
                new(Utils.getColorText((int)Colors.Purple),
                    delegate { wanted["gearColor"] = (int)Colors.Purple; }),
                new(Utils.getColorText((int)Colors.Pink),
                    delegate { wanted["gearColor"] = (int)Colors.Pink; }),
                new(Utils.getColorText((int)Colors.Green),
                    delegate { wanted["gearColor"] = (int)Colors.Green; }),
                new(Utils.getColorText((int)Colors.Red),
                    delegate { wanted["gearColor"] = (int)Colors.Red; }),
                new(Utils.getColorText((int)Colors.Orange),
                    delegate { wanted["gearColor"] = (int)Colors.Orange; }),
                new(Utils.getColorText((int)Colors.Yellow),
                    delegate { wanted["gearColor"] = (int)Colors.Yellow; })
            };

            var floatMenuMap = new FloatMenu(opts);
            Find.WindowStack.Add(floatMenuMap);
        }

        //Weapons section
        list.Gap(6);
        list.ButtonImage(Tex.catWeapons, 820, 50);
        list.Gap(6);

        if (Utils.modernUSFM() && list.RadioButton("MFM_Weapon4".Translate(Settings.priceWeapon4), weapon == 4))
        {
            weapon = 4;
        }

        if (list.RadioButton(("MFM_Weapon3" + compMedieval).Translate(Settings.priceWeapon3), weapon == 3))
        {
            weapon = 3;
        }

        if (list.RadioButton(("MFM_Weapon2" + compMedieval).Translate(Settings.priceWeapon2), weapon == 2))
        {
            weapon = 2;
        }

        if (list.RadioButton(("MFM_Weapon1" + compMedieval).Translate(Settings.priceWeapon1), weapon == 1))
        {
            weapon = 1;
        }

        if (list.RadioButton("MFM_Weapon0".Translate(), weapon == 0))
        {
            weapon = 0;
        }

        var salary = 0;
        var guarantee = 0;
        var shipping = 0;
        var nbMerc = 0;
        var discountVisual = 0;
        int immediateCost;

        //Display by mercenary type
        foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
        {
            var index1 = GC_MFM.buildStockIndex(type, MercenaryLevel.Recruit);
            var index2 = GC_MFM.buildStockIndex(type, MercenaryLevel.Confirmed);
            var index3 = GC_MFM.buildStockIndex(type, MercenaryLevel.Veteran);
            var index4 = GC_MFM.buildStockIndex(type, MercenaryLevel.Elite);
            var index5 = GC_MFM.buildStockIndex(type, MercenaryLevel.Cyborg);

            wanted.TryAdd(index1, 0);

            wanted.TryAdd(index2, 0);

            wanted.TryAdd(index3, 0);

            wanted.TryAdd(index4, 0);

            wanted.TryAdd(index5, 0);

            list.Gap(6);
            list.ButtonImage(Utils.getMercenaryCategoryCover(type), 820, 50);
            list.Gap(6);

            list.Label("MFM_WantedRecruit".Translate(wanted[index1], Utils.GCMFM.getNbInStock(index1)));
            wanted[index1] = (int)list.Slider(wanted[index1], 0, Utils.GCMFM.getNbInStock(index1));

            list.Label("MFM_WantedConfirmed".Translate(wanted[index2], Utils.GCMFM.getNbInStock(index2)));
            wanted[index2] = (int)list.Slider(wanted[index2], 0, Utils.GCMFM.getNbInStock(index2));

            list.Label("MFM_WantedVeteran".Translate(wanted[index3], Utils.GCMFM.getNbInStock(index3)));
            wanted[index3] = (int)list.Slider(wanted[index3], 0, Utils.GCMFM.getNbInStock(index3));

            list.Label("MFM_WantedElite".Translate(wanted[index4], Utils.GCMFM.getNbInStock(index4)));
            wanted[index4] = (int)list.Slider(wanted[index4], 0, Utils.GCMFM.getNbInStock(index4));

            if (Utils.modernUSFM())
            {
                list.Label("MFM_WantedCyborg".Translate(wanted[index5], Utils.GCMFM.getNbInStock(index5)));
                wanted[index5] = (int)list.Slider(wanted[index5], 0, Utils.GCMFM.getNbInStock(index5));
            }

            var multiplier = 1;
            if (wantBuy)
            {
                multiplier = Settings.buyMercSalaryMultiplier;
            }

            if (wanted[index1] != 0)
            {
                salary += wanted[index1] * Settings.priceRecruit * multiplier;
                guarantee += wanted[index1] * Settings.guaranteeRecruit;
            }

            if (wanted[index2] != 0)
            {
                salary += wanted[index2] * Settings.priceConfirmed * multiplier;
                guarantee += wanted[index2] * Settings.guaranteeConfirmed;
            }

            if (wanted[index3] != 0)
            {
                salary += wanted[index3] * Settings.priceVeteran * multiplier;
                guarantee += wanted[index3] * Settings.guaranteeVeteran;
            }

            if (wanted[index4] != 0)
            {
                salary += wanted[index4] * Settings.priceElite * multiplier;
                guarantee += wanted[index4] * Settings.guaranteeElite;
            }

            if (Utils.modernUSFM() && wanted[index5] != 0)
            {
                salary += wanted[index5] * Settings.priceCyborg * multiplier;
                guarantee += wanted[index5] * Settings.guaranteeCyborg;
            }

            nbMerc += wanted[index1] + wanted[index2] + wanted[index3] + wanted[index4] + wanted[index5];
        }

        if (delivery == 2)
        {
            shipping = nbMerc * Settings.transportQuickPrice;
        }
        else if (delivery == 1)
        {
            shipping = nbMerc * Settings.transportImmediatePrice;
        }

        //Calculation of non-consumed days
        var salaryNonConsumed = (int)(salary *
                                      (GenDate.DayOfQuadrum(Find.TickManager.TicksAbs,
                                          Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile).x) / (float)15));

        //Material cost calculation
        var gearCostUnit = 0;
        if (gear == 3)
        {
            gearCostUnit = Settings.priceGear3;
        }
        else if (gear == 2)
        {
            gearCostUnit = Settings.priceGear2;
        }
        else if (gear == 1)
        {
            gearCostUnit = Settings.priceGear1;
        }

        var weaponCostUnit = 0;
        if (weapon == 4)
        {
            weaponCostUnit = Settings.priceWeapon4;
        }
        else if (weapon == 3)
        {
            weaponCostUnit = Settings.priceWeapon3;
        }
        else if (weapon == 2)
        {
            weaponCostUnit = Settings.priceWeapon2;
        }
        else if (weapon == 1)
        {
            weaponCostUnit = Settings.priceWeapon1;
        }

        var stuffCost = (nbMerc * gearCostUnit) + (weaponCostUnit * nbMerc);

        list.End();
        Widgets.EndScrollView();

        if (!wantBuy)
        {
            immediateCost = salary - salaryNonConsumed + guarantee + shipping + stuffCost;
        }
        else
        {
            immediateCost = salary + shipping + stuffCost;
        }

        if (Utils.GCMFM.getDiscount(codeDiscount, out var discount))
        {
            discountVisual = (int)(discount * 100);
        }

        if (discount != 0)
        {
            immediateCost -= (int)Math.Abs(immediateCost * discount);
        }


        GUI.color = Color.cyan;
        if (!wantBuy)
        {
            Widgets.Label(new Rect(0f, 580, 820, 30), "MFM_Guarantee".Translate(guarantee));
        }

        Widgets.Label(new Rect(200f, 580, 820, 30), "MFM_Shipping".Translate(shipping));
        GUI.color = Color.yellow;
        if (!wantBuy)
        {
            Widgets.Label(new Rect(400f, 580, 820, 30), "MFM_Prorata".Translate($"-{salaryNonConsumed}"));
        }

        Widgets.Label(new Rect(600f, 580, 820, 30), "MFM_Discount".Translate(discountVisual));
        GUI.color = Color.cyan;

        Widgets.Label(new Rect(0f, 600, 820, 30), "MFM_StuffCost".Translate(stuffCost));
        Widgets.Label(new Rect(200f, 600, 820, 30), "MFM_ImmediateCost".Translate(immediateCost));
        GUI.color = Color.green;
        if (!wantBuy)
        {
            Widgets.Label(new Rect(400f, 600, 820, 30), "MFM_CostPerMonth".Translate(salary));
        }

        GUI.color = Color.white;

        var discountTxt = "-";
        if (!string.IsNullOrEmpty(codeDiscount))
        {
            discountTxt = codeDiscount;
        }

        if (Widgets.ButtonText(new Rect(600f, 600f, 180f, 30f), discountTxt))
        {
            Utils.showDiscountCodeList(delegate(string code) { codeDiscount = code; });
        }

        //codeDiscount = Widgets.TextField(new Rect(600f, 600f, 180f, 30f), codeDiscount);
        //Widgets.Label(new Rect(600f, 600, 820, 30), "MFM_Count".Translate(nbMerc));
        GUI.color = Color.green;

        var mapHasEnoughtSilver = money >= Math.Abs(immediateCost);

        if (nbMerc == 0 || immediateCost > 0 && !mapHasEnoughtSilver)
        {
            GUI.color = Color.gray;
        }
        else
        {
            GUI.color = Color.green;
        }

        if (Widgets.ButtonText(new Rect(0f, 630f, 390f, 30f), $"{"OK".Translate()} ({immediateCost}/{money})"))
        {
            if (nbMerc != 0 && mapHasEnoughtSilver)
            {
                //Parameter storage linked to gears and weapon
                wanted["gear"] = gear;
                wanted["weapon"] = weapon;

                if (caravan == null)
                {
                    //Payment sending
                    if (Utils.modernUSFM())
                    {
                        //The orbital beacon
                        TradeUtility.LaunchSilver(selMap, Math.Abs(immediateCost));
                    }
                    else
                    {
                        //Via money transfer on local map to physical trader
                        Utils.payLocalSilver(Math.Abs(immediateCost), selMap, trader);
                    }
                }
                else
                {
                    Utils.caravanPayCost(caravan, Math.Abs(immediateCost));
                }

                //Delayed delivery
                Utils.GCMFM.addPendingMercOrder(Utils.getDeliveryGT(delivery), wanted);

                //Deletion of promo code if applicable
                if (Utils.GCMFM.getDiscount(codeDiscount, out discount))
                {
                    Utils.GCMFM.removeDiscount(codeDiscount);
                }

                Find.WindowStack.TryRemove(this);
            }
            else
            {
                if (nbMerc == 0)
                {
                    Messages.Message("MFM_MsgNeedSelectMercToContinue".Translate(), MessageTypeDefOf.NegativeEvent);
                }
                else if (!mapHasEnoughtSilver)
                {
                    Messages.Message(
                        Utils.modernUSFM()
                            ? "NeedSilverLaunchable".Translate(Math.Abs(immediateCost).ToString())
                            : "NotEnoughSilver".Translate(),
                        MessageTypeDefOf.NegativeEvent);
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

    private void showRaceList()
    {
        racesAlreadyAdded.Clear();
        var opts = new List<FloatMenuOption>();

        foreach (var pkd in DefDatabase<PawnKindDef>.AllDefs)
        {
            if (pkd.race == null
                || racesAlreadyAdded.Contains(pkd.race)
                || !pkd.RaceProps.Humanlike
                || Settings.blacklistedPawnKind.Contains(pkd.defName))
            {
                continue;
            }

            racesAlreadyAdded.Add(pkd.race);
            opts.Add(new FloatMenuOption(pkd.race.LabelCap, delegate
            {
                Utils.removeKeysStartingWith(wanted, "race_");
                wanted[$"race_{pkd.defName}"] = 1;
                Utils.GCMFM.preferedRace = pkd.defName;
            }));
        }

        if (opts.Count == 0)
        {
            return;
        }

        opts.Add(new FloatMenuOption("MFM_RandomRace".Translate(), delegate
        {
            Utils.removeKeysStartingWith(wanted, "race_");
            Utils.GCMFM.preferedRace = "";
        }));

        opts.SortBy(p => p.Label);
        var floatMenuMap = new FloatMenu(opts);
        Find.WindowStack.Add(floatMenuMap);
    }

    private void CacheMoney()
    {
        if (caravan != null)
        {
            money = Utils.moneyInCaravan(caravan);
            return;
        }

        if (selMap == null)
        {
            Log.Warning("Current map is null");
            Close();
            return;
        }

        if (Utils.modernUSFM())
        {
            money = (from el in TradeUtility.AllLaunchableThingsForTrade(selMap)
                where el.def == ThingDefOf.Silver
                select el).Sum(t => t.stackCount);
            return;
        }

        if (trader == null)
        {
            Log.Warning("Current trader is null");
            Close();
            return;
        }

        money = (from t in Utils.AllLocalSilverForTrade(map)
            where t.def == ThingDefOf.Silver
            select t).Sum(t => t.stackCount);
    }
}