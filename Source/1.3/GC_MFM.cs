using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.AI;
using UnityEngine;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class GC_MFM : GameComponent
    {

        public GC_MFM(Game game)
        {
            this.game = game;
            Utils.GCMFM = this;

            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();

            if (Utils.MEDIEVALTIMESLOADED)
            {
                data["MFM_IronAllianceBowman"] = new List<string> { "MedTimes_AppBody_Tunic", "MedTimes_AppBody_Trousers", "MedTimes_Socks_Tailored", "MedTimes_Helm_Wooden", "MedTimes_WoodenPlates", "MedTimes_Bracers_Wooden", "MedTimes_Boots_Wooden" };
                data["MFM_IronAllianceGuard"] = new List<string> { "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored", "MedTimes_Hauberk", "MedTimes_Gloves_Scaled", "MedTimes_Boots_Scaled", "MedTimes_Helm_Domed" };
                data["MFM_IronAllianceEliteGuard"] = new List<string> { "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored", "MedTimes_EncasedSteel", "MedTimes_Gauntlets_EncasedSteel", "MedTimes_Boots_EncasedSteel", "Apparel_ShieldBelt", "MedTimes_Helm_Domed" };
                data["MFM_IronAllianceEliteBowman"] = new List<string> { "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored", "MedTimes_Helm_Headwrap", "MedTimes_PlateJack", "MedTimes_Bracer_Archer", "MedTimes_Boots_Plated" };
                data["MFM_IronAllianceVillager"] = new List<string> { "MedTimes_AppBody_Tunic", "MedTimes_AppBody_Trousers", "MedTimes_Socks_Tailored", "MedTimes_Headgear_ArmingCap", "MedTimes_Gloves_Tailored", "MedTimes_Boots_Tailored" };
                data["MFM_IronAllianceTrader"] = data["MFM_IronAllianceVillager"];
                data["MFM_IronAllianceKnight"] = new List<string> { "MedTimes_AppBody_Tunic", "MedTimes_AppBody_TrousersStriped", "MedTimes_Socks_Tailored", "MedTimes_EncasedSteel", "MedTimes_Gauntlets_EncasedSteel", "MedTimes_Boots_EncasedSteel", "Apparel_ShieldBelt", "MedTimes_Helm_DeathMask" };
                data["MFM_IronAllianceLord"] = data["MFM_IronAllianceKnight"];
            }
            else
            {
                
                data["MFM_IronAllianceEliteBowman"] = new List<string> { "Apparel_PlateArmor"};
                data["MFM_IronAllianceEliteGuard"] = new List<string> { "Apparel_PlateArmor", "Apparel_SimpleHelmet" };
                data["MFM_IronAllianceKnight"] = new List<string> { "Apparel_PlateArmor", "Apparel_AdvancedHelmet" };
                data["MFM_IronAllianceLord"] = new List<string> { "Apparel_PlateArmor", "Apparel_AdvancedHelmet" };
            }
            foreach (var entry in data)
            {
                try
                {
                    PawnKindDef kd = DefDatabase<PawnKindDef>.GetNamedSilentFail(entry.Key);
                    if (kd == null)
                        continue;

                    kd.apparelRequired.Clear();
                    foreach (var el in entry.Value)
                    {
                        try
                        {
                            ThingDef td = DefDatabase<ThingDef>.GetNamedSilentFail(el);
                            if (td == null)
                                continue;
                            kd.apparelRequired.Add(td);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            //Application initiale des regles sur les vetements USFM
            Utils.refreshUSFMStuffRule();
            Utils.SS_slaveHediff = DefDatabase<HediffDef>.GetNamedSilentFail("Enslaved");
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                reset();
            }

            Scribe_Values.Look<string>(ref preferedRace, "preferedRace", "");
            Scribe_Collections.Look<Settlement>(ref this.savedIAS, "savedIAS", LookMode.Deep);
            Scribe_Values.Look<bool>(ref billInProgress, "billInProgress", false);
            Scribe_Values.Look<bool>(ref mercWantJoinInProgress, "mercWantJoinInProgress", false);
            Scribe_Collections.Look<Pawn>(ref this.rentedPawns, "rentedPawns", LookMode.Deep);
            Scribe_Values.Look<bool>(ref counterOfferInProgress, "counterOfferInProgress", false);
            Scribe_Values.Look<int>(ref GTFirstDiscount, "GTFirstDiscount", 0);
            Scribe_Collections.Look(ref discounts, "discounts", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref pendingMedievalDelivery, "pendingMedievalDelivery", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref pendingMedievalDeliveryText, "pendingMedievalDeliveryText", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref pendingStuffAndGuarantee, "pendingStuffAndGuarantee", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref pendingPowerBeam, "pendingPowerBeam", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref pendingMercOrder, "pendingMercOrder", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref pendingRentedMercComeBack, "pendingRentedMercComeBack", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref this.listerRentedMercenaries, "listerRentedMercenaries", LookMode.Reference);
            Scribe_Collections.Look(ref this.stock, "stock", LookMode.Value, LookMode.Value);
            Scribe_Values.Look<int>(ref stockExpireGT, "stockExpireGT", 0);
            Scribe_Values.Look<int>(ref lastQuad, "lastQuad", -1);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                initNull();

            //Suppression des références null 
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                listerRentedMercenaries.RemoveAll(item => item == null);
            }
        }

        public override void GameComponentOnGUI()
        {
            base.GameComponentOnGUI();

            

        }


        public override void GameComponentTick()
        {
            int GT = Find.TickManager.TicksGame;

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
            if(GT % 60000 == 0)
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

        void checkRentedMercEarnedXP()
        {
            int CGT = Find.TickManager.TicksGame;

            foreach(var m in listerRentedMercenaries)
            {
                if (m != null)
                {
                    //reevaluation experience gagné
                    Comp_USFM comp = m.TryGetComp<Comp_USFM>();
                    if (comp == null)
                        continue;

                    int diff = 0;

                    if (comp.xpEarnedLastCT != -1)
                        diff = (CGT - comp.xpEarnedLastCT);
                    else
                        comp.xpEarnedLastCT = CGT;

                    if (diff >= 2500)
                    {
                        int nbHours = diff / 2500;
                        //Log.Message("Increment XP of " + nbHours + " for " + m.LabelCap);
                        Utils.incMercSkillXP(m, nbHours);
                        comp.xpEarnedLastCT = CGT;
                    }
                }
            }
        }

        void checkRemoveIronAlliance()
        {
            factionIronAlliance = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("USFM_FactionAOS"));
            if (factionIronAlliance != null)
            {
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
                        foreach(var el in savedIAS)
                        {
                            //settlement.SetFaction(faction);
                            //settlement.Tile = num4;
                            //settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement, null);
                            if(!Find.WorldObjects.Contains(el))
                                Find.WorldObjects.Add(el);
                        }
                        factionIronAlliance.defeated = false;
                    }
                    savedIAS.Clear();
                    factionIronAlliance.def.hidden = false;
                }
            }
        }


        void checkLastQuadInit()
        {
            if (lastQuad == -1)
                lastQuad = (int)Utils.getCurrentQuad();
        }

        /*
         * Check de l'arrivage ou non de caravane de paiement de l'iron alliance
         */
        public void checkPendingMedievalDeliveryCaravan()
        {
            int CGT = Find.TickManager.TicksGame;
            List<int> toDel = null;
            foreach (var entry in pendingMedievalDelivery)
            {
                //Commande arrivé à échéance livraison des mercenaires
                if (entry.Key <= CGT)
                {
                    if (toDel == null)
                        toDel = new List<int>();
                    toDel.Add(entry.Key);

                    Dictionary<string, int> wanted = Utils.unserializeDSI(entry.Value);
                    if (wanted == null)
                        continue;

                    Map map = null;

                    //Obtention de la map concernée
                    if (wanted.ContainsKey("map"))
                        map = Find.CurrentMap;

                    map = Utils.getMapFromUID(wanted["map"]);
                    if (map == null)
                        map = Find.CurrentMap;

                    //Spawn caravane allant déposer la cargaison
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);
                    thing.stackCount = wanted["silver"];
                    List<Thing> toSpawn = new List<Thing>();
                    toSpawn.Add(thing);

                    //Spawn de la caravane est larguage de la tune
                    IntVec3 spawnedThings;
                    if (Utils.spawnMedievalCaravan(map, toSpawn, out spawnedThings))
                    {
                        //Affichage message souhaité
                        Find.LetterStack.ReceiveLetter("MFM_LetterArrivedMedievalCaravanDelivery".Translate(), pendingMedievalDeliveryText[entry.Key], LetterDefOf.PositiveEvent, new LookTargets(spawnedThings, map));
                    }
                }
            }

            if (toDel != null)
            {
                foreach (var entry in toDel)
                {
                    pendingMedievalDelivery.Remove(entry);
                    pendingMedievalDeliveryText.Remove(entry);
                }
            }
        }

        void checkGotFirstDiscount()
        {
            int CGT = Find.TickManager.TicksGame;

            if(GTFirstDiscount != - 1 && CGT > GTFirstDiscount)
            {
                GTFirstDiscount = -1;
                float discount = Rand.Range(Settings.minDiscount, Settings.maxDiscount);
                string code = Utils.generateDiscountCode((uint)CGT);
                addDiscount(code, discount);
                Find.LetterStack.ReceiveLetter("MFM_LetterDiscount".Translate(Utils.getUSFMLabel()), "MFM_LetterFirstDiscount".Translate((int)(discount * 100), code), LetterDefOf.PositiveEvent);
            }
        }

        void checkInitFirstDiscount()
        {
            if (GTFirstDiscount == 0)
                GTFirstDiscount = Rand.Range(18000, 360000);
        }

        /*
         * Check powerbeam attack
         */
        void checkPowerBeamAttack()
        {
            int CGT = Find.TickManager.TicksGame;
            List<int> toDel = null;
            foreach (var entry in pendingPowerBeam)
            {
                if (entry.Key <= CGT)
                {
                    if (toDel == null)
                        toDel = new List<int>();
                    toDel.Add(entry.Key);

                    Map map = null;
                    IntVec3 coord;
                    Utils.unserializePowerBeamAttack(entry.Value, out map, out coord);
                    if (map == null)
                        continue;

                    Utils.startPowerBeamAt(map, coord);

                    //msg = "MFM_LetterDepositRecoveryDesc".Translate(toDeliver.Count - 1, lst.Count, txt + "\n", guaranteeTotal);
                    //Find.LetterStack.ReceiveLetter("MFM_LetterDepositRecovery".Translate(), msg, LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, backMap));
                }
            }

            if (toDel != null)
            {
                foreach (var entry in toDel)
                {
                    pendingPowerBeam.Remove(entry);
                }
            }
        }

        /*
         * Check retour par dropPod des guaranties et stuff
         */
        void checkPendingStuffAndGuarantee()
        {
            if (pendingStuffAndGuarantee.Count == 0)
                return;

            int CGT = Find.TickManager.TicksGame;
            List<int> toDel = null;
            foreach (var entry in pendingStuffAndGuarantee)
            {
                //Commande arrivé à échéance retour des stuff et guarantee
                if (entry.Key <= CGT)
                {
                    if (toDel == null)
                        toDel = new List<int>();
                    toDel.Add(entry.Key);

                    Map backMap = null;
                    List<Pawn> lst = Utils.unserializeLP(entry.Value, out backMap);
                    if (lst == null)
                        continue;
                    if (backMap == null)
                        backMap = Utils.getRandomMapOfPlayer();

                    int guaranteeTotal = 0;
                    int val = 0;
                    string txt = "";
                    string msg = "";
                    int nbItems = 0;
                    List<Thing> toDeliver = new List<Thing>();
                    List<Thing> toDel2 = new List<Thing>();

                    //Calcul texte et montant guaranties + Déduction et comptage nombre de stuff à restituer
                    foreach (var p in lst)
                    {
                        Comp_USFM comp = p.TryGetComp<Comp_USFM>();
                        if (comp == null)
                            continue;

                        toDel2.Clear();
                        float newScore = Utils.getPawnScore(p);
                        //Le pawn à été abimé on va effectué un prorata de sa caution
                        if (comp.origScore > newScore)
                        {
                            val =  (int)(comp.guarantee * ((comp.origScore-newScore) /comp.origScore));
                            txt += "\n" + ("MFM_GuaranteeLineNOK".Translate(comp.guarantee - val, p.LabelCap, comp.guarantee, val ));
                            guaranteeTotal += comp.guarantee - val;
                        }
                        else
                        {
                            txt += "\n" + ("MFM_GuaranteeLineOK".Translate(comp.guarantee , p.LabelCap));
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
                                if (e.def.defName != "MFM_Apparel_USFMBasicShirt" && e.def.defName != "MFM_Apparel_USFMBasicShirtCivil")
                                {
                                    toDeliver.Add(e);
                                    toDel2.Add(e);
                                }
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
                            popRentedPawn(p);

                        Find.WorldPawns.PassToWorld(p, RimWorld.Planet.PawnDiscardDecideMode.Discard);
                    }

                    Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);
                    thing.stackCount = guaranteeTotal;
                    toDeliver.Add(thing);

                    IntVec3 dropCellNear = DropCellFinder.TradeDropSpot(backMap);

                    if (!dropCellNear.IsValid)
                    {
                        //dropCellNear = CellFinder.RandomEdgeCell(backMap);
                        dropCellNear = DropCellFinder.FindRaidDropCenterDistant(backMap);
                    }

                    //S'il y a des trucs à délivrer
                    if (toDeliver.Count != 0)
                    {
                        if(Utils.modernUSFM())
                            DropPodUtility.DropThingsNear(dropCellNear, backMap, toDeliver, 100, false, false, false);
                        else
                        {
                            Utils.spawnMedievalCaravan(backMap, toDeliver, out dropCellNear);
                        }
                    }

                    msg = "MFM_LetterDepositRecoveryDesc".Translate(toDeliver.Count-1, lst.Count, txt+"\n", guaranteeTotal);
                    Find.LetterStack.ReceiveLetter("MFM_LetterDepositRecovery".Translate(), msg, LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, backMap));
                }
            }

            if (toDel != null)
            {
                foreach (var entry in toDel)
                {
                    pendingStuffAndGuarantee.Remove(entry);
                }
            }
        }

        /*
         * Facture quadrum si mercenaire et 1er du mois
         */
        void checkQuadrumBill()
        {
            int cquad = (int)Utils.getCurrentQuad();
            

            //Si le premier du mois (lastQuad != cquad) Et il y a des éléments facturables (mercenaires acheté ou loué ou mercenaire en cours de livraison)
            if (cquad != lastQuad && (playerHaveMerc() || listerRentedMercenaries.Count != 0 || pendingMercOrder.Count != 0))
            {
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
                ChoiceLetter_Bill cl = (ChoiceLetter_Bill)LetterMaker.MakeLetter(DefDatabase<LetterDef>.GetNamed("MFM_CLBill"));
                cl.init();
                cl.StartTimeout(duration: 60000);
                cl.label = "MFM_Bill".Translate(Utils.getUSFMLabel());
                Find.LetterStack.ReceiveLetter(@let: cl);
            }
        }

        /*
         * Check de l'arrivage ou non de mercenaire faisant suite à une commande
         */
        public void checkPendingMercOrder()
        {
            int CGT = Find.TickManager.TicksGame;
            List<int> toDel = null;
            foreach(var entry in pendingMercOrder)
            {
                //Commande arrivé à échéance livraison des mercenaires
                if(entry.Key <= CGT)
                {
                    if (toDel == null)
                        toDel = new List<int>();
                    toDel.Add(entry.Key);

                    Dictionary<string, int> wanted = Utils.unserializeDSI(entry.Value);
                    if (wanted == null)
                        continue;

                    Map map = null;

                    //Obtention de la map concernée
                    if (wanted.ContainsKey("map"))
                        map = Find.CurrentMap;

                    map = Utils.getMapFromUID(wanted["map"]);
                    if(map == null)
                        map = Find.CurrentMap;

                    spawnOrderedMercs(wanted);
                }
            }

            if(toDel != null)
            {
                foreach(var entry in toDel)
                {
                    pendingMercOrder.Remove(entry);
                }
            }
        }


        public bool mercIsInPendingRentedMercComeBack(string mercUID)
        {
            foreach (var entry in pendingRentedMercComeBack)
            {
                List<string> data = Utils.unserializeLS(entry.Value);
                if (data == null)
                    continue;

                foreach (var d in data)
                {
                    if (d == mercUID)
                        return true;
                }
            }

            return false;
        }

        /*
         * Check du retour de mercenaires du joueur
         */
        private void checkPendingRentedMercComeBack()
        {
            int CGT = Find.TickManager.TicksGame;
            List<int> toDel = null;
            foreach (var entry in pendingRentedMercComeBack)
            {
                //Commande arrivé à échéance livraison des mercenaires
                if (entry.Key <= CGT)
                {
                    if (toDel == null)
                        toDel = new List<int>();
                    toDel.Add(entry.Key);

                    List<string> data = Utils.unserializeLS(entry.Value);
                    if (data == null)
                        continue;

                    Map map = null;
                    //Obtention de la map concernée
                    if (data.Count == 0)
                        map = Utils.getRandomMapOfPlayer();
                    else
                    {
                        Int32.TryParse(data[0], out int val);
                        map = Utils.getMapFromUID(val);
                    }

                    if (map == null)
                        map = Find.CurrentMap;

                    //IntVec3 dropCellNear = CellFinder.RandomEdgeCell(map);
                    IntVec3 dropCellNear = DropCellFinder.FindRaidDropCenterDistant(map);

                    //Spawn par dropPod des mercenaire au player mentionnés
                    List<Pawn> mercs = new List<Pawn>();
                    foreach (var d in data)
                    {
                        foreach(var m in listerRentedMercenaries)
                        {
                            if (m == null)
                                continue;

                            if (m.GetUniqueLoadID() == d)
                            {
                                if (rentedPawns.Contains(m))
                                {
                                    rentedPawns.Remove(m);
                                    //Find.WorldPawns.PassToWorld(m, RimWorld.Planet.PawnDiscardDecideMode.KeepForever);
                                }
                                if (Find.WorldPawns.Contains(m))
                                    Find.WorldPawns.RemovePawn(m);

                                m.SetFactionDirect(Faction.OfPlayer);
                                if (m.holdingOwner != null)
                                {
                                    m.holdingOwner.Remove(m);
                                    m.holdingOwner = null;
                                }
                                mercs.Add(m);
                            }
                        }
                    }

                    //Suppression des mercenaires devant revenir de la liste des rented mercenaries
                    if(mercs.Count > 0)
                    {
                        foreach(var m in mercs)
                        {
                            listerRentedMercenaries.Remove(m);
                        }

                        //spawn effectif des mercenaires par dropPods

                        dropCellNear = Utils.spawnMercOnMap(map, mercs);
                        if(dropCellNear.IsValid)
                            Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercArrival".Translate(), "MFM_LetterRentedMercArrivalDesc".Translate(Utils.getUSFMLabel()), LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, map));
                    }
                }
            }

            if (toDel != null)
            {
                foreach (var entry in toDel)
                {
                    pendingRentedMercComeBack.Remove(entry);
                }
            }
        }

        /*
         * Check si les offres de l'USFM doivent être renouvellées
         */
        public void checkForRenewUSFMOffers()
        {
            int CGT = Find.TickManager.TicksGame;

            //Renouvellement si expiration des offres atteintes 
            if(CGT > stockExpireGT || stock.Count == 0)
            {
                int nbRecruitMin, nbRecruitMax;
                int nbConfirmedMin, nbConfirmedMax;
                int nbVeteranMin, nbVeteranMax;
                int nbEliteMin, nbEliteMax;
                int nbCyborgMin, nbCyborgMax;

                foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
                {
                    string indexBase = ((int)type).ToString();

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

                    stock[indexBase+"nbRecruit"] = Rand.Range(nbRecruitMin, nbRecruitMax);
                    stock[indexBase+"nbConfirmed"] = Rand.Range(nbConfirmedMin, nbConfirmedMax);
                    stock[indexBase+"nbVeteran"] = Rand.Range(nbVeteranMin, nbVeteranMax);
                    stock[indexBase+"nbElite"] = Rand.Range(nbEliteMin, nbEliteMax);
                    stock[indexBase + "nbCyborg"] = Rand.Range(nbCyborgMin, nbCyborgMax);
                }

                if(stockExpireGT != 0)
                {

                    Find.LetterStack.ReceiveLetter("MFM_LetterUSFMRenewStock".Translate(Utils.getUSFMLabel()), "MFM_LetterUSFMRenewStockDesc".Translate(Utils.getUSFMLabel()), LetterDefOf.PositiveEvent);
                }
                //Report renouvellement stock de mercenaires
                stockExpireGT = CGT + Settings.timeBeforeRenewOffers;
            }
        }

        /*
         * Spawn des mercenaires commandés
         */
        public void spawnOrderedMercs(Dictionary<string, int> wanted)
        {
            List<Pawn> mercs = new List<Pawn>();
            Pawn p = null;

            //Déduction race le cas echeant
            string defNameRace = Utils.getFirstKeyStartingWith(wanted, "race_");
            if(defNameRace != null)
            {
                defNameRace = defNameRace.ReplaceFirst("race_", "");
            }

            //Check si mercenaires achetés 
            bool purchased = false;
            if (wanted.ContainsKey("buy") && wanted["buy"] == 1)
                purchased = true;

            //Generation des mercenaires
            foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
            {
                string index1 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Recruit);
                string index2 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Confirmed);
                string index3 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Veteran);
                string index4 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Elite);
                string index5 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Cyborg);

                if (wanted[index1] != 0)
                {
                    for (int i = 0; i != wanted[index1]; i++)
                    {
                        p = Utils.generateMerc(type, MercenaryLevel.Recruit, wanted["gear"], wanted["weapon"],wanted["gearColor"], defNameRace, purchased);
                        if(p != null)
                            mercs.Add(p);
                    }
                }
                if (wanted[index2] != 0)
                {
                    for (int i = 0; i != wanted[index2]; i++)
                    {
                        p = Utils.generateMerc(type, MercenaryLevel.Confirmed, wanted["gear"], wanted["weapon"], wanted["gearColor"], defNameRace, purchased);
                        if(p != null)
                            mercs.Add(p);
                    }
                }
                if (wanted[index3] != 0)
                {
                    for (int i = 0; i != wanted[index3]; i++)
                    {
                        p = Utils.generateMerc(type, MercenaryLevel.Veteran, wanted["gear"], wanted["weapon"], wanted["gearColor"], defNameRace, purchased);
                        if(p != null)
                            mercs.Add(p);
                    }
                }
                if (wanted[index4] != 0)
                {
                    for (int i = 0; i != wanted[index4]; i++)
                    {
                        p = Utils.generateMerc(type, MercenaryLevel.Elite, wanted["gear"], wanted["weapon"], wanted["gearColor"], defNameRace, purchased);
                        if(p != null)
                            mercs.Add(p);
                    }
                }
                if (wanted[index5] != 0)
                {
                    for (int i = 0; i != wanted[index5]; i++)
                    {
                        p = Utils.generateMerc(type, MercenaryLevel.Cyborg, wanted["gear"], wanted["weapon"], wanted["gearColor"], defNameRace, purchased);
                        if (p != null)
                            mercs.Add(p);
                    }
                }
            }

            Map map = null;

            //Obtaining the concerned map
            if (!wanted.ContainsKey("map"))
                map = Utils.getRandomMapOfPlayer();
            else
            {
                map = Utils.getMapFromUID(wanted["map"]);
                if (map == null)
                    map = Utils.getRandomMapOfPlayer();
            }

            //IntVec3 dropCellNear = CellFinder.RandomEdgeCell(map);
            //IntVec3 dropCellNear = DropCellFinder.FindRaidDropCenterDistant(map);
            IntVec3 dropCellNear = Utils.spawnMercOnMap(map, mercs);
            Find.LetterStack.ReceiveLetter("MFM_LetterMercArrival".Translate(), "MFM_LetterMercArrivalDesc".Translate(), LetterDefOf.PositiveEvent, new LookTargets(dropCellNear, map));
            //DropPodUtility.DropThingsNear(dropCellNear, map, mercs.Cast<Thing>(), 100, false, false, false);
        }

        public string buildStockIndex(MercenaryType type, MercenaryLevel level)
        {
            string index = ((int)type).ToString();
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
            string index = buildStockIndex(type, level);
            return getNbInStock(index);
        }

        public int getNbInStock(string index)
        {
            if (stock.ContainsKey(index))
            {
                return stock[index];
            }
            else
            {
                return 0;
            }
        }

        /*
         * Stock update
         */
        public void setNbInStock(MercenaryType type, MercenaryLevel level, int val)
        {
            string index = buildStockIndex(type, level);
            if (stock.ContainsKey(index))
            {
                stock[index] = val;
            }
        }

        public bool playerHaveMerc()
        {
            return Utils.getPlayerMercenaries().Count > 0;
        }

        public bool playerHaveRentedMerc()
        {
            return listerRentedMercenaries.Count > 0;
        }

        public bool playerHaveRentedSlaveMerc()
        {
            foreach(var m in listerRentedMercenaries)
            {
                if (Utils.isSS_Slave(m))
                    return true;
            }

            return false;
        }

        public void copyStockStructure(Dictionary<string, int> dest)
        {
            foreach(var entry in stock)
            {
                dest[entry.Key] = 0;
            }
        }


        public void addPendingRendMercComeBack(int GT,List<Pawn> mercList, Map comeBackMap)
        {
            List<string> lst = new List<string>();
            lst.Add(comeBackMap.uniqueID.ToString());

            foreach(var m in mercList)
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
            pendingStuffAndGuarantee.Add(GT, Utils.serializeLP(lst,backMap));
        }

        public void addPendingPowerBeam(int GT, Map map, IntVec3 pos)
        {
            bool ok = false;

            while (!ok)
            {
                if (pendingPowerBeam.ContainsKey(GT))
                    GT += 1;
                else
                {
                    pendingPowerBeam.Add(GT, map.GetUniqueLoadID() + "," + pos.x + "," + pos.y + "," + pos.z);
                    ok = true;
                }
            }
        }

        /*
         * Added timed mercenary command
         */
        public void addPendingMercOrder(int GT, Dictionary<string,int> wanted)
        {
            pendingMercOrder.Add(GT,Utils.serializeDSI(wanted));
        }

        public void addPendingMedievalCaravan(int GT, Dictionary<string, int> wanted, string text)
        {
            bool ok = false;

            if(GT == 0)
            {
                GT = Find.TickManager.TicksGame + (Rand.Range(Settings.medievalModeMinHourDeliveryCaravan, Settings.medievalModeMaxHourDeliveryCaravan)*2500);
            }

            while (!ok)
            {
                if (pendingMedievalDelivery.ContainsKey(GT))
                    GT += 1;
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
            if(pendingMercOrder.ContainsKey(GT))
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
            if (!discounts.ContainsKey(code))
                return false;
            else
                v = discounts[code];

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
                return null;

            IEnumerable<Pawn> ret = listerRentedMercenaries.Where(delegate (Pawn x)
            {
                return x!= null && Utils.isSS_Slave(x);
            });

            if (ret == null || ret.Count() == 0)
                return null;

            return ret.RandomElement();
        }

        public Pawn getRandomMerc()
        {
            List<Pawn> listerMercenaries = Utils.getPlayerMercenaries();
            if (listerMercenaries.Count() == 0)
                return null;

            return listerMercenaries.RandomElement();
        }

        public void popRentedMercenary(Pawn merc)
        {
            listerRentedMercenaries.Remove(merc);
        }

        public List<Pawn> getRandomRogueMerc()
        {
            List<Pawn> listerMercenaries = Utils.getPlayerMercenaries();
            if (listerMercenaries.Count < 1)
                return null;

            List<Pawn> ret = new List<Pawn>();
            float prop = Rand.Range(0.15f, 0.70f);
            int nb = (int) (listerMercenaries.Count * prop);
            if (nb == 0)
                nb = 1;

            List<Pawn> tmp = listerMercenaries.ToList();

            for(int i = 0; i != nb; i++)
            {
                Pawn sel = tmp.RandomElement();
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
                rentedPawns.Remove(pawn);
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
            if (stock == null)
                stock = new Dictionary<string,int>();
            if (pendingMercOrder == null)
                pendingMercOrder = new Dictionary<int, string>();
            if (listerRentedMercenaries == null)
                listerRentedMercenaries = new List<Pawn>();
            if (pendingRentedMercComeBack == null)
                pendingRentedMercComeBack = new Dictionary<int, string>();
            if (pendingStuffAndGuarantee == null)
                pendingStuffAndGuarantee = new Dictionary<int, string>();
            if (pendingPowerBeam == null)
                pendingPowerBeam = new Dictionary<int, string>();
            if (discounts == null)
                discounts = new Dictionary<string, float>();
            if (rentedPawns == null)
                rentedPawns = new HashSet<Pawn>();
            if (pendingMedievalDelivery == null)
                pendingMedievalDelivery = new Dictionary<int, string>();
            if (pendingMedievalDeliveryText == null)
                pendingMedievalDeliveryText = new Dictionary<int, string>();
            if (savedIAS == null)
                savedIAS = new List<Settlement>();
        }

        public bool CounterOfferInProgress
        {
            get
            {
                return counterOfferInProgress;
            }

            set
            {
                counterOfferInProgress = value;
            }
        }

        public bool MercWantJoinInProgress
        {
            get
            {
                return mercWantJoinInProgress;
            }

            set
            {
                mercWantJoinInProgress = value;
            }
        }

        public bool BillInProgress
        {
            get
            {
                return billInProgress;
            }

            set
            {
                billInProgress = value;
            }
        }

        public Faction factionIronAlliance = null;

        private List<Settlement> savedIAS = new List<Settlement>();

        public string preferedRace = "";

        private bool billInProgress = false;
        private bool mercWantJoinInProgress = false;
        private bool counterOfferInProgress = false;
        private int GTFirstDiscount = 0;
        //Discounts 
        private Dictionary<string, float> discounts = new Dictionary<string, float>();
        //Powerbeam attack launched by USFM
        private Dictionary<int, string> pendingPowerBeam = new Dictionary<int, string>();
        //Return of deposit and stuff in progress after mercenaries leave
        private Dictionary<int, string> pendingStuffAndGuarantee = new Dictionary<int, string>();
        //Mercenary order being delivered
        private Dictionary<int, string> pendingMercOrder = new Dictionary<int, string>();
        //Quantity available for the moment of mercenaries
        private Dictionary<string,int> stock = new Dictionary<string,int>();
        //Medieval delivery caravan
        private Dictionary<int, string> pendingMedievalDelivery = new Dictionary<int, string>();
        private Dictionary<int, string> pendingMedievalDeliveryText = new Dictionary<int, string>();
        private int stockExpireGT = 0;
        private int lastQuad = -1;

        private HashSet<Pawn> rentedPawns = new HashSet<Pawn>();

        //Mercenaries of the players to return to the colony
        private Dictionary<int, string> pendingRentedMercComeBack = new Dictionary<int, string>();
        private List<Pawn> listerRentedMercenaries = new List<Pawn>();
        private Game game;
    }
}