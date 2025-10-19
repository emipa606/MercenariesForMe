using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace aRandomKiwi.MFM;

public class GC_MFM : GameComponent
{
    private readonly Game game;
    private bool billInProgress;

    private bool counterOfferInProgress;

    //Discounts 
    private Dictionary<string, float> discounts = new();

    public Faction factionIronAlliance;
    private int GTFirstDiscount;
    private int lastQuad = -1;
    private List<Pawn> listerRentedMercenaries = [];

    private bool mercWantJoinInProgress;

    //Medieval delivery caravan
    private Dictionary<int, string> pendingMedievalDelivery = new();

    private Dictionary<int, string> pendingMedievalDeliveryText = new();

    //Mercenary order being delivered
    private Dictionary<int, string> pendingMercOrder = new();

    //Powerbeam attack launched by USFM
    private Dictionary<int, string> pendingPowerBeam = new();

    //Mercenaries of the players to return to the colony
    private Dictionary<int, string> pendingRentedMercComeBack = new();

    //Return of deposit and stuff in progress after mercenaries leave
    private Dictionary<int, string> pendingStuffAndGuarantee = new();

    public string preferedRace = "";

    private HashSet<Pawn> rentedPawns = [];

    private List<Settlement> savedIAS = [];

    //Quantity available for the moment of mercenaries
    private Dictionary<string, int> stock = new();
    private int stockExpireGT;

    public GC_MFM(Game game)
    {
        this.game = game;
        Utils.GCMFM = this;

        var data = new Dictionary<string, List<string>>();

        if (Utils.MEDIEVALTIMESLOADED)
        {
            data["MFM_IronAllianceBowman"] =
            [
                "MedTimes_AppBody_Tunic", "MedTimes_AppBody_Trousers", "MedTimes_Socks_Tailored",
                "MedTimes_Helm_Wooden", "MedTimes_WoodenPlates", "MedTimes_Bracers_Wooden", "MedTimes_Boots_Wooden"
            ];
            data["MFM_IronAllianceGuard"] =
            [
                "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored",
                "MedTimes_Hauberk", "MedTimes_Gloves_Scaled", "MedTimes_Boots_Scaled", "MedTimes_Helm_Domed"
            ];
            data["MFM_IronAllianceEliteGuard"] =
            [
                "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored",
                "MedTimes_EncasedSteel", "MedTimes_Gauntlets_EncasedSteel", "MedTimes_Boots_EncasedSteel",
                "Apparel_ShieldBelt", "MedTimes_Helm_Domed"
            ];
            data["MFM_IronAllianceEliteBowman"] =
            [
                "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored",
                "MedTimes_Helm_Headwrap", "MedTimes_PlateJack", "MedTimes_Bracer_Archer", "MedTimes_Boots_Plated"
            ];
            data["MFM_IronAllianceVillager"] =
            [
                "MedTimes_AppBody_Tunic", "MedTimes_AppBody_Trousers", "MedTimes_Socks_Tailored",
                "MedTimes_Headgear_ArmingCap", "MedTimes_Gloves_Tailored", "MedTimes_Boots_Tailored"
            ];
            data["MFM_IronAllianceTrader"] = data["MFM_IronAllianceVillager"];
            data["MFM_IronAllianceKnight"] =
            [
                "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored",
                "MedTimes_EncasedSteel", "MedTimes_Gauntlets_EncasedSteel", "MedTimes_Boots_EncasedSteel",
                "Apparel_ShieldBelt", "MedTimes_Helm_DeathMask"
            ];
            data["MFM_IronAllianceLord"] = data["MFM_IronAllianceKnight"];
        }
        else
        {
            data["MFM_IronAllianceEliteBowman"] = ["Apparel_PlateArmor"];
            data["MFM_IronAllianceEliteGuard"] = ["Apparel_PlateArmor", "Apparel_SimpleHelmet"];
            data["MFM_IronAllianceKnight"] = ["Apparel_PlateArmor", "Apparel_AdvancedHelmet"];
            data["MFM_IronAllianceLord"] = ["Apparel_PlateArmor", "Apparel_AdvancedHelmet"];
        }

        foreach (var entry in data)
        {
            try
            {
                var kd = DefDatabase<PawnKindDef>.GetNamedSilentFail(entry.Key);
                if (kd == null)
                {
                    continue;
                }

                kd.apparelRequired.Clear();
                foreach (var el in entry.Value)
                {
                    try
                    {
                        var td = DefDatabase<ThingDef>.GetNamedSilentFail(el);
                        if (td == null)
                        {
                            continue;
                        }

                        kd.apparelRequired.Add(td);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        //Application initiale des regles sur les vetements USFM
        Utils.refreshUSFMStuffRule();
        Utils.SS_slaveHediff = DefDatabase<HediffDef>.GetNamedSilentFail("Enslaved");
    }

    public bool CounterOfferInProgress
    {
        get => counterOfferInProgress;

        set => counterOfferInProgress = value;
    }

    public bool MercWantJoinInProgress
    {
        get => mercWantJoinInProgress;

        set => mercWantJoinInProgress = value;
    }

    public bool BillInProgress
    {
        get => billInProgress;

        set => billInProgress = value;
    }

    public override void ExposeData()
    {
        base.ExposeData();

        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            reset();
        }

        Scribe_Values.Look(ref preferedRace, "preferedRace", "");
        Scribe_Collections.Look(ref savedIAS, "savedIAS", LookMode.Deep);
        Scribe_Values.Look(ref billInProgress, "billInProgress");
        Scribe_Values.Look(ref mercWantJoinInProgress, "mercWantJoinInProgress");
        Scribe_Collections.Look(ref rentedPawns, "rentedPawns", LookMode.Deep);
        Scribe_Values.Look(ref counterOfferInProgress, "counterOfferInProgress");
        Scribe_Values.Look(ref GTFirstDiscount, "GTFirstDiscount");
        Scribe_Collections.Look(ref discounts, "discounts", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref pendingMedievalDelivery, "pendingMedievalDelivery", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref pendingMedievalDeliveryText, "pendingMedievalDeliveryText", LookMode.Value,
            LookMode.Value);
        Scribe_Collections.Look(ref pendingStuffAndGuarantee, "pendingStuffAndGuarantee", LookMode.Value,
            LookMode.Value);
        Scribe_Collections.Look(ref pendingPowerBeam, "pendingPowerBeam", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref pendingMercOrder, "pendingMercOrder", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref pendingRentedMercComeBack, "pendingRentedMercComeBack", LookMode.Value,
            LookMode.Value);
        Scribe_Collections.Look(ref listerRentedMercenaries, "listerRentedMercenaries", LookMode.Reference);
        Scribe_Collections.Look(ref stock, "stock", LookMode.Value, LookMode.Value);
        Scribe_Values.Look(ref stockExpireGT, "stockExpireGT");
        Scribe_Values.Look(ref lastQuad, "lastQuad", -1);

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            initNull();
        }

        //Suppression des références null 
        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            listerRentedMercenaries.RemoveAll(item => item == null);
        }
    }


    public override void GameComponentTick()
    {
        var GT = Find.TickManager.TicksGame;

        if (GT % 120 == 0)
        {
            //Check de l'arrivage ou non de mercenaire faisant suite à une commande
            checkPendingMercOrder();
            //Check retour de mercenaires du joueur
            checkPendingRentedMercComeBack();
            //Check retour stuff et guarantee 
            checkPendingStuffAndGuarantee();
            //Pending powerbeam
            checkPowerBeamAttack();
            //Caravanes de livraison
            checkPendingMedievalDeliveryCaravan();
        }

        //Toutes les heures
        if (GT % 2500 == 0)
        {
            //Incrémentation XP des mercenaires loués tous les jours
            checkRentedMercEarnedXP();
            //Check renouvellement des offres USFM
            checkForRenewUSFMOffers();
            //Check si on doit afficher la facture
            checkQuadrumBill();
            checkGotFirstDiscount();
        }

        if (GT % 60000 == 0)
        {
            //Suppression SOP potentiels oubliés
            Utils.clearAllMedievalSiteOfPayment();
        }
    }

    public override void LoadedGame()
    {
        base.LoadedGame();
        checkInitFirstDiscount();
        checkForRenewUSFMOffers();
        checkLastQuadInit();
        checkRemoveIronAlliance();
    }

    public override void StartedNewGame()
    {
        checkForRenewUSFMOffers();
        checkInitFirstDiscount();
        checkLastQuadInit();
        checkRemoveIronAlliance();
    }

    private void checkRentedMercEarnedXP()
    {
        var CGT = Find.TickManager.TicksGame;

        foreach (var m in listerRentedMercenaries)
        {
            //reevaluation experience gagné
            var comp = m?.TryGetComp<Comp_USFM>();
            if (comp == null)
            {
                continue;
            }

            var diff = 0;

            if (comp.xpEarnedLastCT != -1)
            {
                diff = CGT - comp.xpEarnedLastCT;
            }
            else
            {
                comp.xpEarnedLastCT = CGT;
            }

            if (diff < 2500)
            {
                continue;
            }

            var nbHours = diff / 2500;
            //Log.Message("Increment XP of " + nbHours + " for " + m.LabelCap);
            Utils.incMercSkillXP(m, nbHours);
            comp.xpEarnedLastCT = CGT;
        }
    }

    private void checkRemoveIronAlliance()
    {
        factionIronAlliance =
            Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("USFM_FactionAOS"));
        if (factionIronAlliance == null)
        {
            return;
        }

        if (Settings.currentEpoch == 3)
        {
            if (!factionIronAlliance.defeated)
            {
                foreach (var el in Find.WorldObjects.SettlementBases.ToList())
                {
                    if (el.Faction == factionIronAlliance)
                    {
                        savedIAS.Add(el);
                    }
                }

                if (savedIAS.Count != 0)
                {
                    foreach (var el in savedIAS)
                    {
                        Find.WorldObjects.SettlementBases.Remove(el);
                        Find.WorldObjects.Remove(el);
                    }
                }

                factionIronAlliance.defeated = true;
                //Find.FactionManager.Remove(factionIronAlliance);
            }

            factionIronAlliance.def.hidden = true;
            factionIronAlliance = null;
        }
        else
        {
            //Force faction discovery a recreer faction le cas echeant (faction detruite)
            if (factionIronAlliance.defeated)
            {
                foreach (var el in savedIAS)
                {
                    //settlement.SetFaction(faction);
                    //settlement.Tile = num4;
                    //settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement, null);
                    if (!Find.WorldObjects.Contains(el))
                    {
                        Find.WorldObjects.Add(el);
                    }
                }

                factionIronAlliance.defeated = false;
            }

            savedIAS.Clear();
            factionIronAlliance.def.hidden = false;
        }
    }


    private void checkLastQuadInit()
    {
        if (lastQuad == -1)
        {
            lastQuad = (int)Utils.getCurrentQuad();
        }
    }

    /*
     * Check de l'arrivage ou non de caravane de paiement de l'iron alliance
     */
    private void checkPendingMedievalDeliveryCaravan()
    {
        var CGT = Find.TickManager.TicksGame;
        List<int> toDel = null;
        foreach (var entry in pendingMedievalDelivery)
        {
            //Commande arrivée à échéance livraison des mercenaires
            if (entry.Key > CGT)
            {
                continue;
            }

            toDel ??= [];

            toDel.Add(entry.Key);

            var wanted = Utils.unserializeDSI(entry.Value);
            if (wanted == null)
            {
                continue;
            }


            var map = Utils.getMapFromUID(wanted["map"]) ?? Find.CurrentMap;

            //Spawn caravane allant déposer la cargaison
            var thing = ThingMaker.MakeThing(ThingDefOf.Silver);
            thing.stackCount = wanted["silver"];
            var toSpawn = new List<Thing> { thing };

            //Spawn de la caravane est larguage de la tune
            if (Utils.spawnMedievalCaravan(map, toSpawn, out var spawnedThings))
            {
                //Affichage message souhaité
                Find.LetterStack.ReceiveLetter("MFM_LetterArrivedMedievalCaravanDelivery".Translate(),
                    pendingMedievalDeliveryText[entry.Key], LetterDefOf.PositiveEvent,
                    new LookTargets(spawnedThings, map));
            }
        }

        if (toDel == null)
        {
            return;
        }

        foreach (var entry in toDel)
        {
            pendingMedievalDelivery.Remove(entry);
            pendingMedievalDeliveryText.Remove(entry);
        }
    }

    private void checkGotFirstDiscount()
    {
        var CGT = Find.TickManager.TicksGame;

        if (GTFirstDiscount == -1 || CGT <= GTFirstDiscount)
        {
            return;
        }

        GTFirstDiscount = -1;
        var discount = Rand.Range(Settings.minDiscount, Settings.maxDiscount);
        var code = Utils.generateDiscountCode((uint)CGT);
        addDiscount(code, discount);
        Find.LetterStack.ReceiveLetter("MFM_LetterDiscount".Translate(Utils.getUSFMLabel()),
            "MFM_LetterFirstDiscount".Translate((int)(discount * 100), code), LetterDefOf.PositiveEvent);
    }

    private void checkInitFirstDiscount()
    {
        if (GTFirstDiscount == 0)
        {
            GTFirstDiscount = Rand.Range(18000, 360000);
        }
    }

    /*
     * Check powerbeam attack
     */
    private void checkPowerBeamAttack()
    {
        var CGT = Find.TickManager.TicksGame;
        List<int> toDel = null;
        foreach (var entry in pendingPowerBeam)
        {
            if (entry.Key > CGT)
            {
                continue;
            }

            toDel ??= [];

            toDel.Add(entry.Key);

            Utils.unserializePowerBeamAttack(entry.Value, out var map, out var coord);
            if (map == null)
            {
                continue;
            }

            Utils.startPowerBeamAt(map, coord);

            //msg = "MFM_LetterDepositRecoveryDesc".Translate(toDeliver.Count - 1, lst.Count, txt + "\n", guaranteeTotal);
            //Find.LetterStack.ReceiveLetter("MFM_LetterDepositRecovery".Translate(), msg, LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, backMap));
        }

        if (toDel == null)
        {
            return;
        }

        foreach (var entry in toDel)
        {
            pendingPowerBeam.Remove(entry);
        }
    }

    /*
     * Check retour par dropPod des guaranties et stuff
     */
    private void checkPendingStuffAndGuarantee()
    {
        if (pendingStuffAndGuarantee.Count == 0)
        {
            return;
        }

        var CGT = Find.TickManager.TicksGame;
        List<int> toDel = null;
        foreach (var entry in pendingStuffAndGuarantee)
        {
            //Commande arrivé à échéance retour des stuff et guarantee
            if (entry.Key > CGT)
            {
                continue;
            }

            toDel ??= [];

            toDel.Add(entry.Key);

            var lst = Utils.unserializeLP(entry.Value, out var backMap);
            if (lst == null)
            {
                continue;
            }

            backMap ??= Utils.getRandomMapOfPlayer();

            var guaranteeTotal = 0;
            var txt = "";
            var toDeliver = new List<Thing>();
            var toDel2 = new List<Thing>();

            //Calcul texte et montant guaranties + Déduction et comptage nombre de stuff à restituer
            foreach (var p in lst)
            {
                var comp = p.TryGetComp<Comp_USFM>();
                if (comp == null)
                {
                    continue;
                }

                toDel2.Clear();
                var newScore = Utils.getPawnScore(p);
                //Le pawn à été abimé on va effectué un prorata de sa caution
                if (comp.origScore > newScore)
                {
                    var val = (int)(comp.guarantee * ((comp.origScore - newScore) / comp.origScore));
                    txt += "\n" +
                           "MFM_GuaranteeLineNOK".Translate(comp.guarantee - val, p.LabelCap, comp.guarantee, val);
                    guaranteeTotal += comp.guarantee - val;
                }
                else
                {
                    txt += "\n" + "MFM_GuaranteeLineOK".Translate(comp.guarantee, p.LabelCap);
                    guaranteeTotal += comp.guarantee;
                }

                if (p.equipment != null)
                {
                    foreach (var e in p.equipment.AllEquipmentListForReading)
                    {
                        toDeliver.Add(e);
                        toDel2.Add(e);
                    }

                    foreach (var e in toDel2)
                    {
                        p.equipment.Remove((ThingWithComps)e);
                    }

                    toDel2.Clear();
                }

                if (p.apparel != null)
                {
                    foreach (var e in p.apparel.WornApparel)
                    {
                        if (e.def.defName == "MFM_Apparel_USFMBasicShirt" ||
                            e.def.defName == "MFM_Apparel_USFMBasicShirtCivil")
                        {
                            continue;
                        }

                        toDeliver.Add(e);
                        toDel2.Add(e);
                    }

                    foreach (var e in toDel2)
                    {
                        p.apparel.Remove((Apparel)e);
                        //p.apparel.WornApparel.Remove((Apparel)e);
                    }

                    toDel2.Clear();
                }

                if (p.inventory != null)
                {
                    foreach (var e in p.inventory.innerContainer.InnerListForReading)
                    {
                        toDeliver.Add(e);
                        toDel2.Add(e);
                    }

                    foreach (var e in toDel2)
                    {
                        p.inventory.innerContainer.Remove(e);
                    }
                }

                if (rentedPawns.Contains(p))
                {
                    popRentedPawn(p);
                }

                Find.WorldPawns.PassToWorld(p, PawnDiscardDecideMode.Discard);
            }

            var thing = ThingMaker.MakeThing(ThingDefOf.Silver);
            thing.stackCount = guaranteeTotal;
            toDeliver.Add(thing);

            var dropCellNear = DropCellFinder.TradeDropSpot(backMap);

            if (!dropCellNear.IsValid)
            {
                //dropCellNear = CellFinder.RandomEdgeCell(backMap);
                dropCellNear = DropCellFinder.FindRaidDropCenterDistant(backMap);
            }

            //S'il y a des trucs à délivrer
            if (toDeliver.Count != 0)
            {
                if (Utils.modernUSFM())
                {
                    DropPodUtility.DropThingsNear(dropCellNear, backMap, toDeliver, 100, false, false, false);
                }
                else
                {
                    Utils.spawnMedievalCaravan(backMap, toDeliver, out dropCellNear);
                }
            }

            string msg = "MFM_LetterDepositRecoveryDesc".Translate(toDeliver.Count - 1, lst.Count, $"{txt}\n",
                guaranteeTotal);
            Find.LetterStack.ReceiveLetter("MFM_LetterDepositRecovery".Translate(), msg, LetterDefOf.PositiveEvent,
                new LookTargets(dropCellNear, backMap));
        }

        if (toDel == null)
        {
            return;
        }

        foreach (var entry in toDel)
        {
            pendingStuffAndGuarantee.Remove(entry);
        }
    }

    /*
     * Facture quadrum si mercenaire et 1er du mois
     */
    private void checkQuadrumBill()
    {
        var cquad = (int)Utils.getCurrentQuad();


        //Si le premier du mois (lastQuad != cquad) Et il y a des éléments facturables (mercenaires acheté ou loué ou mercenaire en cours de livraison).
        if (cquad == lastQuad ||
            !playerHaveMerc() && listerRentedMercenaries.Count == 0 && pendingMercOrder.Count == 0)
        {
            return;
        }

        lastQuad = cquad;
        //Check liste de colons rented et check les dégats  (death, renvois)

        //Création du point de paiement si en mode médiéval
        if (!Utils.modernUSFM())
        {
            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome)
                {
                    Utils.createSiteOfPayment(map);
                }
            }
        }

        Utils.GCMFM.BillInProgress = true;
        var cl = (ChoiceLetter_Bill)LetterMaker.MakeLetter(DefDatabase<LetterDef>.GetNamed("MFM_CLBill"));
        cl.init();
        cl.StartTimeout(60000);
        cl.Label = "MFM_Bill".Translate(Utils.getUSFMLabel());
        Find.LetterStack.ReceiveLetter(cl);
    }

    /*
     * Check de l'arrivage ou non de mercenaire faisant suite à une commande
     */
    private void checkPendingMercOrder()
    {
        var CGT = Find.TickManager.TicksGame;
        List<int> toDel = null;
        foreach (var entry in pendingMercOrder)
        {
            //Commande arrivée à échéance livraison des mercenaires
            if (entry.Key > CGT)
            {
                continue;
            }

            toDel ??= [];

            toDel.Add(entry.Key);

            var wanted = Utils.unserializeDSI(entry.Value);
            if (wanted == null)
            {
                continue;
            }

            _ = Utils.getMapFromUID(wanted["map"]) ?? Find.CurrentMap;

            spawnOrderedMercs(wanted);
        }

        if (toDel == null)
        {
            return;
        }

        foreach (var entry in toDel)
        {
            pendingMercOrder.Remove(entry);
        }
    }


    public bool mercIsInPendingRentedMercComeBack(string mercUID)
    {
        foreach (var entry in pendingRentedMercComeBack)
        {
            var data = Utils.unserializeLS(entry.Value);
            if (data == null)
            {
                continue;
            }

            foreach (var d in data)
            {
                if (d == mercUID)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /*
     * Check du retour de mercenaires du joueur
     */
    private void checkPendingRentedMercComeBack()
    {
        var CGT = Find.TickManager.TicksGame;
        List<int> toDel = null;
        foreach (var entry in pendingRentedMercComeBack)
        {
            //Commande arrivée à échéance livraison des mercenaires
            if (entry.Key > CGT)
            {
                continue;
            }

            toDel ??= [];

            toDel.Add(entry.Key);

            var data = Utils.unserializeLS(entry.Value);
            if (data == null)
            {
                continue;
            }

            Map map;
            //Obtention de la map concernée
            if (data.Count == 0)
            {
                map = Utils.getRandomMapOfPlayer();
            }
            else
            {
                int.TryParse(data[0], out var val);
                map = Utils.getMapFromUID(val);
            }

            map ??= Find.CurrentMap;

            //Patch avec SOS2: Si c'est une map spatiale SOS2, on prendra une autre --Par Ionfrigate12345:
            if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
            {
                map = HarmonyUtils.GetPlayerMainColonyMap(true, false);
            }

            if (map == null) //Si on trouve toujours pas (le joueur n'a pas de map planétaire)
            {
                Log.Warning(
                    "Cannot find a map to spawn mercenaries. The player seems to have only SOS2 space maps where the spawning is prevented.");
                continue;
            }

            //IntVec3 dropCellNear = CellFinder.RandomEdgeCell(map);

            //Spawn par dropPod des mercenaire au player mentionnés
            var mercs = new List<Pawn>();
            foreach (var d in data)
            {
                foreach (var m in listerRentedMercenaries)
                {
                    if (m == null)
                    {
                        continue;
                    }

                    if (m.GetUniqueLoadID() != d)
                    {
                        continue;
                    }

                    if (rentedPawns.Contains(m))
                    {
                        rentedPawns.Remove(m);
                        //Find.WorldPawns.PassToWorld(m, RimWorld.Planet.PawnDiscardDecideMode.KeepForever);
                    }

                    if (Find.WorldPawns.Contains(m))
                    {
                        Find.WorldPawns.RemovePawn(m);
                    }

                    m.SetFactionDirect(Faction.OfPlayer);
                    if (m.holdingOwner != null)
                    {
                        m.holdingOwner.Remove(m);
                        m.holdingOwner = null;
                    }

                    mercs.Add(m);
                }
            }

            //Suppression des mercenaires devant revenir de la liste des rented mercenaries
            if (mercs.Count <= 0)
            {
                continue;
            }

            foreach (var m in mercs)
            {
                //Advance bio age if applicable
                var comp = m.TryGetComp<Comp_USFM>();
                comp?.rentedMercAdvanceBioAge();

                if (listerRentedMercenaries.Contains(m))
                {
                    listerRentedMercenaries.Remove(m);
                }
            }

            //spawn effectif des mercenaires par dropPods

            var dropCellNear = Utils.spawnMercOnMap(map, mercs);
            if (dropCellNear.IsValid)
            {
                Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercArrival".Translate(),
                    "MFM_LetterRentedMercArrivalDesc".Translate(Utils.getUSFMLabel()),
                    LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, map));
            }
        }

        if (toDel == null)
        {
            return;
        }

        foreach (var entry in toDel)
        {
            pendingRentedMercComeBack.Remove(entry);
        }
    }

    /*
     * Check si les offres de l'USFM doivent être renouvellées
     */
    private void checkForRenewUSFMOffers()
    {
        var CGT = Find.TickManager.TicksGame;

        //Renouvellement si expiration des offres atteintes 
        if (CGT <= stockExpireGT && stock.Count != 0)
        {
            return;
        }

        foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
        {
            var indexBase = ((int)type).ToString();

            int nbRecruitMin;
            int nbRecruitMax;
            int nbCyborgMax;
            int nbCyborgMin;
            int nbEliteMax;
            int nbEliteMin;
            int nbVeteranMax;
            int nbVeteranMin;
            int nbConfirmedMax;
            int nbConfirmedMin;
            switch (type)
            {
                case MercenaryType.Melee:
                    nbRecruitMin = Settings.minRecruitMelee;
                    nbRecruitMax = Settings.maxRecruitMelee;
                    nbConfirmedMin = Settings.minConfirmedMelee;
                    nbConfirmedMax = Settings.maxConfirmedMelee;
                    nbVeteranMin = Settings.minVeteranMelee;
                    nbVeteranMax = Settings.maxVeteranMelee;
                    nbEliteMin = Settings.minEliteMelee;
                    nbEliteMax = Settings.maxEliteMelee;
                    nbCyborgMin = Settings.minCyborgMelee;
                    nbCyborgMax = Settings.maxCyborgMelee;
                    break;
                case MercenaryType.Ranged:
                    nbRecruitMin = Settings.minRecruitRanged;
                    nbRecruitMax = Settings.maxRecruitRanged;
                    nbConfirmedMin = Settings.minConfirmedRanged;
                    nbConfirmedMax = Settings.maxConfirmedRanged;
                    nbVeteranMin = Settings.minVeteranRanged;
                    nbVeteranMax = Settings.maxVeteranRanged;
                    nbEliteMin = Settings.minEliteRanged;
                    nbEliteMax = Settings.maxEliteRanged;
                    nbCyborgMin = Settings.minCyborgRanged;
                    nbCyborgMax = Settings.maxCyborgRanged;
                    break;
                default:
                    nbRecruitMin = Settings.minRecruit;
                    nbRecruitMax = Settings.maxRecruit;
                    nbConfirmedMin = Settings.minConfirmed;
                    nbConfirmedMax = Settings.maxConfirmed;
                    nbVeteranMin = Settings.minVeteran;
                    nbVeteranMax = Settings.maxVeteran;
                    nbEliteMin = Settings.minElite;
                    nbEliteMax = Settings.maxElite;
                    nbCyborgMin = Settings.minCyborg;
                    nbCyborgMax = Settings.maxCyborg;
                    break;
            }

            stock[$"{indexBase}nbRecruit"] = Rand.Range(nbRecruitMin, nbRecruitMax);
            stock[$"{indexBase}nbConfirmed"] = Rand.Range(nbConfirmedMin, nbConfirmedMax);
            stock[$"{indexBase}nbVeteran"] = Rand.Range(nbVeteranMin, nbVeteranMax);
            stock[$"{indexBase}nbElite"] = Rand.Range(nbEliteMin, nbEliteMax);
            stock[$"{indexBase}nbCyborg"] = Rand.Range(nbCyborgMin, nbCyborgMax);
        }

        if (stockExpireGT != 0)
        {
            Find.LetterStack.ReceiveLetter("MFM_LetterUSFMRenewStock".Translate(Utils.getUSFMLabel()),
                "MFM_LetterUSFMRenewStockDesc".Translate(Utils.getUSFMLabel()), LetterDefOf.PositiveEvent);
        }

        //Report renouvellement stock de mercenaires
        stockExpireGT = CGT + Settings.timeBeforeRenewOffers;
    }

    /*
     * Spawn des mercenaires commandés
     */
    private static void spawnOrderedMercs(Dictionary<string, int> wanted)
    {
        var mercs = new List<Pawn>();

        //Déduction race le cas echeant
        var defNameRace = Utils.getFirstKeyStartingWith(wanted, "race_");
        defNameRace = defNameRace?.ReplaceFirst("race_", "");

        //Check si mercenaires achetés 
        var purchased = wanted.ContainsKey("buy") && wanted["buy"] == 1;

        //Generation des mercenaires
        foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
        {
            var index1 = buildStockIndex(type, MercenaryLevel.Recruit);
            var index2 = buildStockIndex(type, MercenaryLevel.Confirmed);
            var index3 = buildStockIndex(type, MercenaryLevel.Veteran);
            var index4 = buildStockIndex(type, MercenaryLevel.Elite);
            var index5 = buildStockIndex(type, MercenaryLevel.Cyborg);

            Pawn p;
            if (wanted[index1] != 0)
            {
                for (var i = 0; i != wanted[index1]; i++)
                {
                    p = Utils.generateMerc(type, MercenaryLevel.Recruit, wanted["gear"], wanted["weapon"],
                        wanted["gearColor"], defNameRace, purchased);
                    if (p != null)
                    {
                        mercs.Add(p);
                    }
                }
            }

            if (wanted[index2] != 0)
            {
                for (var i = 0; i != wanted[index2]; i++)
                {
                    p = Utils.generateMerc(type, MercenaryLevel.Confirmed, wanted["gear"], wanted["weapon"],
                        wanted["gearColor"], defNameRace, purchased);
                    if (p != null)
                    {
                        mercs.Add(p);
                    }
                }
            }

            if (wanted[index3] != 0)
            {
                for (var i = 0; i != wanted[index3]; i++)
                {
                    p = Utils.generateMerc(type, MercenaryLevel.Veteran, wanted["gear"], wanted["weapon"],
                        wanted["gearColor"], defNameRace, purchased);
                    if (p != null)
                    {
                        mercs.Add(p);
                    }
                }
            }

            if (wanted[index4] != 0)
            {
                for (var i = 0; i != wanted[index4]; i++)
                {
                    p = Utils.generateMerc(type, MercenaryLevel.Elite, wanted["gear"], wanted["weapon"],
                        wanted["gearColor"], defNameRace, purchased);
                    if (p != null)
                    {
                        mercs.Add(p);
                    }
                }
            }

            if (wanted[index5] == 0)
            {
                continue;
            }

            for (var i = 0; i != wanted[index5]; i++)
            {
                p = Utils.generateMerc(type, MercenaryLevel.Cyborg, wanted["gear"], wanted["weapon"],
                    wanted["gearColor"], defNameRace, purchased);
                if (p != null)
                {
                    mercs.Add(p);
                }
            }
        }

        Map map;

        //Obtaining the concerned map
        if (!wanted.TryGetValue("map", out var value))
        {
            map = Utils.getRandomMapOfPlayer();
        }
        else
        {
            map = Utils.getMapFromUID(value) ?? Utils.getRandomMapOfPlayer();
        }

        //Patch avec SOS2: Si c'est une map spatiale SOS2, on prendra une autre --Par Ionfrigate12345:
        if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
        {
            map = HarmonyUtils.GetPlayerMainColonyMap(true, false);
        }

        if (map == null) //Si on trouve toujours pas (le joueur n'a pas de map planétaire)
        {
            Log.Warning(
                "Cannot find a map to spawn mercenaries. The player seems to have only SOS2 space maps where the spawning is prevented.");
            return;
        }

        //IntVec3 dropCellNear = CellFinder.RandomEdgeCell(map);
        //IntVec3 dropCellNear = DropCellFinder.FindRaidDropCenterDistant(map);
        var dropCellNear = Utils.spawnMercOnMap(map, mercs);
        Find.LetterStack.ReceiveLetter("MFM_LetterMercArrival".Translate(), "MFM_LetterMercArrivalDesc".Translate(),
            LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, map));
        //DropPodUtility.DropThingsNear(dropCellNear, map, mercs.Cast<Thing>(), 100, false, false, false);
    }

    public static string buildStockIndex(MercenaryType type, MercenaryLevel level)
    {
        var index = ((int)type).ToString();
        switch (level)
        {
            case MercenaryLevel.Recruit:
                index += "nbRecruit";
                break;
            case MercenaryLevel.Confirmed:
                index += "nbConfirmed";
                break;
            case MercenaryLevel.Veteran:
                index += "nbVeteran";
                break;
            case MercenaryLevel.Elite:
                index += "nbElite";
                break;
            case MercenaryLevel.Cyborg:
                index += "nbCyborg";
                break;
        }

        return index;
    }


    /*
     * Obtaining quantity in stock
     */
    public int getNbInStock(MercenaryType type, MercenaryLevel level)
    {
        var index = buildStockIndex(type, level);
        return getNbInStock(index);
    }

    public int getNbInStock(string index)
    {
        return stock.GetValueOrDefault(index, 0);
    }

    /*
     * Stock update
     */
    public void setNbInStock(MercenaryType type, MercenaryLevel level, int val)
    {
        var index = buildStockIndex(type, level);
        if (stock.ContainsKey(index))
        {
            stock[index] = val;
        }
    }

    public static bool playerHaveMerc()
    {
        return Utils.getPlayerMercenaries().Count > 0;
    }

    public bool playerHaveRentedMerc()
    {
        return listerRentedMercenaries.Count > 0;
    }

    public bool playerHaveRentedSlaveMerc()
    {
        foreach (var m in listerRentedMercenaries)
        {
            if (Utils.isSS_Slave(m))
            {
                return true;
            }
        }

        return false;
    }

    public void copyStockStructure(Dictionary<string, int> dest)
    {
        foreach (var entry in stock)
        {
            dest[entry.Key] = 0;
        }
    }


    public void addPendingRendMercComeBack(int GT, List<Pawn> mercList, Map comeBackMap)
    {
        var lst = new List<string> { comeBackMap.uniqueID.ToString() };

        foreach (var m in mercList)
        {
            lst.Add(m.GetUniqueLoadID());
        }

        pendingRentedMercComeBack[GT] = Utils.serializeLS(lst);
    }

    /*
     * Addition of a deposit and stuff order
     */
    public void addPendingStuffAndGuarantee(int GT, List<Pawn> lst, Map backMap)
    {
        pendingStuffAndGuarantee.Add(GT, Utils.serializeLP(lst, backMap));
    }

    public void addPendingPowerBeam(int GT, Map map, IntVec3 pos)
    {
        var ok = false;

        while (!ok)
        {
            if (pendingPowerBeam.ContainsKey(GT))
            {
                GT += 1;
            }
            else
            {
                pendingPowerBeam.Add(GT, $"{map.GetUniqueLoadID()},{pos.x},{pos.y},{pos.z}");
                ok = true;
            }
        }
    }

    /*
     * Added timed mercenary command
     */
    public void addPendingMercOrder(int GT, Dictionary<string, int> wanted)
    {
        pendingMercOrder.Add(GT, Utils.serializeDSI(wanted));
    }

    public void addPendingMedievalCaravan(int GT, Dictionary<string, int> wanted, string text)
    {
        var ok = false;

        if (GT == 0)
        {
            GT = Find.TickManager.TicksGame + (Rand.Range(Settings.medievalModeMinHourDeliveryCaravan,
                Settings.medievalModeMaxHourDeliveryCaravan) * 2500);
        }

        while (!ok)
        {
            if (pendingMedievalDelivery.ContainsKey(GT))
            {
                GT += 1;
            }
            else
            {
                pendingMedievalDelivery.Add(GT, Utils.serializeDSI(wanted));
                pendingMedievalDeliveryText.Add(GT, text);
                ok = true;
            }
        }
    }

    public void removePendingMercOrder(int GT)
    {
        pendingMercOrder.Remove(GT);
    }

    public void addDiscount(string code, float v)
    {
        discounts.Add(code, v);
    }

    public void removeDiscount(string code)
    {
        discounts.Remove(code);
    }

    public bool getDiscount(string code, out float v)
    {
        v = 0.0f;
        if (!discounts.TryGetValue(code, out var discount))
        {
            return false;
        }

        v = discount;

        return true;
    }

    public Dictionary<int, string> getPendingMedievalDelivery()
    {
        return pendingMedievalDelivery;
    }

    public Dictionary<string, float> getDiscountList()
    {
        return discounts;
    }

    public Dictionary<int, string> getPendingMercOrder()
    {
        return pendingMercOrder;
    }

    public Dictionary<int, string> getPendingPowerBeam()
    {
        return pendingPowerBeam;
    }

    public Dictionary<int, string> getPendingStuffAndGuarantee()
    {
        return pendingStuffAndGuarantee;
    }

    public Dictionary<int, string> getPendingRendMercComeBack()
    {
        return pendingRentedMercComeBack;
    }

    public void pushRentedMercenary(Pawn merc)
    {
        listerRentedMercenaries.Add(merc);
    }

    public Pawn getRandomRentedMerc()
    {
        return listerRentedMercenaries.RandomElement();
    }

    public Pawn getRandomRentedSlaveMerc()
    {
        if (listerRentedMercenaries.Count == 0)
        {
            return null;
        }

        var ret = listerRentedMercenaries.Where(x => x != null && Utils.isSS_Slave(x));

        return !ret.Any() ? null : ret.RandomElement();
    }

    public Pawn getRandomMerc()
    {
        var listerMercenaries = Utils.getPlayerMercenaries();
        return !listerMercenaries.Any() ? null : listerMercenaries.RandomElement();
    }

    public void popRentedMercenary(Pawn merc)
    {
        listerRentedMercenaries.Remove(merc);
    }

    public static List<Pawn> getRandomRogueMerc()
    {
        var listerMercenaries = Utils.getPlayerMercenaries();
        if (listerMercenaries.Count < 1)
        {
            return null;
        }

        var ret = new List<Pawn>();
        var prop = Rand.Range(0.15f, 0.70f);
        var nb = (int)(listerMercenaries.Count * prop);
        if (nb == 0)
        {
            nb = 1;
        }

        var tmp = listerMercenaries.ToList();

        for (var i = 0; i != nb; i++)
        {
            var sel = tmp.RandomElement();
            ret.Add(sel);
            tmp.Remove(sel);
        }

        return ret;
    }

    public List<Pawn> getRentedMercenaries()
    {
        return listerRentedMercenaries;
    }

    public HashSet<Pawn> getRentedPawns()
    {
        return rentedPawns;
    }

    public void pushRentedPawn(Pawn pawn)
    {
        rentedPawns.Add(pawn);
    }

    public void popRentedPawn(Pawn pawn)
    {
        if (rentedPawns.Contains(pawn))
        {
            rentedPawns.Remove(pawn);
        }
    }

    private void reset()
    {
        stock.Clear();
        pendingMercOrder.Clear();
        listerRentedMercenaries.Clear();
        pendingRentedMercComeBack.Clear();
        pendingStuffAndGuarantee.Clear();
        pendingPowerBeam.Clear();
        discounts.Clear();
        rentedPawns.Clear();
        pendingMedievalDelivery.Clear();
        pendingMedievalDelivery.Clear();
        savedIAS.Clear();
    }

    private void initNull()
    {
        stock ??= new Dictionary<string, int>();

        pendingMercOrder ??= new Dictionary<int, string>();

        listerRentedMercenaries ??= [];

        pendingRentedMercComeBack ??= new Dictionary<int, string>();

        pendingStuffAndGuarantee ??= new Dictionary<int, string>();

        pendingPowerBeam ??= new Dictionary<int, string>();

        discounts ??= new Dictionary<string, float>();

        rentedPawns ??= [];

        pendingMedievalDelivery ??= new Dictionary<int, string>();

        pendingMedievalDeliveryText ??= new Dictionary<int, string>();

        savedIAS ??= [];
    }
}