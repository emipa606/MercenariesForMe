using UnityEngine;
using Verse;
using RimWorld;
using System;
using System.Collections.Generic;

namespace aRandomKiwi.MFM
{
    public class Settings : ModSettings
    {
        public static bool imprisonedMercenariesAreNoLongerMerc = true;
        public static string lastVersionInfo = "";
        public static bool allowNonZeroOtherSkillsMerc = false;
        public static int buyMercSalaryMultiplier = 8;
        public static float rateDecreaseAppliedToRentedMercSalary = 0.45f;
        public static bool preventNonMercenariesPawnSpawnWithUSFMStuff = false;
        public static List<string> mustHaveTraits = new List<string> { };
        public static float percentIncomeDecreaseSlave = 0.30f;
        public static int currentEpoch = 0;
        public static int medievalModeMinHourDeliveryCaravan = 8;
        public static int medievalModeMaxHourDeliveryCaravan = 48;
        public static bool allowNonViolentToBeRented = true;
        public static List<string> blacklistedWeapons = new List<string> { };
        public static List<string> hediffToNotClear = new List<string> { "DummyPrivates", "DummyPrivatesForModdedPawnsOnBirthday" };
        public static List<string> blacklistedPawnKind = new List<string> { "M7MechPawn", "Android5Tier" };
        public static int minXPRentedMercs = 600;
        public static int maxXPRentedMercs = 3000;
        public static int maxAge = 80;
        public static int minAge = 18;
        public static float riskCyborgRemovingPartExplode = 0.75f;
        public static int cyborgBionicModSource = -1;
        static public List<string> blacklistedTraitsV2 = new List<string> { "wimp", "pyromaniac", "abrasive", "nervous", "volatile", "psychically hypersensitive", "psychically sensitive", "pessimist", "depressive" };
        public static int minTimeUSFMSuspended = 480000;
        public static int maxTimeUSFMSuspended = 1200000;
        public static bool onlyVanillaForWeapon = false;
        public static bool onlyVanillaForGear = false;
        //public static bool onlyVanillaForCyborg = false;
        public static bool hideMercenariesIcon = false;
        public static bool hideMercenariesLevel = false;
        public static bool disableNotifWhenDisusedMercDie = false;
        //Guarantee in the event of death or damage to the mercenary
        public static int guaranteeRecruit = 400;
        public static int guaranteeConfirmed = 750;
        public static int guaranteeVeteran = 1000;
        public static int guaranteeElite = 1400;
        public static int guaranteeCyborg = 2500;

        //Monthly price of mercenaries according to type
        public static int priceRecruit = 400;
        public static int priceConfirmed = 750;
        public static int priceVeteran = 1000;
        public static int priceElite = 1400;
        public static int priceCyborg = 2500;

        //Gears price
        public static int priceGear1 = 745;
        public static int priceGear2 = 1200;
        public static int priceGear3 = 2700;

        //Weapon prices
        public static int priceWeapon1 = 250;
        public static int priceWeapon2 = 600;
        public static int priceWeapon3 = 900;
        public static int priceWeapon4 = 1600;

        //Quantity of mercenaries in stock ===> Melee
        public static int minRecruitMelee = 15;
        public static int maxRecruitMelee = 100;
        public static int minConfirmedMelee = 5;
        public static int maxConfirmedMelee = 75;
        public static int minVeteranMelee = 1;
        public static int maxVeteranMelee = 50;
        public static int minEliteMelee = 0;
        public static int maxEliteMelee = 25;
        public static int minCyborgMelee = 0;
        public static int maxCyborgMelee = 10;
        //Quantity of mercenaries in stock ===> Ranged
        public static int minRecruitRanged = 15;
        public static int maxRecruitRanged = 100;
        public static int minConfirmedRanged = 5;
        public static int maxConfirmedRanged = 75;
        public static int minVeteranRanged = 1;
        public static int maxVeteranRanged = 50;
        public static int minEliteRanged = 0;
        public static int maxEliteRanged = 25;
        public static int minCyborgRanged = 0;
        public static int maxCyborgRanged = 10;
        //QUANTITIES of mercenaries in stock ===> Civilian
        public static int minRecruit = 10;
        public static int maxRecruit = 45;
        public static int minConfirmed = 5;
        public static int maxConfirmed = 25;
        public static int minVeteran = 1;
        public static int maxVeteran = 15;
        public static int minElite = 0;
        public static int maxElite = 8;
        public static int minCyborg = 0;
        public static int maxCyborg = 10;

        public static int transportStandardMinHour = 24;
        public static int transportStandardMaxHour = 96;
        public static int transportQuickMinHour = 12;
        public static int transportQuickMaxHour = 24;
        public static int transportQuickPrice = 250;
        public static int transportImmediatePrice = 500;

        public static int badMoodNbhPerQuadFloor = 72;
        public static int goodMoodNbhPerQuadFloor = 96;
        public static float badMoodCoefIncCost = 0.30f;
        public static float goodMoodCoefDecCost = 0.25f;

        public static int minHourDeliverStuffAndGuarantee = 2;
        public static int maxHourDeliverStuffAndGuarantee = 9;

        public static float minDiscount = 0.15f;
        public static float maxDiscount = 0.55f;

        public static int powerBeamCost = 9500;
        public static bool enableIncidentRaid = true;
        public static bool enableIncidentCounterOffer = true;
        public static float minRateIncSalaryIncidentCounterOffer = 1.0f;
        public static float maxRateIncSalaryIncidentCounterOffer = 3.0f;

        //Quantity of mercenaries that can die per quadrum
        public static float maxPercentDisusedMercCanDiePerQuad = 0.10f;
        public static int timeBeforeRenewOffers = 1800000;

        public static int mercJoinNbSalaryToPay = 3;

        public static Vector2 scrollPosition = Vector2.zero;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            inRect.yMin += 15f;
            inRect.yMax -= 15f;

            var defaultColumnWidth = (inRect.width - 50);
            Listing_Standard list = new Listing_Standard() { ColumnWidth = defaultColumnWidth };


            var outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            var scrollRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height * 10f + blacklistedTraitsV2.Count * 55 + blacklistedPawnKind.Count * 55 + hediffToNotClear.Count * 55 + blacklistedWeapons.Count * 55 + mustHaveTraits.Count * 55  + 800);
            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect, true);

            list.Begin(scrollRect);

            list.GapLine();
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionGeneral".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();


            list.CheckboxLabeled("MFM_SettingsImprisonedMercenariesAreNoLongerMerc".Translate(), ref imprisonedMercenariesAreNoLongerMerc);

            string buffMinXPRentedMercs = minXPRentedMercs.ToString();
            string buffMaxXPRentedMercs = maxXPRentedMercs.ToString();
            string buffPowerBeamCost = powerBeamCost.ToString();

            list.CheckboxLabeled("MFM_SettingsAllowMercNonZeroOtherSkills".Translate(), ref allowNonZeroOtherSkillsMerc);

            bool prevPreventNonMercenariesPawnSpawnWithUSFMStuff = preventNonMercenariesPawnSpawnWithUSFMStuff;
            list.CheckboxLabeled("MFM_SettingsPreventNonMercSpawnWithUSFMStuff".Translate(), ref preventNonMercenariesPawnSpawnWithUSFMStuff);
            if(preventNonMercenariesPawnSpawnWithUSFMStuff != prevPreventNonMercenariesPawnSpawnWithUSFMStuff)
            {
                Utils.refreshUSFMStuffRule();
            }

            list.Label("MFM_SettingsCurrentEpoch".Translate()+" : ");
            if (list.RadioButton("MFM_SettingsCurrentEpochAuto".Translate(), (currentEpoch == 0)))
                currentEpoch = 0;
            if (list.RadioButton("MFM_SettingsCurrentEpochMedieval".Translate(), (currentEpoch == 1)))
                currentEpoch = 1;
            if (list.RadioButton("MFM_SettingsCurrentEpochModern".Translate(), (currentEpoch == 2)))
                currentEpoch = 2;
            GUI.color = Color.red;
            if (list.RadioButton("MFM_SettingsCurrentEpochModernOnly".Translate(), (currentEpoch == 3)))
                currentEpoch = 3;
            GUI.color = Color.white;

            list.CheckboxLabeled("MFM_SettingsAllowNonViolentToBeRent".Translate(), ref allowNonViolentToBeRented);

            list.Label("MFM_SettingsMercSlaveIncomeDecrease".Translate((int)(percentIncomeDecreaseSlave*100) ));
            percentIncomeDecreaseSlave = list.Slider(percentIncomeDecreaseSlave, 0.01f, 0.95f);

            list.Label("MFM_SettingsMinXPGainPerDayRentedMerc".Translate());
            list.TextFieldNumeric(ref minXPRentedMercs, ref buffMinXPRentedMercs, 1, 999999);
            list.Label("MFM_SettingsMaxXPGainPerDayRentedMerc".Translate());
            list.TextFieldNumeric(ref maxXPRentedMercs, ref buffMaxXPRentedMercs, 1, 999999);

            if (maxXPRentedMercs < minXPRentedMercs)
                maxXPRentedMercs = minXPRentedMercs;

            list.Label("MFM_SettingsMinAge".Translate(minAge));
            minAge = (int) list.Slider(minAge, 10,100);
            list.Label("MFM_SettingsMaxAge".Translate(maxAge));
            maxAge = (int) list.Slider(maxAge, 10, 100f);

            list.Label("MFM_SettingsCyborgBionicSource".Translate());
            if (list.RadioButton("Auto", (cyborgBionicModSource == -1)))
                cyborgBionicModSource = -1;
            if (list.RadioButton("Vanilla", (cyborgBionicModSource == 0)))
                cyborgBionicModSource = 0;
            if (list.RadioButton("EPOE", (cyborgBionicModSource == 1)))
                cyborgBionicModSource = 1;
            if (list.RadioButton("RSBE", (cyborgBionicModSource == 2)))
                cyborgBionicModSource = 2;
            if (list.RadioButton("Cybernetic Organism and Neural Network", (cyborgBionicModSource == 3)))
                cyborgBionicModSource = 3;
            if (list.RadioButton("Evolved Organs", (cyborgBionicModSource == 4)))
                cyborgBionicModSource = 4;

            list.Label("MFM_SettingsRiskRemovedCyborgPartExplode".Translate((int)(riskCyborgRemovingPartExplode * 100)));
            riskCyborgRemovingPartExplode = list.Slider(riskCyborgRemovingPartExplode, 0.0f, 1.0f);

            list.Label("MFM_SettingsMercJoinNbSalary".Translate(mercJoinNbSalaryToPay));
            mercJoinNbSalaryToPay = (int)list.Slider(mercJoinNbSalaryToPay, 1, 20);

            //base prices
            list.Label("MFM_SettingsPowerBeamCost".Translate());
            list.TextFieldNumeric(ref powerBeamCost, ref buffPowerBeamCost, 1, 999999);

            list.Label("MFM_SettingsUSFMSuspendedMinTime".Translate(minTimeUSFMSuspended.ToStringTicksToPeriodVerbose()));
            minTimeUSFMSuspended = (int)list.Slider(minTimeUSFMSuspended, 2500, 3600000);
            list.Label("MFM_SettingsUSFMSuspendedMaxTime".Translate(maxTimeUSFMSuspended.ToStringTicksToPeriodVerbose()));
            maxTimeUSFMSuspended = (int)list.Slider(maxTimeUSFMSuspended, 2500, 3600000);
            if (maxTimeUSFMSuspended < minTimeUSFMSuspended)
                maxTimeUSFMSuspended = minTimeUSFMSuspended;

            list.CheckboxLabeled("MFM_SettingsUseOnlyVanillaForWeapon".Translate(), ref onlyVanillaForWeapon);
            list.CheckboxLabeled("MFM_SettingsUseOnlyVanillaForGear".Translate(), ref onlyVanillaForGear);
            list.CheckboxLabeled("MFM_SettingsHideIcon".Translate(), ref hideMercenariesIcon);
            list.CheckboxLabeled("MFM_SettingsHideLevel".Translate(), ref hideMercenariesLevel);
            list.CheckboxLabeled("MFM_SettingsDisableNotifWhenDisusedMercDie".Translate(), ref disableNotifWhenDisusedMercDie);
            list.Label("MFM_SettingsPercentDisusedMercCanDiePerQuad".Translate((int)(maxPercentDisusedMercCanDiePerQuad * 100)));
            maxPercentDisusedMercCanDiePerQuad = list.Slider(maxPercentDisusedMercCanDiePerQuad, 0.0f, 1.0f);

            list.Label("MFM_SettingsTimeBeforeRenewOffers".Translate(timeBeforeRenewOffers.ToStringTicksToPeriodVerbose()));
            timeBeforeRenewOffers = (int) list.Slider(timeBeforeRenewOffers, 0, 99999999);

            list.Label("MFM_SettingsBadMoodNbhPerQuadFloor".Translate(badMoodNbhPerQuadFloor));
            badMoodNbhPerQuadFloor = (int)list.Slider(badMoodNbhPerQuadFloor, 0, 360);
            list.Label("MFM_SettingsGoodMoodNbhPerQuadFloor".Translate(goodMoodNbhPerQuadFloor));
            goodMoodNbhPerQuadFloor = (int)list.Slider(goodMoodNbhPerQuadFloor, 0, 360);
            list.Label("MFM_SettingsBadMoodCoefIncCost".Translate( (int)(badMoodCoefIncCost*100) ));
            badMoodCoefIncCost = list.Slider(badMoodCoefIncCost, 0f, 1.0f);
            list.Label("MFM_SettingsBadMoodCoefIncCost".Translate((int)(goodMoodCoefDecCost * 100)));
            goodMoodCoefDecCost = list.Slider(goodMoodCoefDecCost, 0f, 1.0f);


            list.Gap();
            list.GapLine(10);
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionMedievalMode".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine(10);

            list.Label("MFM_SettingsMedievalModeDeliveryCaravanMinTime".Translate(medievalModeMinHourDeliveryCaravan));
            medievalModeMinHourDeliveryCaravan = (int)list.Slider(medievalModeMinHourDeliveryCaravan, 1, 150);
            list.Label("MFM_SettingsMedievalModeDeliveryCaravanMaxTime".Translate(medievalModeMaxHourDeliveryCaravan));
            medievalModeMaxHourDeliveryCaravan = (int)list.Slider(medievalModeMaxHourDeliveryCaravan, 1, 150);
            if (medievalModeMaxHourDeliveryCaravan < medievalModeMinHourDeliveryCaravan)
                medievalModeMaxHourDeliveryCaravan = medievalModeMinHourDeliveryCaravan;



            list.Gap();
            list.GapLine(10);
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionTransport".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine(10);

            list.Label("MFM_SettingsStuffAndGuaranteeDeliveryTimeMin".Translate(minHourDeliverStuffAndGuarantee));
            minHourDeliverStuffAndGuarantee = (int)list.Slider(minHourDeliverStuffAndGuarantee, 1, 150);
            list.Label("MFM_SettingsStuffAndGuaranteeDeliveryTimeMax".Translate(maxHourDeliverStuffAndGuarantee));
            maxHourDeliverStuffAndGuarantee = (int)list.Slider(maxHourDeliverStuffAndGuarantee, 1, 150);
            if (maxHourDeliverStuffAndGuarantee < minHourDeliverStuffAndGuarantee)
                maxHourDeliverStuffAndGuarantee = minHourDeliverStuffAndGuarantee;

            list.Label("MFM_SettingsTransportStandardMinHour".Translate(transportStandardMinHour));
            transportStandardMinHour = (int)list.Slider(transportStandardMinHour, 1, 1000);
            list.Label("MFM_SettingsTransportStandardMaxHour".Translate(transportStandardMaxHour));
            transportStandardMaxHour = (int)list.Slider(transportStandardMaxHour, 1, 1000);
            if (transportStandardMaxHour < transportStandardMinHour)
                transportStandardMaxHour = transportStandardMinHour;

            list.Label("MFM_SettingsTransportQuickMinHour".Translate(transportQuickMinHour));
            transportQuickMinHour = (int)list.Slider(transportQuickMinHour, 1, 1000);
            list.Label("MFM_SettingsTransportQuickMaxHour".Translate(transportQuickMaxHour));
            transportQuickMaxHour = (int)list.Slider(transportQuickMaxHour, 1, 1000);
            if (transportQuickMaxHour < transportQuickMinHour)
                transportQuickMaxHour = transportQuickMinHour;

            string buffTransportQuickPrice = transportQuickPrice.ToString();
            string buffTransportImmediatePrice = transportImmediatePrice.ToString();

            list.Label("MFM_SettingsTransportQuickPrice".Translate());
            list.TextFieldNumeric(ref transportQuickPrice, ref buffTransportQuickPrice, 1, 999999);
            list.Label("MFM_SettingsTransportImmediatePrice".Translate());
            list.TextFieldNumeric(ref transportImmediatePrice, ref buffTransportImmediatePrice, 1, 999999);


            list.Gap();
            list.GapLine(10);
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionMercPrice".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine(10);

            string buffPriceRecruit = priceRecruit.ToString();
            string buffPriceConfirmed = priceConfirmed.ToString();
            string buffPriceVeteran = priceVeteran.ToString();
            string buffPriceElite = priceElite.ToString();
            string buffPriceCyborg = priceCyborg.ToString();
            string buffGuaranteeRecruit = guaranteeRecruit.ToString();
            string buffGuaranteeConfirmed = guaranteeConfirmed.ToString();
            string buffGuaranteeVeteran = guaranteeVeteran.ToString();
            string buffGuaranteeElite = guaranteeElite.ToString();
            string buffGuaranteeCyborg = guaranteeCyborg.ToString();
            string buffPriceGear1 = priceGear1.ToString();
            string buffPriceGear2 = priceGear2.ToString();
            string buffPriceGear3 = priceGear3.ToString();
            string buffPriceWeapon1 = priceWeapon1.ToString();
            string buffPriceWeapon2 = priceWeapon2.ToString();
            string buffPriceWeapon3 = priceWeapon3.ToString();
            string buffPriceWeapon4 = priceWeapon4.ToString();


            list.Label("MFM_SettingsBuyMercSalaryMultiplier".Translate((buyMercSalaryMultiplier)));
            buyMercSalaryMultiplier = (int) list.Slider(buyMercSalaryMultiplier, 1, 30);

            list.Label("MFM_SettingsSalaryDecreaseRateAppliedToRentedMerc".Translate((int)(rateDecreaseAppliedToRentedMercSalary * 100)));
            rateDecreaseAppliedToRentedMercSalary = list.Slider(rateDecreaseAppliedToRentedMercSalary, 0.05f, 0.95f);

            list.Label("MFM_SettingsDiscountMin".Translate((int)(minDiscount * 100)));
            minDiscount = list.Slider(minDiscount, 0.01f, 0.90f);
            list.Label("MFM_SettingsDiscountMax".Translate((int)(maxDiscount * 100)));
            maxDiscount = list.Slider(maxDiscount, 0.01f, 0.90f);
            if (maxDiscount < minDiscount)
                maxDiscount = minDiscount;

            //base prices
            list.Label("MFM_SettingsPriceRecruit".Translate());
            list.TextFieldNumeric(ref priceRecruit, ref buffPriceRecruit, 1, 999999);
            list.Label("MFM_SettingsPriceConfirmed".Translate());
            list.TextFieldNumeric(ref priceConfirmed, ref buffPriceConfirmed, 1, 999999);
            list.Label("MFM_SettingsPriceVeteran".Translate());
            list.TextFieldNumeric(ref priceVeteran, ref buffPriceVeteran, 1, 999999);
            list.Label("MFM_SettingsPriceElite".Translate());
            list.TextFieldNumeric(ref priceElite, ref buffPriceElite, 1, 999999);
            list.Label("MFM_SettingsPriceCyborg".Translate());
            list.TextFieldNumeric(ref priceCyborg, ref buffPriceCyborg, 1, 999999);
            //Garantee
            list.Label("MFM_SettingsGuaranteeRecruit".Translate());
            list.TextFieldNumeric(ref guaranteeRecruit, ref buffGuaranteeRecruit, 1, 999999);
            list.Label("MFM_SettingsGuaranteeConfirmed".Translate());
            list.TextFieldNumeric(ref guaranteeConfirmed, ref buffGuaranteeConfirmed, 1, 999999);
            list.Label("MFM_SettingsGuaranteeVeteran".Translate());
            list.TextFieldNumeric(ref guaranteeVeteran, ref buffGuaranteeVeteran, 1, 999999);
            list.Label("MFM_SettingsGuaranteeElite".Translate());
            list.TextFieldNumeric(ref guaranteeElite, ref buffGuaranteeElite, 1, 999999);
            list.Label("MFM_SettingsGuaranteeCyborg".Translate());
            list.TextFieldNumeric(ref guaranteeCyborg, ref buffGuaranteeCyborg, 1, 999999);
            //Gears
            list.Label("MFM_SettingsPriceGear1".Translate());
            list.TextFieldNumeric(ref priceGear1, ref buffPriceGear1, 1, 999999);
            list.Label("MFM_SettingsPriceGear2".Translate());
            list.TextFieldNumeric(ref priceGear2, ref buffPriceGear2, 1, 999999);
            list.Label("MFM_SettingsPriceGear3".Translate());
            list.TextFieldNumeric(ref priceGear3, ref buffPriceGear3, 1, 999999);
            //Weapons
            list.Label("MFM_SettingsPriceWeapon1".Translate());
            list.TextFieldNumeric(ref priceWeapon1, ref buffPriceWeapon1, 1, 999999);
            list.Label("MFM_SettingsPriceWeapon2".Translate());
            list.TextFieldNumeric(ref priceWeapon2, ref buffPriceWeapon2, 1, 999999);
            list.Label("MFM_SettingsPriceWeapon3".Translate());
            list.TextFieldNumeric(ref priceWeapon3, ref buffPriceWeapon3, 1, 999999);
            list.Label("MFM_SettingsPriceWeapon4".Translate());
            list.TextFieldNumeric(ref priceWeapon4, ref buffPriceWeapon4, 1, 999999);

            list.Gap();
            list.Gap();
            list.GapLine(10);
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionNbMercenaries".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine(10);

            string buffMinRecruit = minRecruit.ToString();
            string buffMaxRecruit = maxRecruit.ToString();
            string buffMinConfirmed = minConfirmed.ToString();
            string buffMaxConfirmed = maxConfirmed.ToString();
            string buffMinVeteran = minVeteran.ToString();
            string buffMaxVeteran = maxVeteran.ToString();
            string buffMinElite = minElite.ToString();
            string buffMaxElite = maxElite.ToString();
            string buffMaxCyborg = maxCyborg.ToString();
            string buffMinCyborg = minCyborg.ToString();

            string buffMinRecruitMelee = minRecruitMelee.ToString();
            string buffMaxRecruitMelee = maxRecruitMelee.ToString();
            string buffMinConfirmedMelee = minConfirmedMelee.ToString();
            string buffMaxConfirmedMelee = maxConfirmedMelee.ToString();
            string buffMinVeteranMelee = minVeteranMelee.ToString();
            string buffMaxVeteranMelee = maxVeteranMelee.ToString();
            string buffMinEliteMelee = minEliteMelee.ToString();
            string buffMaxEliteMelee = maxEliteMelee.ToString();
            string buffMinCyborgMelee = minCyborgMelee.ToString();
            string buffMaxCyborgMelee = maxCyborgMelee.ToString();

            string buffMinRecruitRanged = minRecruitRanged.ToString();
            string buffMaxRecruitRanged = maxRecruitRanged.ToString();
            string buffMinConfirmedRanged = minConfirmedRanged.ToString();
            string buffMaxConfirmedRanged = maxConfirmedRanged.ToString();
            string buffMinVeteranRanged = minVeteranRanged.ToString();
            string buffMaxVeteranRanged = maxVeteranRanged.ToString();
            string buffMinEliteRanged = minEliteRanged.ToString();
            string buffMaxEliteRanged = maxEliteRanged.ToString();
            string buffMinCyborgRanged = minCyborgRanged.ToString();
            string buffMaxCyborgRanged = maxCyborgRanged.ToString();

            list.Label("MFM_SettingsMinRecruit".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref minRecruitMelee, ref buffMinRecruitMelee, 1, 999999);
            list.Label("MFM_SettingsMaxRecruit".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref maxRecruitMelee, ref buffMaxRecruitMelee, 1, 999999);
            if (maxRecruitMelee < minRecruitMelee)
                maxRecruitMelee = minRecruitMelee;
            list.Label("MFM_SettingsMinConfirmed".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref minConfirmedMelee, ref buffMinConfirmedMelee, 1, 999999);
            list.Label("MFM_SettingsMaxConfirmed".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref maxConfirmedMelee, ref buffMaxConfirmedMelee, 1, 999999);
            if (maxConfirmedMelee < minConfirmedMelee)
                maxConfirmedMelee = minConfirmedMelee;
            list.Label("MFM_SettingsMinVeteran".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref minVeteranMelee, ref buffMinVeteranMelee, 1, 999999);
            list.Label("MFM_SettingsMaxVeteran".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref maxVeteranMelee, ref buffMaxVeteranMelee, 1, 999999);
            if (maxVeteranMelee < minVeteranMelee)
                maxVeteranMelee = minVeteranMelee;
            list.Label("MFM_SettingsMinElite".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref minEliteMelee, ref buffMinEliteMelee, 1, 999999);
            list.Label("MFM_SettingsMaxElite".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref maxEliteMelee, ref buffMaxEliteMelee, 1, 999999);
            if (maxEliteMelee < minEliteMelee)
                maxEliteMelee = minEliteMelee;
            list.Label("MFM_SettingsMinCyborg".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref minCyborgMelee, ref buffMinCyborgMelee, 1, 999999);
            list.Label("MFM_SettingsMaxCyborg".Translate("MFM_Melee".Translate()));
            list.TextFieldNumeric(ref maxCyborgMelee, ref buffMaxCyborgMelee, 1, 999999);
            if (maxCyborgMelee < minCyborgMelee)
                maxCyborgMelee = minCyborgMelee;

            list.Gap();

            list.Label("MFM_SettingsMinRecruit".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref minRecruitRanged, ref buffMinRecruitRanged, 1, 999999);
            list.Label("MFM_SettingsMaxRecruit".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref maxRecruitRanged, ref buffMaxRecruitRanged, 1, 999999);
            if (maxRecruitRanged < minRecruitRanged)
                maxRecruitRanged = minRecruitRanged;
            list.Label("MFM_SettingsMinConfirmed".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref minConfirmedRanged, ref buffMinConfirmedRanged, 1, 999999);
            list.Label("MFM_SettingsMaxConfirmed".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref maxConfirmedRanged, ref buffMaxConfirmedRanged, 1, 999999);
            if (maxConfirmedRanged < minConfirmedRanged)
                maxConfirmedRanged = minConfirmedRanged;
            list.Label("MFM_SettingsMinVeteran".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref minVeteranRanged, ref buffMinVeteranRanged, 1, 999999);
            list.Label("MFM_SettingsMaxVeteran".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref maxVeteranRanged, ref buffMaxVeteranRanged, 1, 999999);
            if (maxVeteranRanged < minVeteranRanged)
                maxVeteranRanged = minVeteranRanged;
            list.Label("MFM_SettingsMinElite".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref minEliteRanged, ref buffMinEliteRanged, 1, 999999);
            list.Label("MFM_SettingsMaxElite".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref maxEliteRanged, ref buffMaxEliteRanged, 1, 999999);
            if (maxEliteRanged < minEliteRanged)
                maxEliteRanged = minEliteRanged;
            list.Label("MFM_SettingsMinCyborg".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref minCyborgRanged, ref buffMinCyborgRanged, 1, 999999);
            list.Label("MFM_SettingsMaxCyborg".Translate("MFM_Ranged".Translate()));
            list.TextFieldNumeric(ref maxCyborgRanged, ref buffMaxCyborgRanged, 1, 999999);
            if (maxCyborgRanged < minCyborgRanged)
                maxCyborgRanged = minCyborgRanged;

            list.Gap();

            list.Label("MFM_SettingsMinRecruit".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref minRecruit, ref buffMinRecruit, 1, 999999);
            list.Label("MFM_SettingsMaxRecruit".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref maxRecruit, ref buffMaxRecruit, 1, 999999);
            if (maxRecruit < minRecruit)
                maxRecruit = minRecruit;
            list.Label("MFM_SettingsMinConfirmed".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref minConfirmed, ref buffMinConfirmed, 1, 999999);
            list.Label("MFM_SettingsMaxConfirmed".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref maxConfirmed, ref buffMaxConfirmed, 1, 999999);
            if (maxConfirmed < minConfirmed)
                maxConfirmed = minConfirmed;
            list.Label("MFM_SettingsMinVeteran".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref minVeteran, ref buffMinVeteran, 1, 999999);
            list.Label("MFM_SettingsMaxVeteran".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref maxVeteran, ref buffMaxVeteran, 1, 999999);
            if (maxVeteran < minVeteran)
                maxVeteran = minVeteran;
            list.Label("MFM_SettingsMinElite".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref minElite, ref buffMinElite, 1, 999999);
            list.Label("MFM_SettingsMaxElite".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref maxElite, ref buffMaxElite, 1, 999999);
            if (maxElite < minElite)
                maxElite = minElite;
            list.Label("MFM_SettingsMinCyborg".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref minCyborg, ref buffMinCyborg, 1, 999999);
            list.Label("MFM_SettingsMaxCyborg".Translate("MFM_Civil".Translate()));
            list.TextFieldNumeric(ref maxCyborg, ref buffMaxCyborg, 1, 999999);
            if (maxCyborg < minCyborg)
                maxCyborg = minCyborg;

            list.Gap();
            list.Gap();
            list.GapLine(10);
            list.Gap(10);
            list.Label("MFM_SettingsSectionIncidents".Translate());
            list.Gap(10);
            list.GapLine(10);

            list.CheckboxLabeled("MFM_SettingsIncidentsRaid".Translate()+Utils.SEP+ "MFM_SettingsIncidentEnable".Translate(), ref enableIncidentRaid);
            list.CheckboxLabeled("MFM_SettingsIncidentsCounterOffer".Translate()+ Utils.SEP + "MFM_SettingsIncidentEnable".Translate(), ref enableIncidentCounterOffer);

            list.Label("MFM_SettingsIncidentsCounterOffer".Translate()+Utils.SEP+ "MFM_SettingsIncidentsCounterOfferMinPrice".Translate((int)(minRateIncSalaryIncidentCounterOffer*100)));
            minRateIncSalaryIncidentCounterOffer = list.Slider(minRateIncSalaryIncidentCounterOffer, 0.5f, 20f);
            list.Label("MFM_SettingsIncidentsCounterOffer".Translate() + Utils.SEP + "MFM_SettingsIncidentsCounterOfferMaxPrice".Translate((int)(maxRateIncSalaryIncidentCounterOffer*100)));
            maxRateIncSalaryIncidentCounterOffer = list.Slider(maxRateIncSalaryIncidentCounterOffer, 0.5f, 20f);
            if (maxRateIncSalaryIncidentCounterOffer < minRateIncSalaryIncidentCounterOffer)
                maxRateIncSalaryIncidentCounterOffer = minRateIncSalaryIncidentCounterOffer;


            list.Gap();
            list.Gap();
            list.GapLine(10);
            GUI.color = Color.green;
            list.Gap(10);
            list.Label("MFM_SettingsSectionBlacklistedPawnKind".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine(10);

            GUI.color = Color.green;
            if (list.ButtonText("+"))
                blacklistedPawnKind.Add("");
            GUI.color = Color.white;

            GUI.color = Color.red;
            if (list.ButtonText("-"))
            {
                if (blacklistedPawnKind.Count != 0)
                    blacklistedPawnKind.RemoveLast();
            }
            GUI.color = Color.white;

            for (var i = 0; i != blacklistedPawnKind.Count; i++)
            {
                list.Label("#" + i.ToString());
                blacklistedPawnKind[i] = list.TextEntry(blacklistedPawnKind[i]);
                list.Gap(4f);
            }

            list.Gap(10f);
            if (list.ButtonText("MFM_SettingsResetBlacklistedPawnKind".Translate()))
                resetBlacklistedPawnKind();


            list.Gap();
            list.Gap();
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionBlacklistedTraits".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();

            GUI.color = Color.green;
            if (list.ButtonText("+"))
                blacklistedTraitsV2.Add("");
            GUI.color = Color.white;

            GUI.color = Color.red;
            if (list.ButtonText("-"))
            {
                if (blacklistedTraitsV2.Count != 0)
                    blacklistedTraitsV2.RemoveLast();
            }
            GUI.color = Color.white;

            for (var i = 0; i != blacklistedTraitsV2.Count; i++)
            {
                list.Label("#" + i.ToString());
                blacklistedTraitsV2[i] = list.TextEntry(blacklistedTraitsV2[i]);
                list.Gap(4f);
            }

            list.Gap(10f);
            if (list.ButtonText("MFM_SettingsResetBlacklistedTraits".Translate()))
                resetBlacklistedTraits();

            list.Gap(25f);
            list.GapLine();



            list.Gap();
            list.Gap();
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionHediffToKeepOnMercGen".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();

            GUI.color = Color.green;
            if (list.ButtonText("+"))
                hediffToNotClear.Add("");
            GUI.color = Color.white;

            GUI.color = Color.red;
            if (list.ButtonText("-"))
            {
                if (hediffToNotClear.Count != 0)
                    hediffToNotClear.RemoveLast();
            }
            GUI.color = Color.white;

            for (var i = 0; i != hediffToNotClear.Count; i++)
            {
                list.Label("#" + i.ToString());
                hediffToNotClear[i] = list.TextEntry(hediffToNotClear[i]);
                list.Gap(4f);
            }

            list.Gap(10f);
            if (list.ButtonText("MFM_SettingsResetBlacklistedTraits".Translate()))
                resetHediffToNotClear();

            list.Gap(25f);
            list.GapLine();




            list.Gap();
            list.Gap();
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionBlacklistedWeapons".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();

            GUI.color = Color.green;
            if (list.ButtonText("+"))
                blacklistedWeapons.Add("");
            GUI.color = Color.white;

            GUI.color = Color.red;
            if (list.ButtonText("-"))
            {
                if (blacklistedWeapons.Count != 0)
                    blacklistedWeapons.RemoveLast();
            }
            GUI.color = Color.white;

            for (var i = 0; i != blacklistedWeapons.Count; i++)
            {
                list.Label("#" + i.ToString());
                blacklistedWeapons[i] = list.TextEntry(blacklistedWeapons[i]);
                list.Gap(4f);
            }

            list.Gap(10f);
            if (list.ButtonText("MFM_SettingsResetBlacklistedWeapons".Translate()))
                resetBlacklistedWeapons();

            list.Gap(25f);
            list.GapLine();





            list.Gap();
            list.Gap();
            list.GapLine();
            list.Gap(10);
            GUI.color = Color.green;
            list.Label("MFM_SettingsSectionMustHaveTraits".Translate());
            GUI.color = Color.white;
            list.Gap(10);
            list.GapLine();

            GUI.color = Color.green;
            if (list.ButtonText("+"))
                mustHaveTraits.Add("");
            GUI.color = Color.white;

            GUI.color = Color.red;
            if (list.ButtonText("-"))
            {
                if (mustHaveTraits.Count != 0)
                    mustHaveTraits.RemoveLast();
            }
            GUI.color = Color.white;

            List<string> toDel = null;

            for (var i = 0; i != mustHaveTraits.Count; i++)
            {
                list.Label("#" + i.ToString());
                mustHaveTraits[i] = list.TextEntry(mustHaveTraits[i]);
                list.Gap(4f);

                //Removal of traits listed in the traits blacklist
                foreach (var bl in blacklistedTraitsV2)
                {
                    if(bl == mustHaveTraits[i])
                    {
                        if (toDel == null)
                            toDel = new List<string>();
                        toDel.Add(bl);
                        break;
                    }
                }
            }

            if(toDel != null)
            {
                foreach(var el in toDel)
                {
                    mustHaveTraits.Remove(el);
                    Messages.Message("MFM_MsgMustHaveTraitInBlacklistedTraits".Translate(el), MessageTypeDefOf.NegativeEvent);
                }
            }

            list.Gap(10f);
            if (list.ButtonText("MFM_SettingsResetMustHaveTraits".Translate()))
                resetMustHaveTraits();

            list.Gap(25f);
            list.GapLine();

            //hediffToNotClear

            list.End();
            Widgets.EndScrollView();
            //settings.Write();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look<string>(ref lastVersionInfo, "lastVersionInfo", "");

            Scribe_Values.Look<bool>(ref allowNonZeroOtherSkillsMerc, "allowNonZeroOtherSkillsMerc", false);
            Scribe_Values.Look<int>(ref buyMercSalaryMultiplier, "buyMercSalaryMultiplier", 8);
            Scribe_Values.Look<float>(ref rateDecreaseAppliedToRentedMercSalary, "rateDecreaseAppliedToRentedMercSalary", 0.45f);
            Scribe_Values.Look<bool>(ref preventNonMercenariesPawnSpawnWithUSFMStuff, "preventNonMercenariesPawnSpawnWithUSFMStuff", false);

            Scribe_Values.Look<float>(ref percentIncomeDecreaseSlave, "percentIncomeDecreaseSlave", 0.30f);
            Scribe_Values.Look<int>(ref currentEpoch, "currentEpoch", 0);

            Scribe_Values.Look<int>(ref medievalModeMinHourDeliveryCaravan, "medievalModeMinHourDeliveryCaravan", 8);
            Scribe_Values.Look<int>(ref medievalModeMaxHourDeliveryCaravan, "medievalModeMaxHourDeliveryCaravan", 48);

            Scribe_Values.Look<bool>(ref allowNonViolentToBeRented, "allowNonViolentToBeRented", true);
            Scribe_Values.Look<int>(ref minXPRentedMercs, "minXPRentedMercs", 600);
            Scribe_Values.Look<int>(ref maxXPRentedMercs, "maxXPRentedMercs", 3000);

            Scribe_Values.Look<int>(ref minAge, "minAge", 18);
            Scribe_Values.Look<int>(ref maxAge, "maxAge", 80);

            Scribe_Values.Look<float>(ref riskCyborgRemovingPartExplode, "riskCyborgRemovingPartExplode", 0.75f);
            Scribe_Values.Look<int>(ref cyborgBionicModSource, "cyborgBionicModSource", -1); 

            Scribe_Values.Look<float>(ref minDiscount, "minDiscount", 0.15f);
            Scribe_Values.Look<float>(ref maxDiscount, "maxDiscount", 0.55f);

            Scribe_Values.Look<int>(ref mercJoinNbSalaryToPay, "mercJoinNbSalaryToPay", 3);
            Scribe_Values.Look<float>(ref minRateIncSalaryIncidentCounterOffer, "minRateIncSalaryIncidentCounterOffer", 1.0f);
            Scribe_Values.Look<float>(ref maxRateIncSalaryIncidentCounterOffer, "maxRateIncSalaryIncidentCounterOffer", 3.0f);

            Scribe_Values.Look<bool>(ref enableIncidentCounterOffer, "enableIncidentCounterOffer", true);
            Scribe_Values.Look<bool>(ref enableIncidentRaid, "enableIncidentRaid", true);

            Scribe_Values.Look<int>(ref powerBeamCost, "powerBeamCost", 25000);
            Scribe_Values.Look<int>(ref minTimeUSFMSuspended, "minTimeUSFMSuspended", 480000);
            Scribe_Values.Look<int>(ref maxTimeUSFMSuspended, "maxTimeUSFMSuspended", 1200000);

            Scribe_Values.Look<int>(ref badMoodNbhPerQuadFloor, "badMoodNbhPerQuadFloor", 72);
            Scribe_Values.Look<int>(ref goodMoodNbhPerQuadFloor, "goodMoodNbhPerQuadFloor", 96);
            Scribe_Values.Look<float>(ref badMoodCoefIncCost, "badMoodCoefIncCost", 0.30f);
            Scribe_Values.Look<float>(ref goodMoodCoefDecCost, "goodMoodCoefDecCost", 0.25f);

            Scribe_Values.Look<int>(ref minHourDeliverStuffAndGuarantee, "minHourDeliverStuffAndGuarantee", 2);
            Scribe_Values.Look<int>(ref maxHourDeliverStuffAndGuarantee, "maxHourDeliverStuffAndGuarantee", 9);

            Scribe_Values.Look<bool>(ref onlyVanillaForWeapon, "onlyVanillaForWeapon", false);
            Scribe_Values.Look<bool>(ref onlyVanillaForGear, "onlyVanillaForGear", false);
            //Scribe_Values.Look<bool>(ref onlyVanillaForCyborg, "onlyVanillaForCyborg", false);
            
            Scribe_Values.Look<bool>(ref hideMercenariesIcon, "hideMercenariesIcon", false);
            Scribe_Values.Look<bool>(ref hideMercenariesLevel, "hideMercenariesLevel", false);
            Scribe_Values.Look<bool>(ref disableNotifWhenDisusedMercDie, "disableNotifWhenDisusedMercDie", false);
            Scribe_Values.Look<int>(ref timeBeforeRenewOffers, "timeBeforeRenewOffers", 1800000);

            Scribe_Values.Look<int>(ref transportQuickMinHour, "transportQuickMinHour", 12);
            Scribe_Values.Look<int>(ref transportQuickMaxHour, "transportQuickMaxHour", 24);
            Scribe_Values.Look<int>(ref transportStandardMinHour, "transportStandardMinHour", 24);
            Scribe_Values.Look<int>(ref transportStandardMaxHour, "transportStandardMaxHour", 240);

            Scribe_Values.Look<int>(ref transportQuickPrice, "transportQuickPrice", 250);
            Scribe_Values.Look<int>(ref transportImmediatePrice, "transportImmediatePrice", 500);
            

            Scribe_Values.Look<int>(ref minRecruitMelee, "minRecruitMelee", 15);
            Scribe_Values.Look<int>(ref maxRecruitMelee, "maxRecruitMelee", 100);
            Scribe_Values.Look<int>(ref minConfirmedMelee, "minConfirmedMelee", 5);
            Scribe_Values.Look<int>(ref maxConfirmedMelee, "maxConfirmedMelee", 75);
            Scribe_Values.Look<int>(ref minVeteranMelee, "minVeteranMelee", 1);
            Scribe_Values.Look<int>(ref maxVeteranMelee, "maxVeteranMelee", 50);
            Scribe_Values.Look<int>(ref minEliteMelee, "minEliteMelee", 0);
            Scribe_Values.Look<int>(ref maxEliteMelee, "maxEliteMelee", 25);
            Scribe_Values.Look<int>(ref minCyborgMelee, "minCyborgMelee", 0);
            Scribe_Values.Look<int>(ref maxCyborgMelee, "maxCyborgMelee", 10);

            Scribe_Values.Look<int>(ref minRecruitRanged, "minRecruitRanged", 15);
            Scribe_Values.Look<int>(ref maxRecruitRanged, "maxRecruitRanged", 100);
            Scribe_Values.Look<int>(ref minConfirmedRanged, "minConfirmedRanged", 5);
            Scribe_Values.Look<int>(ref maxConfirmedRanged, "maxConfirmedRanged", 75);
            Scribe_Values.Look<int>(ref minVeteranRanged, "minVeteranRanged", 1);
            Scribe_Values.Look<int>(ref maxVeteranRanged, "maxVeteranRanged", 50);
            Scribe_Values.Look<int>(ref minEliteRanged, "minEliteRanged", 0);
            Scribe_Values.Look<int>(ref maxEliteRanged, "maxEliteRanged", 25);
            Scribe_Values.Look<int>(ref minCyborgRanged, "minCyborgRanged", 0);
            Scribe_Values.Look<int>(ref maxCyborgRanged, "maxCyborgRanged", 10);

            Scribe_Values.Look<int>(ref minRecruit, "minRecruit", 10);
            Scribe_Values.Look<int>(ref maxRecruit, "maxRecruit", 45);
            Scribe_Values.Look<int>(ref minConfirmed, "minConfirmed", 5);
            Scribe_Values.Look<int>(ref maxConfirmed, "maxConfirmed", 25);
            Scribe_Values.Look<int>(ref minVeteran, "minVeteran", 1);
            Scribe_Values.Look<int>(ref maxVeteran, "maxVeteran", 15);
            Scribe_Values.Look<int>(ref minElite, "minElite", 0);
            Scribe_Values.Look<int>(ref maxElite, "maxElite", 8);
            Scribe_Values.Look<int>(ref minCyborg, "minCyborg", 0);
            Scribe_Values.Look<int>(ref maxCyborg, "maxCyborg", 10);

            Scribe_Values.Look<int>(ref priceGear1, "priceGear1", 745);
            Scribe_Values.Look<int>(ref priceGear2, "priceGear2", 1200);
            Scribe_Values.Look<int>(ref priceGear3, "priceGear3", 2700);

            Scribe_Values.Look<int>(ref priceWeapon1, "priceWeapon1", 250);
            Scribe_Values.Look<int>(ref priceWeapon2, "priceWeapon2", 600);
            Scribe_Values.Look<int>(ref priceWeapon3, "priceWeapon3", 900);
            Scribe_Values.Look<int>(ref priceWeapon4, "priceWeapon4", 1600);

            Scribe_Values.Look<int>(ref priceRecruit, "priceRecruit", 400);
            Scribe_Values.Look<int>(ref priceConfirmed, "priceConfirmed", 750);
            Scribe_Values.Look<int>(ref priceVeteran, "priceVeteran", 1000);
            Scribe_Values.Look<int>(ref priceElite, "priceElite", 1400);
            Scribe_Values.Look<int>(ref priceCyborg, "priceCyborg", 2500);

            Scribe_Values.Look<int>(ref guaranteeRecruit, "guaranteeRecruit", 400);
            Scribe_Values.Look<int>(ref guaranteeConfirmed, "guaranteeConfirmed", 750);
            Scribe_Values.Look<int>(ref guaranteeVeteran, "guaranteeVeteran", 1000);
            Scribe_Values.Look<int>(ref guaranteeElite, "guaranteeElite", 1400);
            Scribe_Values.Look<int>(ref guaranteeCyborg, "guaranteeCyborg", 2500);


            Scribe_Collections.Look<string>(ref mustHaveTraits, "mustHaveTraits", LookMode.Value);
            Scribe_Collections.Look<string>(ref blacklistedWeapons, "blacklistedWeapons", LookMode.Value);
            Scribe_Collections.Look<string>(ref blacklistedPawnKind, "blacklistedPawnKind", LookMode.Value);
            Scribe_Collections.Look<string>(ref blacklistedTraitsV2, "blacklistedTraitsV2", LookMode.Value);
            Scribe_Collections.Look<string>(ref hediffToNotClear, "hediffToNotClear", LookMode.Value);
            Scribe_Values.Look<bool>(ref imprisonedMercenariesAreNoLongerMerc, "imprisonedMercenariesAreNoLongerMerc", true);
            

            if (blacklistedTraitsV2 == null)
            {
                resetBlacklistedTraits();
            }
            if (blacklistedPawnKind == null)
            {
                resetBlacklistedPawnKind();
            }
            if(hediffToNotClear == null)
            {
                resetHediffToNotClear();
            }
            if(blacklistedWeapons == null)
            {
                resetBlacklistedWeapons();
            }
            if(mustHaveTraits == null)
            {
                resetMustHaveTraits();
            }
        }


        public static void resetMustHaveTraits()
        {
            mustHaveTraits = new List<string>() { };
        }

        public static void resetBlacklistedPawnKind()
        {
            blacklistedPawnKind = new List<string>() { "M7MechPawn", "Android5Tier" };
        }

        public static void resetBlacklistedTraits()
        {
            blacklistedTraitsV2 = new List<string>() { "wimp", "pyromaniac", "abrasive", "nervous", "volatile", "psychically hypersensitive", "psychically sensitive", "pessimist", "depressive" };
        }

        public static void resetHediffToNotClear()
        {
            hediffToNotClear = new List<string> { "DummyPrivates", "DummyPrivatesForModdedPawnsOnBirthday" };
        }

        public static void resetBlacklistedWeapons()
        {
            blacklistedWeapons = new List<string> { };
        }

        public static int getBionicSource()
        {
            //In auto mode the source is determined automatically
            if (cyborgBionicModSource == -1)
            {
                if (Utils.EPOELOADED)
                    return 1;
                else if (Utils.RSBELOADED)
                    return 2;
                else if (Utils.CONNLOADED)
                    return 3;
                else if (Utils.EVOLOADED)
                    return 4;
                else
                    return 0;
            }
            else
                return cyborgBionicModSource;
        }
    }
}