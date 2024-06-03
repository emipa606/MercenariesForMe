using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using Verse.AI.Group;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    static class Utils
    {
        static public Mod currentModInst;

        static public HediffDef SS_slaveHediff = null;
        static public GC_MFM GCMFM;
        static public bool EVOLOADED = false;
        static public bool CONNLOADED = false;
        static public bool RSBELOADED = false;
        static public bool EPOELOADED = false;
        static public bool CELOADED = false;
        static public bool MSELOADED = false;
        static public bool MEDIEVALTIMESLOADED = false;
        static public bool GFMLOADED = false;

        static public readonly List<string> intMedievalStuffException = new List<string>() { "Apparel_BasicShirt", "Apparel_Pants", "Apparel_FlakJacket", "Apparel_FlakPants" };
        static public readonly List<string> intBlacklistedWeapons = new List<string>() { "TornadoGenerator", "OrbitalTargeterPowerBeam", "OrbitalTargeterBombardment", "LS_CloudSword","LS_Nightfall", "LS_Executioner", "LS_Deathbringer", "LS_RedQueen" };
        const string ALPHABET = "AG8FOLE2WVTCPY5ZH3NIUDBXSMQK7946";
        static public readonly List<string> workTypeDefsToNotClear = new List<string> { "Firefighter", "Patient", "PatientBedRest", "BasicWorker", "Hauling", "Cleaning" };
        static public readonly string SEP = " : ";
        static public readonly string[] types = new string[] { "ranged", "melee", "artist", "builder", "cooker", "farmer", "medical", "miner", "scientist", "speak", "tech", "trainer", "carry", "clean" };
        public static string TranslateTicksToTextIRLSeconds(int ticks)
        {
            //If less than one hour ingame then display seconds
            if (ticks < 2500)
                return ticks.ToStringSecondsFromTicks();
            else
                return ticks.ToStringTicksToPeriodVerbose(true);
        }

        public static Material getMercenaryIcon(MercenaryType type)
        {
            switch (type)
            {
                case MercenaryType.Artist:
                    return Tex.artist;
                case MercenaryType.Builder:
                    return Tex.builder;
                case MercenaryType.Cooker:
                    return Tex.cooker;
                case MercenaryType.Farmer:
                    return Tex.farmer;
                case MercenaryType.Medical:
                    return Tex.medical;
                case MercenaryType.Melee:
                    return Tex.melee;
                case MercenaryType.Ranged:
                    return Tex.ranged;
                case MercenaryType.Scientist:
                    return Tex.scientist;
                case MercenaryType.Miner:
                    return Tex.miner;
                case MercenaryType.Speaker:
                    return Tex.speak;
                case MercenaryType.Tech:
                    return Tex.tech;
                case MercenaryType.Trainer:
                    return Tex.trainer;
                default:
                    int v = (int)type;
                    if ( v == -1)
                        return Tex.ironAlliance;
                    else if(v == -2)
                        return Tex.ironAllianceTrader;
                    return null;
            }
        }

        public static WorkTags getTypeWorkTag(MercenaryType type)
        {
            switch (type)
            {
                case MercenaryType.Artist:
                    return WorkTags.Artistic;
                case MercenaryType.Builder:
                    return WorkTags.ManualSkilled;
                case MercenaryType.Cooker:
                    return WorkTags.Cooking;
                case MercenaryType.Farmer:
                    return WorkTags.PlantWork;
                case MercenaryType.Medical:
                    return WorkTags.Caring;
                case MercenaryType.Melee:
                    return WorkTags.Violent;
                case MercenaryType.Ranged:
                    return WorkTags.Violent;
                case MercenaryType.Scientist:
                    return WorkTags.Intellectual;
                case MercenaryType.Miner:
                    return WorkTags.Mining;
                case MercenaryType.Speaker:
                    return WorkTags.Social;
                case MercenaryType.Tech:
                    return WorkTags.Crafting;
                case MercenaryType.Trainer:
                    return WorkTags.Animals;
                default:
                    return WorkTags.Violent;
            }
        }

        public static Texture2D getMercenaryCategoryCover(MercenaryType type)
        {
            switch (type)
            {
                case MercenaryType.Artist:
                    return Tex.catArtist;
                case MercenaryType.Builder:
                    return Tex.catBuilder;
                case MercenaryType.Cooker:
                    return Tex.catCooker;
                case MercenaryType.Farmer:
                    return Tex.catFarmer;
                case MercenaryType.Medical:
                    return Tex.catMedical;
                case MercenaryType.Melee:
                    return Tex.catSoldier;
                case MercenaryType.Ranged:
                    return Tex.catSoldier2;
                case MercenaryType.Scientist:
                    return Tex.catScientist;
                case MercenaryType.Miner:
                    return Tex.catMiner;
                case MercenaryType.Speaker:
                    return Tex.catSpeak;
                case MercenaryType.Tech:
                    return Tex.catTech;
                case MercenaryType.Trainer:
                    return Tex.catTrainer;
                default:
                    return null;
            }
        }

        public static string getReadableType(MercenaryType type)
        {
            switch (type)
            {
                case MercenaryType.Artist:
                    return "MFM_Artist".Translate();
                case MercenaryType.Builder:
                    return "MFM_Builder".Translate();
                case MercenaryType.Cooker:
                    return "MFM_Cooker".Translate();
                case MercenaryType.Farmer:
                    return "MFM_Farmer".Translate();
                case MercenaryType.Medical:
                    return "MFM_Medical".Translate();
                case MercenaryType.Melee:
                    return "MFM_Melee".Translate();
                case MercenaryType.Ranged:
                    return "MFM_Ranged".Translate();
                case MercenaryType.Scientist:
                    return "MFM_Scientist".Translate();
                case MercenaryType.Miner:
                    return "MFM_Miner".Translate();
                case MercenaryType.Speaker:
                    return "MFM_Speak".Translate();
                case MercenaryType.Tech:
                    return "MFM_Tech".Translate();
                case MercenaryType.Trainer:
                    return "MFM_Trainer".Translate();
                default:
                    return "";
            }
        }

        public static string getReadableLevel(MercenaryLevel level)
        {
            switch (level)
            {
                case MercenaryLevel.Recruit:
                    return "MFM_Recruit".Translate();
                case MercenaryLevel.Confirmed:
                    return "MFM_Confirmed".Translate();
                case MercenaryLevel.Veteran:
                    return "MFM_Veteran".Translate();
                case MercenaryLevel.Elite:
                    return "MFM_Elite".Translate();
                case MercenaryLevel.Cyborg:
                    return "MFM_Cyborg".Translate();
                default:
                    return "";
            }
        }

        public static MercenaryLevel getLevelFromSkill(Pawn pawn, MercenaryType type)
        {
            SkillRecord skill = getAssociatedSkill(pawn, type);
            

            if (skill != null)
            {
                if (skill.Level <= 6)
                    return MercenaryLevel.Recruit;
                else if (skill.Level <= 10)
                    return MercenaryLevel.Confirmed;
                else if (skill.Level <= 15)
                    return MercenaryLevel.Veteran;
                else if(skill.Level <= 18 || !Utils.modernUSFM())
                    return MercenaryLevel.Elite;
                else
                    return MercenaryLevel.Cyborg;
            }
            else
                return MercenaryLevel.Recruit;
        }

        /*
         * Obtaining the skill concerned
         */
        public static SkillRecord getAssociatedSkill(Pawn pawn, MercenaryType type)
        {
            SkillRecord skill = null;

            if (pawn.skills == null)
                return null;

            switch (type)
            {
                case MercenaryType.Artist:
                    skill = pawn.skills.GetSkill(SkillDefOf.Artistic);
                    break;
                case MercenaryType.Builder:
                    skill = pawn.skills.GetSkill(SkillDefOf.Construction);
                    break;
                case MercenaryType.Cooker:
                    skill = pawn.skills.GetSkill(SkillDefOf.Cooking);
                    break;
                case MercenaryType.Farmer:
                    skill = pawn.skills.GetSkill(SkillDefOf.Plants);
                    break;
                case MercenaryType.Medical:
                    skill = pawn.skills.GetSkill(SkillDefOf.Medicine);
                    break;
                case MercenaryType.Melee:
                    skill = pawn.skills.GetSkill(SkillDefOf.Melee);
                    break;
                case MercenaryType.Ranged:
                    skill = pawn.skills.GetSkill(SkillDefOf.Shooting);
                    break;
                case MercenaryType.Miner:
                    skill = pawn.skills.GetSkill(SkillDefOf.Mining);
                    break;
                case MercenaryType.Scientist:
                    skill = pawn.skills.GetSkill(SkillDefOf.Intellectual);
                    break;
                case MercenaryType.Speaker:
                    skill = pawn.skills.GetSkill(SkillDefOf.Social);
                    break;
                case MercenaryType.Tech:
                    skill = pawn.skills.GetSkill(SkillDefOf.Crafting);
                    break;
                case MercenaryType.Trainer:
                    skill = pawn.skills.GetSkill(SkillDefOf.Animals);
                    break;
            }

            return skill;
        }

        /*
         * Generation of a mercenary pawn of type and level mentioned in parameter
         */
        public static Pawn generateMerc(MercenaryType type, MercenaryLevel level, int gear, int weapon, int intGearColor, string defNameRace=null, bool purchased=false)
         {
            PawnKindDef pawnKindDef = null;

            if(defNameRace != null)
            {
                pawnKindDef = DefDatabase<PawnKindDef>.GetNamed(defNameRace);
            }

            //We randomly draw a pawnkindDef for the mercenary 
            try
            {
                if (defNameRace == null || pawnKindDef == null)
                {
                    pawnKindDef = (from d in DefDatabase<PawnKindDef>.AllDefs
                                   where d != null && d.race != null && d.RaceProps != null && d.RaceProps.Humanlike
                                   && !Settings.blacklistedPawnKind.Contains(d.defName)
                                   select d).RandomElement<PawnKindDef>();
                }
            }
            catch (Exception)
            {

            }

            if (pawnKindDef == null)
                pawnKindDef = PawnKindDefOf.AncientSoldier;

            //Pawn p = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfPlayer);
            PawnGenerationRequest pgr = new PawnGenerationRequest(pawnKindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false, false, false, false, true,1, true, false, true, true, false, false, false, false, false, 0f, 0f, null);
            //prevent royalty titles
            pgr.ForbidAnyTitle = true;
            //prevent ideo for mercenaries
            pgr.ForceNoIdeo = true;
            
            Pawn p = PawnGenerator.GeneratePawn(pgr);

            int cAge = (int)(p.ageTracker.AgeChronologicalTicks / 3600000f);
            if (cAge < Settings.minAge || cAge >= Settings.maxAge)
            {
                p.ageTracker.AgeBiologicalTicks = (long)(Rand.Range(Settings.minAge, Settings.maxAge) * 3600000f);
            }

            if (p == null)
                return null;
            //p.workSettings.EnableAndInitialize();

            /**************************************** Traits ***************************************************/
            //Removal of traits that do not comply with mercenary conditions (pyromaniac, nudist, ...)
            List<Trait> traitsToDel = null;
            bool contradictedTrait = false;
            foreach(var s in p.story.traits.allTraits)
            {
                bool ok2 = ((s.def.defName == "BodyPuristDisgust" || s.def.defName == "BodyPurist") && level == MercenaryLevel.Cyborg)
                    || (s.def.defName == "Brawler" && type == MercenaryType.Ranged);

                if (ok2)
                    contradictedTrait = true;

                if (Settings.blacklistedTraitsV2.Contains(s.def.DataAtDegree(s.Degree).untranslatedLabel)
                    || ok2)
                {
                    if (traitsToDel == null)
                        traitsToDel = new List<Trait>();
                    traitsToDel.Add(s);
                }
            }

            if (traitsToDel != null)
            {
                foreach (var t in traitsToDel)
                {
                    p.story.traits.allTraits.Remove(t);
                }
            }

            //We remove the headiffs by default
            p.health.capacities.Clear();
            p.health.summaryHealth.Notify_HealthChanged();
            p.health.surgeryBills.Clear();
            p.health.immunity = new ImmunityHandler(p);

            //Removal of serious hediffs (bads back, cataracts, ...)
            if (Settings.hediffToNotClear != null && p.health.hediffSet != null)
            {
                List<HediffWithComps> lst = new List<HediffWithComps>();
                p.health.hediffSet.GetHediffs<HediffWithComps>(ref lst);
                foreach (var h in lst)
                {
                    if (!Settings.hediffToNotClear.Contains(h.def.defName))
                        p.health.hediffSet.hediffs.Remove(h);
                }
            }


            Comp_USFM compUSFM = p.TryGetComp<Comp_USFM>();
            if (!purchased)
            {
                compUSFM.isMercenary = true;
                compUSFM.type = type;
                compUSFM.hiredByPlayer = true;
            }

            setHiredMercSalary(p);

            switch (level)
            {
                case MercenaryLevel.Recruit:
                    compUSFM.guarantee = Settings.guaranteeRecruit;
                    break;
                case MercenaryLevel.Confirmed:
                    compUSFM.guarantee = Settings.guaranteeConfirmed;
                    break;
                case MercenaryLevel.Veteran:
                    compUSFM.guarantee = Settings.guaranteeVeteran;
                    break;
                case MercenaryLevel.Elite:
                    compUSFM.guarantee = Settings.guaranteeElite;
                    break;
                case MercenaryLevel.Cyborg:
                    compUSFM.guarantee = Settings.guaranteeCyborg;
                    //Cyborg, we're going to replace all the parts of her body with prostheses
                    List<BodyPartDef> parts = new List<BodyPartDef>();
                    //List<Hediff> addedHeddiffs = new List<Hediff>();

                    List<BodyPartRecord> allParts = p.def.race.body.AllParts;
                    int bionicSource = Settings.getBionicSource();
                    for (int i = 0; i < allParts.Count; i++)
                    {
                        BodyPartRecord bp = allParts[i];
                        HediffDef h = null;
                        switch (bionicSource) {
                            //VANILLA
                            case 0:
                                if (bp.def == BodyPartDefOf.Eye)
                                    h = DefDatabase<HediffDef>.GetNamed("ArchotechEye");
                                else if (bp.def == BodyPartDefOf.Heart && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("BionicHeart");
                                else if (bp.def == BodyPartDefOf.Arm)
                                    h = DefDatabase<HediffDef>.GetNamed("ArchotechArm");
                                else if (bp.def == BodyPartDefOf.Leg)
                                    h = DefDatabase<HediffDef>.GetNamed("ArchotechLeg");
                                else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("BionicStomach");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                                    h = DefDatabase<HediffDef>.GetNamed("BionicEar");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                                    h = DefDatabase<HediffDef>.GetNamed("BionicSpine");
                                break;
                                //EPOE
                            case 1:
                                if (bp.def == BodyPartDefOf.Eye)
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicEye");
                                else if (bp.def == BodyPartDefOf.Heart && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticHeart");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Kidney") && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticKidney");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Liver") && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticLiver");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticLung");
                                else if (bp.def == BodyPartDefOf.Arm)
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedPowerArm");
                                else if (bp.def == BodyPartDefOf.Leg)
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicLeg");
                                else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticStomach");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicEar");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicSpine");
                                break;
                             //RSBE
                            case 2:
                                if (bp.def == BodyPartDefOf.Eye)
                                    h = DefDatabase<HediffDef>.GetNamed("ArchotechEye");
                                else if (bp.def == BodyPartDefOf.Heart && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticHeart");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Kidney") && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticKidney");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Liver") && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticLiver");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticLung");
                                else if (bp.def == BodyPartDefOf.Arm)
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedPowerArm");
                                else if (bp.def == BodyPartDefOf.Leg)
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicLeg");
                                else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("SyntheticStomach");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicEar");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                                    h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicSpine");
                                else if (bp.def == BodyPartDefOf.Torso)
                                    h = DefDatabase<HediffDef>.GetNamed("ExoskeletonSuit");
                                break;
                                //CONN
                            case 3:
                                if (bp.def != null && bp.def.tags.Contains(BodyPartTagDefOf.ConsciousnessSource))
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_CombatAI"); 
                                else if (bp.def == BodyPartDefOf.Eye)
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_CyclopsVisor");
                                else if (bp.def == BodyPartDefOf.Heart && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_Lionheart");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_Shadowrunner");
                                else if (bp.def == BodyPartDefOf.Arm)
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_PowerArms");
                                else if (bp.def == BodyPartDefOf.Leg)
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_ElasticAchilles");
                                else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !Utils.MSELOADED)
                                    h = DefDatabase<HediffDef>.GetNamed("BionicStomach");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_SensoricEarside");
                                else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                                    h = DefDatabase<HediffDef>.GetNamed("BionicSpine");
                                else if (bp.def == BodyPartDefOf.Torso)
                                    h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_ExoskeletonArmor");
                                break;
                                //Evolved Organs :
                                case 4:
                                    if (bp.def != null && bp.def.tags.Contains(BodyPartTagDefOf.ConsciousnessSource))
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedBrain");
                                    else if (bp.def == BodyPartDefOf.Eye)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedEye");
                                    else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Nose"))
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedNose");
                                    else if (bp.def == BodyPartDefOf.Heart && !Utils.MSELOADED)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedPrimaryHeart");
                                    else if (bp.def == BodyPartDefOf.Arm)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedArm");
                                    else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !Utils.MSELOADED)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedLung");
                                    else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Kidney") && !Utils.MSELOADED)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedKidney");
                                    else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Liver") && !Utils.MSELOADED)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedLiver");
                                    else if (bp.def == BodyPartDefOf.Leg)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedLeg");
                                    else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !Utils.MSELOADED)
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedStomach");
                                    else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedEar");
                                    else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                                        h = DefDatabase<HediffDef>.GetNamed("EvolvedSpine");
                                else if (bp.def == BodyPartDefOf.Torso)
                                    h = DefDatabase<HediffDef>.GetNamed("EvolvedCarapace");
                                
                                break;

                        }

                        if(h != null)
                            p.health.AddHediff(h, bp, null, null);
                    }
                    //HediffWithComps hediff_BodyPartModule = (HediffWithComps)HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("ArchotechEye"), p, BodyPartDefOf.Eye);
                    //p.health.hediffSet.AddDirect()

                    /*parts.Add(BodyPartDefOf.Eye);
                    HediffGiverUtility.TryApply(p, DefDatabase<HediffDef>.GetNamed("ArchotechEye"), parts, false, 2);
                    parts.Clear();
                    parts.Add(BodyPartDefOf.Heart);
                    HediffGiverUtility.TryApply(p, DefDatabase<HediffDef>.GetNamed("BionicHeart"), parts);
                    parts.Clear();
                    parts.Add(BodyPartDefOf.Arm);
                    HediffGiverUtility.TryApply(p, DefDatabase<HediffDef>.GetNamed("ArchotechArm"), parts, false, 2);
                    parts.Clear();
                    parts.Add(BodyPartDefOf.Leg);
                    HediffGiverUtility.TryApply(p, DefDatabase<HediffDef>.GetNamed("ArchotechLeg"), parts, false, 2);
                    parts.Clear();
                    parts.Add(BodyPartDefOf.Stomach);
                    HediffGiverUtility.TryApply(p, DefDatabase<HediffDef>.GetNamed("BionicStomach"), parts);
                    parts.Clear();
                    parts.Add(DefDatabase<BodyPartDef>.GetNamed("Ear"));
                    HediffGiverUtility.TryApply(p, DefDatabase<HediffDef>.GetNamed("BionicEar"), parts, false,2);
                    parts.Clear();
                    parts.Add(DefDatabase<BodyPartDef>.GetNamed("Spine"));
                    HediffGiverUtility.TryApply(p, DefDatabase<HediffDef>.GetNamed("BionicSpine"), parts);
                    */


                    if (p.health.hediffSet != null)
                    {
                        int val = 1;
                        if (bionicSource == 0)
                            val = 0;
                        for (int i = 0; i < p.health.hediffSet.hediffs.Count; i++)
                        {
                            Hediff h = p.health.hediffSet.hediffs[i];
                            Traverse.Create(h).Field("severityInt").SetValue(val);
                            p.health.Notify_HediffChanged(h);
                        }
                    }

                    break;
            }
            p.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("Mercenary4");
            p.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("MercenaryRecruit36");

            //If childhood disable the function concerned or shoot and mixed then it is changed by a generic
            SkillRecord skill = getAssociatedSkill(p, type);
            SkillRecord skillMeleeGen = getAssociatedSkill(p, MercenaryType.Melee);
            SkillRecord skillShootGen = getAssociatedSkill(p, MercenaryType.Ranged);


            if (skill == null || skillMeleeGen == null || skillShootGen == null)
            {
                //Log.Error("GenerateMerc : unable to find related skill");
                return null;
            }

            WorkTags curTypeWT = getTypeWorkTag(type);
            bool condMH = Settings.mustHaveTraits.Count != 0;

            if (skill.TotallyDisabled 
                || contradictedTrait
                || condMH
                || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(curTypeWT)
                || (skillMeleeGen.TotallyDisabled  && skillShootGen.TotallyDisabled)
                || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(WorkTags.Violent)
                || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(WorkTags.Cleaning)
                || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(WorkTags.Hauling))
            {
                if (condMH)
                {
                    foreach(var trait in Settings.mustHaveTraits)
                    {
                        bool found = false;
                        foreach(var ct in DefDatabase<TraitDef>.AllDefs)
                        {
                            foreach(var ctd in ct.degreeDatas)
                            {
                                if(ctd.untranslatedLabel == trait)
                                {
                                    p.story.traits.GainTrait(new Trait(ct, ctd.degree, true));
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                                break;
                        }
                    }
                }
            }

            //It is necessary to update the skills based on the new childhood / adulthood
            ResetCachedIncapableOf(p);

            //Removal of skills only if not purchased
            if (!purchased)
            {
                //sauvegarde skills originaux
                compUSFM.saveOrigSkills();
                //Removal of all skills except those concerned but storage of the original skills of the mercenary so that if he joins the colony they can be unlocked
                compUSFM.initSkills(type, level);
            }

            clearMercGear(p);

            try
            {
                processMercGear(p, type, gear, intGearColor);
            }
            catch (Exception e)
            {
                Log.Message("[MFM] " + e.Message);
            }
            try
            {
                processMercWeapon(p, type, weapon);
            }
            catch (Exception e)
            {
                Log.Message("[MFM] " + e.Message);
            }

            //Pawn pawn = (Pawn)GenSpawn.Spawn(newThing, Find.CurrentMap.Center, Find.CurrentMap, WipeMode.Vanish);

            return p;
        }

        public static void clearMercGear(Pawn p)
        {
            //We remove all the clothes and equipment present
            p.inventory.DestroyAll();
            p.equipment.DestroyAllEquipment();
            p.apparel.DestroyAll();
        }

        public static void processMercWeapon(Pawn p, MercenaryType type, int weapon)
        {
            Apparel clothe;
            ThingWithComps cw = null;
            string selWeapon = "";
            SkillRecord melee;
            SkillRecord shoot;
            melee = getAssociatedSkill(p, MercenaryType.Melee);
            shoot = getAssociatedSkill(p, MercenaryType.Ranged);

            int minRanged = 0;
            int maxRanged = 0;
            int minMelee = 0;
            int maxMelee = 0;

            

            switch (weapon)
            {
                case 4:
                    if (Settings.onlyVanillaForWeapon)
                    {
                        //If soldier type melee or stats in melee higher than ranged
                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                        {
                            selWeapon = "ThrumboHorn";

                            //Shield
                            clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt")));
                            if (clothe != null)
                            {
                                try
                                {
                                    p.apparel.Wear(clothe, false);
                                }catch(Exception)
                                {

                                }
                            }
                        }
                        else
                        {
                            if (Rand.Chance(0.5f))
                                selWeapon = "Gun_Minigun";
                            else
                                switch (Rand.Range(0, 2))
                                {
                                    case 0:
                                        selWeapon = "Gun_TripleRocket";
                                        break;
                                    case 1:
                                        selWeapon = "Gun_DoomsdayRocket";
                                        break;
                                }
                        }
                    }
                    else
                    {
                        if (Utils.modernUSFM())
                        {
                            minRanged = 1000;
                            maxRanged = 99999999;
                            minMelee = 500;
                            maxMelee = 99999999;
                        }
                        else
                        {
                            minRanged = 90;
                            maxRanged = 99999999;
                            minMelee = 500;
                            maxMelee = 99999999;
                        }

                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                            selWeapon = getRandomWeapon(true, minMelee, maxMelee, false);
                        else
                            selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                    cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                    if(cw != null)
                        p.equipment.AddEquipment(cw);
                    break;
                case 3:
                    if (Settings.onlyVanillaForWeapon)
                    {
                        //If soldier type melee or stats in melee higher than ranged
                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                        {
                            selWeapon = "MeleeWeapon_LongSword";

                            //Shield
                            clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt")));
                            p.apparel.Wear(clothe, false);
                        }
                        else
                        {
                            if (Utils.modernUSFM())
                            {
                                switch (Rand.Range(0, 3))
                                {
                                    case 0:
                                        selWeapon = "Gun_ChargeRifle";
                                        break;
                                    case 1:
                                        selWeapon = "Gun_AssaultRifle";
                                        break;
                                    case 2:
                                        selWeapon = "Gun_SniperRifle";
                                        break;
                                }
                            }
                            else
                                selWeapon = "Bow_Great";
                        }
                    }
                    else
                    {
                        if (Utils.modernUSFM())
                        {
                            minRanged = 450;
                            maxRanged = 950;
                            minMelee = 200;
                            maxMelee = 499;
                        }
                        else
                        {
                            minRanged = 90;
                            maxRanged = 99999999;
                            minMelee = 200;
                            maxMelee = 99999999;
                        }

                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                            selWeapon = getRandomWeapon(true, minMelee, maxMelee,false);
                        else
                            selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                    cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                    if(cw != null)
                        p.equipment.AddEquipment(cw);
                    break;
                case 2:
                    if (Settings.onlyVanillaForWeapon)
                    {
                        //If soldier type melee or stats in melee higher than ranged
                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                        {
                            selWeapon = "MeleeWeapon_Gladius";
                        }
                        else
                        {
                            if (Utils.modernUSFM())
                            {
                                switch (Rand.Range(0, 3))
                                {
                                    case 0:
                                        selWeapon = "Gun_LMG";
                                        break;
                                    case 1:
                                        selWeapon = "Gun_HeavySMG";
                                        break;
                                    case 2:
                                        selWeapon = "Gun_ChainShotgun";
                                        break;
                                }
                            }
                            else
                                selWeapon = "Bow_Recurve";
                        }
                    }
                    else
                    {
                        if (Utils.modernUSFM())
                        {
                            minRanged = 200;
                            maxRanged = 449;
                            minMelee = 100;
                            maxMelee = 199;
                        }
                        else
                        {
                            minRanged = 60;
                            maxRanged = 89;
                            minMelee = 100;
                            maxMelee = 199;
                        }

                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                            selWeapon = getRandomWeapon(true, minMelee, maxMelee,false);
                        else
                            selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                    cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                    if(cw != null)
                        p.equipment.AddEquipment(cw);
                    break;
                case 1:
                    if (Settings.onlyVanillaForWeapon)
                    {
                        //If soldier type melee or stats in melee higher than ranged
                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                        {
                            switch (Rand.Range(0, 3))
                            {
                                case 0:
                                    selWeapon = "MeleeWeapon_Club";
                                    break;
                                case 1:
                                    selWeapon = "MeleeWeapon_Knife";
                                    break;
                                case 2:
                                    selWeapon = "MeleeWeapon_Ikwa";
                                    break;
                            }
                        }
                        else
                        {
                            if (Utils.modernUSFM())
                            {
                                switch (Rand.Range(0, 3))
                                {
                                    case 0:
                                        selWeapon = "Gun_Revolver";
                                        break;
                                    case 1:
                                        selWeapon = "Gun_Autopistol";
                                        break;
                                    case 2:
                                        selWeapon = "Gun_MachinePistol";
                                        break;
                                }
                            }
                            else
                                selWeapon = "Bow_Short";
                        }
                    }
                    else
                    {
                        if (Utils.modernUSFM())
                        {
                            minRanged = 1;
                            maxRanged = 190;
                            minMelee = 1;
                            maxMelee = 99;
                        }
                        else
                        {
                            minRanged = 1;
                            maxRanged = 59;
                            minMelee = 1;
                            maxMelee = 99;
                        }

                        if (type == MercenaryType.Melee || (type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt))
                            selWeapon = getRandomWeapon(true, minMelee, maxMelee, false);
                        else
                            selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                    cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                    if(cw != null)
                        p.equipment.AddEquipment(cw);
                    break;
            }
            if (CELOADED && cw != null)
            {
                ThingComp ammoUser = TryGetCompByTypeName(cw, "CompAmmoUser", "CombatExtended");
                if (ammoUser != null)
                {
                    var props = Traverse.Create(ammoUser).Property("Props").GetValue();
                    int magazineSize = Traverse.Create(props).Field("magazineSize").GetValue<int>();
                    ThingDef def = Traverse.Create(ammoUser).Field("selectedAmmo").GetValue<ThingDef>();
                    if (def != null)
                    {
                        Traverse.Create(ammoUser).Method("ResetAmmoCount", new object[] { def }).GetValue();
                    }
                }
            }
        }

        public static ThingComp TryGetCompByTypeName(ThingWithComps thing, string typeName, string assemblyName = "")
        {
            return thing.AllComps.FirstOrDefault((ThingComp comp) => comp.GetType().Name == typeName);
        }

        public static void processMercGear(Pawn p,MercenaryType type, int gear, int intGearColor)
        {
            if (p.apparel == null)
            {
                Log.Error("[MFM] apparel == null on processMercGear");
                return;
            }

            /**************************************** Clothes *************************************************/

            Color gearColor = getGearColor(intGearColor);

            //================================= Adding free basic clothing
            Apparel clothe = null;
            ThingDef td = null;
            ThingDef onSkinTorso = null;
            int min = 0;
            int max = 0;
            int minHat = 0;
            int maxHat = 0;
            List<string> blacklist = new List<string>();
            List<string> whitelist = new List<string>();
            List<ApparelLayerDef> layers = new List<ApparelLayerDef>();
            List<ApparelLayerDef> blacklistedLayers = new List<ApparelLayerDef>();

            if (gear == 3)
            {
                if (Utils.modernUSFM())
                {
                    min = 600;
                    max = 999999;
                    minHat = 600;
                    maxHat = 999999;
                }
                else
                {
                    min = 400;
                    max = 999999;
                    minHat = 100;
                    maxHat = 999999;
                }
            }
            else if (gear == 2)
            {
                if (Utils.modernUSFM())
                {
                    min = 220;
                    max = 599;
                    minHat = 220;
                    maxHat = 599;
                    blacklist.Add("Apparel_FlakVest");
                }
                else
                {
                    max = 249;
                    min = 100;
                    minHat = 50;
                    maxHat = 99;
                }
            }
            else if (gear == 1)
            {
                if (Utils.modernUSFM())
                {
                    min = 1;
                    max = 219;
                    minHat = 1;
                    maxHat = 219;
                    whitelist.Add("Apparel_FlakVest");
                }
                else
                {
                    min = 1;
                    max = 99;
                    minHat = 1;
                    maxHat = 49;
                }
            }

            Color defColor = gearColor;

            if(gear != 0)
            {
                if (Settings.onlyVanillaForGear)
                {
                    if (Utils.modernUSFM())
                        processMercGearVanilla(p, gear, intGearColor);
                    else
                        processMercGearVanillaMedieval(p, gear, intGearColor);
                }
                else
                { 
                    layers.Add(ApparelLayerDefOf.Overhead);
                    td = getRandomGear(p,BodyPartGroupDefOf.UpperHead, BodyPartGroupDefOf.FullHead, layers, minHat, maxHat, whitelist,blacklist);
                    if (td != null)
                    {
                        clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                        if (clothe != null)
                        {
                            if (clothe.TryGetComp<CompColorable>() != null)
                                clothe.SetColor(defColor);
                            try
                            {
                                p.apparel.Wear(clothe, false);
                            }
                            catch (Exception)
                            {
                            }
                            if (p.outfits != null && p.outfits.forcedHandler != null)
                                p.outfits.forcedHandler.SetForced(clothe, true);
                        }
                    }


                    layers.Clear();
                    layers.Add(ApparelLayerDefOf.OnSkin);
                    td = getRandomGear(p,BodyPartGroupDefOf.Torso, null, layers, min, max, whitelist, blacklist, true);
                    if(td == null)
                        td = getRandomGear(p,BodyPartGroupDefOf.Torso, null, layers, 1, max, whitelist, blacklist, true);
                    if (td != null)
                    {
                        onSkinTorso = td;
                        //Log.Message(td.defName);
                        clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                        if (clothe != null)
                        {
                            if (clothe.TryGetComp<CompColorable>() != null)
                                clothe.SetColor(defColor);
                            try
                            {
                                p.apparel.Wear(clothe, false);
                            }
                            catch (Exception)
                            {

                            }
                            if (p.outfits != null && p.outfits.forcedHandler != null)
                                p.outfits.forcedHandler.SetForced(clothe, true);
                        }
                        else
                        {
                            td = null;
                        }
                    }

                    layers.Clear();
                    layers.Add(ApparelLayerDefOf.Shell);
                    layers.Add(ApparelLayerDefOf.Middle);
                    blacklistedLayers.Add(ApparelLayerDefOf.OnSkin);
                    td = getRandomGear(p,BodyPartGroupDefOf.Torso, null, layers, min, max, whitelist, blacklist, false, blacklistedLayers);
                    if (td != null)
                    {
                        //Log.Message(td.defName);
                        clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                        if (clothe != null)
                        {
                            if (clothe.TryGetComp<CompColorable>() != null)
                                clothe.SetColor(defColor);
                            try
                            {
                                p.apparel.Wear(clothe, false);
                            }
                            catch (Exception)
                            {

                            }
                            if (p.outfits != null && p.outfits.forcedHandler != null)
                                p.outfits.forcedHandler.SetForced(clothe, true);
                        }
                        else
                        {
                            td = null;
                        }
                    }

                    //If the bodyarmo also includes the bottom, we squeeze this part
                    if (onSkinTorso == null || !onSkinTorso.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs))
                    {
                        bool onlyOnSkin = true;
                        layers.Clear();
                        if (td == null || !td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs))
                        {
                            onlyOnSkin = false;
                            layers.Add(ApparelLayerDefOf.Middle);
                        }

                        layers.Add(ApparelLayerDefOf.OnSkin);
                        td = getRandomGear(p,BodyPartGroupDefOf.Legs, null, layers, min, max, whitelist, blacklist, onlyOnSkin, null, true);
                        if(td == null)
                            td = getRandomGear(p,BodyPartGroupDefOf.Legs, null, layers, 1, max, whitelist, blacklist, onlyOnSkin, null, true);
                        if (td != null)
                        {
                            //Log.Message(td.defName);
                            clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                            if (clothe != null)
                            {
                                if (clothe.TryGetComp<CompColorable>() != null)
                                    clothe.SetColor(defColor);
                                try
                                {
                                    p.apparel.Wear(clothe, false);
                                }
                                catch (Exception)
                                {

                                }
                                if (p.outfits != null && p.outfits.forcedHandler != null)
                                    p.outfits.forcedHandler.SetForced(clothe, true);
                            }
                        }
                    }
                }
            }

            if (Utils.modernUSFM())
            {
                if (Settings.onlyVanillaForGear || gear == 0)
                {
                    //Depending on the type of mercenary (military or civilian, they are assigned a basic outfit)
                    if (type == MercenaryType.Melee || type == MercenaryType.Ranged)
                    {
                        //Combat uniform
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt")));
                        if (clothe != null)
                        {
                            clothe.SetColor(Color.white);
                            p.apparel.Wear(clothe, false);
                        }
                    }
                    else
                    {
                        //Civilian uniform
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirtCivil"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt")));
                        Color color = getColorForType(type);
                        if (clothe != null)
                        {
                            clothe.SetColor(color);
                            try
                            {
                                p.apparel.Wear(clothe, false);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
            else
            {
                //Medieval mode ragged assignment if no armor
                if (gear == 0)
                {
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_TribalA"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_TribalA")));
                    //Color color = getColorForType(type);
                    if (clothe != null)
                    {
                        //clothe.SetColor(color);
                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }

            if (clothe != null && p.outfits != null && p.outfits.forcedHandler != null)
            {
                p.outfits.forcedHandler.SetForced(clothe, true);
            }

            if (Utils.modernUSFM())
            {
                //Add generic pants if necessary
                if ((Settings.onlyVanillaForGear && gear != 2) || gear == 0)
                {
                    //Generic pants with two types of mercenaries
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Pants"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Pants")));
                    if (clothe != null)
                    {
                        clothe.SetColor(Color.white);
                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {

                        }
                        if (p.outfits != null && p.outfits.forcedHandler != null)
                            p.outfits.forcedHandler.SetForced(clothe, true);
                    }
                }
            }
        }

        public static void processMercGearVanilla(Pawn p, int gear, int gearColor)
        {
            Apparel clothe;
            Color defColor = getGearColor(gearColor);
            
            switch (gear)
            {
                case 3:
                    //Marine armor
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmor"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmor")));
                    if (clothe != null)
                    {
                        clothe.SetColor(defColor);
                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {

                        }
                        //Marine helmet
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmorHelmet"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmorHelmet")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                    }
                    break;
                case 2:
                    //Flak armor
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_FlakJacket"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_FlakJacket")));
                    if (clothe != null)
                    {
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_FlakPants"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_FlakPants")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                        //Advanced headset
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                    }
                    break;
                case 1:
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_FlakVest"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_FlakVest")));
                    if (clothe != null)
                    {
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                        //Advanced headset
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                    }
                    break;
            }
        }

        public static void processMercGearVanillaMedieval(Pawn p, int gear, int gearColor)
        {
            Apparel clothe;
            Color defColor = getGearColor(gearColor);

            switch (gear)
            {
                case 3:
                    //Marine armor
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_PlateArmor"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_PlateArmor")));
                    if (clothe != null)
                    {
                        clothe.SetColor(defColor);
                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {

                        }
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                    }
                    break;
                case 2:
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Parka"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Parka")));
                    if (clothe != null)
                    {
                        clothe.SetColor(defColor);
                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {

                        }
                        //Single helmet
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                        //Trousers
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Pants"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Pants")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                    }
                    break;
                case 1:
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_BasicShirt"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_BasicShirt")));
                    if (clothe != null)
                    {
                        clothe.SetColor(defColor);
                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {

                        }
                        //Single helmet
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_WarMask"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_WarMask")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                        //Trousers
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Pants"), GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Pants")));
                        clothe.SetColor(defColor);
                        p.apparel.Wear(clothe, false);
                    }
                    break;
            }
        }

        public static string serializeLP(List<Pawn> lp, Map backMap)
        {
            List<string> tmp = new List<string>();
            foreach(var entry in lp)
            {
                tmp.Add(entry.GetUniqueLoadID());
            }
            tmp.Add(backMap.GetUniqueLoadID());

            return serializeLS(tmp);
        }

        public static List<Pawn> unserializeLP(string data, out Map backMap)
        {
            backMap = null;
            List<string> tmp = unserializeLS(data);
            List<Pawn> ret = new List<Pawn>();

            foreach(var entry in tmp)
            {
                foreach(var p in Utils.GCMFM.getRentedPawns())
                {
                    if (p.GetUniqueLoadID() == entry)
                    {
                        ret.Add(p);
                        break;
                    }
                    else if (entry.StartsWith("Map"))
                    {
                        backMap = Utils.getMapFromMUID(entry);
                    }
                }
            }
            return ret;
        }

        public static string serializeLS(List<string> lst)
        {
            return String.Join(",", lst.ToArray());
        }

        public static List<string> unserializeLS(string data)
        {
            List<string> ret = null;
            ret = data.Split(',').ToList();

            return ret;
        }

        public static void unserializePowerBeamAttack(string data, out Map map, out IntVec3 coord)
        {
            string[] ret = data.Split(',');

            coord = default(IntVec3);
            map = null;

            if (ret.Count() != 4)
                return;

            map = Utils.getMapFromMUID(ret[0]);
            coord = new IntVec3(int.Parse(ret[1]), int.Parse(ret[2]), int.Parse(ret[3]));
        }

        public static Map getRandomMapOfPlayer()
        {
            Map ret = null;
            foreach(var map in Find.Maps)
            {
                if (map.IsPlayerHome)
                {
                    ret = map;
                    break;
                }
            }
            return ret;
        }

        /*
         * Serialize a (String,Int) dictionary
         */
        public static string serializeDSI(Dictionary<string, int> data)
        {
            string ret = "";
            foreach(var entry in data)
            {
                ret += entry.Key + ":" + entry.Value + "§";
            }
            return ret;
        }

        /*
         * Unserialize a (String, Int) dictionary
         */
        public static Dictionary<string, int> unserializeDSI(string data)
        {
            Dictionary<string, int> ret = null;
            if (data == "")
                return ret;

            string[] els = data.Split('§');
            ret = new Dictionary<string, int>();
            foreach(var el in els)
            {
                string[] p = el.Split(':');
                if(p.Count() == 2)
                {
                    ret[p[0]] = Int32.Parse(p[1]);
                }
            }

            return ret;
        }

        /*
         * Serialize a (String,Int) dictionary
         */
        public static string serializeDSS(Dictionary<string, string> data)
        {
            string ret = "";
            foreach (var entry in data)
            {
                ret += entry.Key + ":" + entry.Value + "§";
            }
            return ret;
        }

        /*
         * Unserialize a (String, Int) dictionary
         */
        public static Dictionary<string, string> unserializeDSS(string data)
        {
            Dictionary<string, string> ret = null;
            if (data == "")
                return ret;

            string[] els = data.Split('§');
            ret = new Dictionary<string, string>();
            foreach (var el in els)
            {
                string[] p = el.Split(':');
                if (p.Count() == 2)
                {
                    ret[p[0]] = p[1];
                }
            }

            return ret;
        }

        /*
         * Obtaining Map from UID
         */
        public static Map getMapFromUID(int UID)
        {
            foreach(var map in Find.Maps)
            {
                if(map.uniqueID == UID)
                {
                    return map;
                }
            }
            return null;
        }

        /*
         * Obtaining Map from MUID
         */
        public static Map getMapFromMUID(string MUID)
        {
            foreach (var map in Find.Maps)
            {
                if (map.GetUniqueLoadID() == MUID)
                {
                    return map;
                }
            }
            return null;
        }

        public static Color getColorForType(MercenaryType type)
        {
            Color color = default(Color);
            switch (type)
            {
                case MercenaryType.Artist:
                    color = new Color(1.0f, 0.556f, 0.239f);
                    break;
                case MercenaryType.Builder:
                    color = new Color(0.854901f, 0.745009f, 0.235294f);
                    break;
                case MercenaryType.Cooker:
                    color = new Color(0.1411f, 0.7450f, 0.7411f);
                    break;
                case MercenaryType.Farmer:
                    color = new Color(0.2392f, 0.8470f, 0.4823f);
                    break;
                case MercenaryType.Medical:
                    color = new Color(0.870588f, 0.870588f, 0.870588f);
                    break;
                case MercenaryType.Speaker:
                    color = new Color(1.0f, 0.6f, 1.0f);
                    break;
                case MercenaryType.Tech:
                    color = new Color(0.12941f, 0.5254f, 0.7333f);
                    break;
                case MercenaryType.Scientist:
                    color = new Color(0.5921f, 0.2627f, 1.0f);
                    break;
                case MercenaryType.Trainer:
                    color = new Color(0.40392f, 0.7372f, 0.1372f);
                    break;
                case MercenaryType.Miner:
                    color = new Color(0.44313f, 0.423529f, 0.411764f);
                    break;
            }

            return color;
        }

        public static void ResetCachedIncapableOf(Pawn pawn)
        {
            pawn.ClearCachedDisabledWorkTypes();
            pawn.ClearCachedDisabledSkillRecords();
            List<string> incapableList = new List<string>();
            WorkTags combinedDisabledWorkTags = pawn.CombinedDisabledWorkTags;
            if (combinedDisabledWorkTags != WorkTags.None)
            {
                IEnumerable<WorkTags> list = (IEnumerable<WorkTags>)typeof(CharacterCardUtility).GetMethod("WorkTagsFrom", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { combinedDisabledWorkTags });
                foreach (var tag in list)
                {
                    incapableList.Add(WorkTypeDefsUtility.LabelTranslated(tag).CapitalizeFirst());
                }
            }
        }

        public static void ClearCachedDisabledWorkTypes(this Pawn pawn)
        {
            if (pawn != null)
            {
                typeof(Pawn).GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pawn, null);
            }
        }

        public static void ClearCachedDisabledSkillRecords(this Pawn pawn)
        {
            if (pawn.skills != null && pawn.skills.skills != null)
            {
                FieldInfo field = typeof(SkillRecord).GetField("cachedTotallyDisabled", BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var record in pawn.skills.skills)
                {
                    field.SetValue(record, BoolUnknown.Unknown);
                }
            }
        }

        /*
         * Check if a pawn can be recruited as a mercenary
         */
        public static bool canBeMercenary(Pawn pawn, out string msg)
        {
            bool ret = true;
            if (!Settings.allowNonViolentToBeRented)
                msg = "MFM_MsgPawnCannotBeRentedAsMerc".Translate(pawn.LabelCap);
            else
                msg = "MFM_MsgPawnCannotBeRentedAsMerc2".Translate(pawn.LabelCap);

            //If invalid trait
            foreach (var s in pawn.story.traits.allTraits)
            {
                if (Settings.blacklistedTraitsV2.Contains(s.def.DataAtDegree(s.Degree).untranslatedLabel))
                {
                    //Log.Message("=>>" + s.def.defName);
                    ret = false;
                    break;
                }
            }

            //If must have a trait and he does not
            if (Settings.mustHaveTraits.Count != 0)
            {
                foreach (var trait in Settings.mustHaveTraits)
                {
                    bool found = false;
                    foreach (var ct in DefDatabase<TraitDef>.AllDefs)
                    {
                        foreach (var ctd in ct.degreeDatas)
                        {
                            if (ctd.untranslatedLabel == trait)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found)
                            break;
                    }
                    if (!found)
                    {
                        "MFM_MsgPawnCannotBeRentedAsMercBecauseMissingTrait".Translate(pawn.LabelCap);
                    }
                }
            }

            if (!Settings.allowNonViolentToBeRented)
            {
                SkillRecord skillMeleeGen = getAssociatedSkill(pawn, MercenaryType.Melee);
                SkillRecord skillShootGen = getAssociatedSkill(pawn, MercenaryType.Ranged);

                //Si fragile
                if (skillMeleeGen.TotallyDisabled && skillShootGen.TotallyDisabled)
                    ret = false;
            }

            Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();
            //If already a recruited mercenary ==> no
            if (comp.isMercenary)
                ret = false;
            int CGT = Find.TickManager.TicksGame;
            //If fired ===> no
            if (comp.firedGT > CGT)
            {
                msg = "MFM_MsgCannotBeRentedAsMercBecauseFired".Translate(pawn.LabelCap, Utils.getUSFMLabel(),( comp.firedGT - CGT).ToStringTicksToPeriodVerbose());
                ret = false;
            }

            if (pawn.Downed || (pawn.health != null && (pawn.health.summaryHealth.SummaryHealthPercent < 0.75f))) {
                msg = "MFM_MsgCannotBeRentedAsMercBecauseBadHealth".Translate(pawn.LabelCap);
                ret = false;
             }

            return ret;
        }

        /*
         * Obtaining the current quadrul
         */
        public static Quadrum getCurrentQuad()
        {
            /*Map map = Find.CurrentMap;
            if (Utils.getRandomMapOfPlayer() != null)
                map = Utils.getRandomMapOfPlayer();*/

            return GenDate.Quadrum(Find.TickManager.TicksAbs, 0);
        }


        /*
         * based on the type of delivery obtaining a GT
         */
        public static int getDeliveryGT(int delivery)
        {
            int ret = Find.TickManager.TicksGame;
            switch (delivery)
            {
                case 2:
                    ret += Rand.Range(Settings.transportQuickMinHour * 2500, Settings.transportQuickMaxHour * 2500);
                    break;
                case 1:
                    ret += Rand.Range(500, 1250);
                    break;
                default:
                case 3:
                    ret += Rand.Range(Settings.transportStandardMinHour * 2500, Settings.transportStandardMaxHour * 2500);
                    break;
            }

            return ret;
        }


        public static void getWantedInfo(Dictionary<string, int> wanted, out int nbMerc, out int price, out int guarantee)
        {
            nbMerc = 0;
            price = 0;
            guarantee = 0;

            foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
            {
                string index1 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Recruit);
                string index2 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Confirmed);
                string index3 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Veteran);
                string index4 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Elite);
                string index5 = Utils.GCMFM.buildStockIndex(type, MercenaryLevel.Cyborg);

                if (wanted[index1] != 0)
                {
                    price += wanted[index1] * Settings.priceRecruit;
                    guarantee += wanted[index1] * Settings.guaranteeRecruit;
                }
                else if (wanted[index2] != 0)
                {
                    price += wanted[index2] * Settings.priceConfirmed;
                    guarantee += wanted[index2] * Settings.guaranteeConfirmed;
                }
                else if (wanted[index3] != 0)
                {
                    price += wanted[index3] * Settings.priceVeteran;
                    guarantee += wanted[index3] * Settings.guaranteeVeteran;
                }
                else if (wanted[index4] != 0)
                {
                    price += wanted[index4] * Settings.priceElite;
                    guarantee += wanted[index4] * Settings.guaranteeElite;
                }
                else if (wanted[index5] != 0)
                {
                    price += wanted[index5] * Settings.priceCyborg;
                    guarantee += wanted[index5] * Settings.guaranteeCyborg;
                }

                nbMerc += wanted[index1] + wanted[index2] + wanted[index3] + wanted[index4] + wanted[index5];
            }
        }


        /*
         * Random return of a device intended for a specified part of a body and having a value between the two limits
         * And NO tribal
         */
        public static ThingDef getRandomGear(Pawn p, BodyPartGroupDef body, BodyPartGroupDef body2, List<ApparelLayerDef> layers,  int min, int max, List<string> priceWhitelist, List<string> priceBlacklist, bool onlySpecifiedLayer=false, List<ApparelLayerDef> blacklistedLayers=null, bool onlySpecifiedBodypart = false)
         {
            bool medievalMode = !Utils.modernUSFM();

            IEnumerable<ThingDef> ret = DefDatabase<ThingDef>.AllDefsListForReading.Where(delegate (ThingDef x)
            {

                if (!x.IsApparel)
                    return false;

                bool layersOk = false;
                bool bodyPartOK = false;
                

                if (layers != null && layers.Count != 0)
                {
                    if (onlySpecifiedLayer)
                        layersOk = true;
                    foreach (var l in layers)
                    {
                        if (onlySpecifiedLayer)
                        {
                            foreach(var l2 in x.apparel.layers)
                            {
                                if (!layers.Contains(l2))
                                {
                                    layersOk = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (x.apparel.layers.Contains(l))
                            {
                                layersOk = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    layersOk = true;
                }

                if(layersOk && blacklistedLayers != null)
                {
                    foreach(var el in x.apparel.layers)
                    {
                        if (blacklistedLayers.Contains(el))
                        {
                            layersOk = false;
                            break;
                        }
                    }
                }


                if (onlySpecifiedBodypart)
                {
                    foreach (var el in x.apparel.bodyPartGroups)
                    {
                        if (body == el || body2 == el)
                            bodyPartOK = true;
                        else
                        {
                            bodyPartOK = false;
                            break;
                        }
                    }
                }
                else
                {
                    bodyPartOK = (x.apparel.bodyPartGroups.Contains(body) || (body2 != null && x.apparel.bodyPartGroups.Contains(body2)));
                }
                
                

                bool priceOK = (x.BaseMarketValue >= min && x.BaseMarketValue <= max);

                /*if (layersOk
                && bodyPartOK
                && ((priceOK && !priceBlacklist.Contains(x.defName)) || (!priceOK && priceWhitelist.Contains(x.defName)))
                && ((medievalMode && x.techLevel <= TechLevel.Medieval) || (!medievalMode && (x.techLevel > TechLevel.Medieval || intMedievalStuffException.Contains(x.defName)))))
                {
                    Log.Message("=>" + x.defName + " " + x.BaseMarketValue + " " + bodyPartOK + " " + layersOk + " " + (x.apparel.bodyPartGroups.Contains(body) || (body2 != null && x.apparel.bodyPartGroups.Contains(body2))) + " " + ((priceOK && !priceBlacklist.Contains(x.defName)) || (!priceOK && priceWhitelist.Contains(x.defName))) + " " + ((medievalMode && x.techLevel <= TechLevel.Medieval) || (!medievalMode && (x.techLevel > TechLevel.Medieval || intMedievalStuffException.Contains(x.defName)))));
                }*/

                return layersOk
                && Utils.apparelCanBeWearedBy(x,p)
                && bodyPartOK
                && ((priceOK && !priceBlacklist.Contains(x.defName)) || (!priceOK && priceWhitelist.Contains(x.defName)))
                && ((medievalMode && x.techLevel <= TechLevel.Medieval) || (!medievalMode && (x.techLevel > TechLevel.Medieval || intMedievalStuffException.Contains(x.defName))));
            });
            if (ret.Count() == 0)
                return null;
            else
                return ret.RandomElement();
        }


        /*
         * Randomly return a weapon with a value between the two limits
         * And NO tribal
         */
        public static string getRandomWeapon(bool melee, int min, int max, bool minTechLevelMedieval=true)
        {
            bool medievalMode = !Utils.modernUSFM();

            IEnumerable<ThingDef> ret = DefDatabase<ThingDef>.AllDefsListForReading.Where(delegate (ThingDef x)
            {
                /*if (x.IsWeapon
                && ((melee && x.IsMeleeWeapon) || (!melee && x.IsRangedWeapon)))
                    Log.Message("=>"+x.defName + " " + x.BaseMarketValue);*/

                return x.IsWeapon
                && x.BaseMarketValue >= min && x.BaseMarketValue <= max
                && ((melee && x.IsMeleeWeapon) || (!melee && x.IsRangedWeapon))
                && ((medievalMode && x.techLevel <= TechLevel.Medieval) || (!medievalMode && ((minTechLevelMedieval && x.techLevel > TechLevel.Medieval) || !minTechLevelMedieval)))
                && (!Settings.blacklistedWeapons.Contains(x.defName) && !intBlacklistedWeapons.Contains(x.defName));
            });

            if (ret == null || ret.Count() == 0)
                return null;

            ThingDef cdef = ret.RandomElement();
            //Log.Message(cdef.defName + " " + cdef.BaseMarketValue+" "+(!intBlacklistedWeapons.Contains(cdef.defName)));
            return cdef.defName;
        }

        public static string getColorText(int color)
        {
            string ret = "";
            switch ((Colors)color)
            {
                case Colors.Blue:
                    ret = "MFM_ColorBlue";
                    break;
                case Colors.Gray:
                    ret = "MFM_ColorGray";
                    break;
                case Colors.Green:
                    ret = "MFM_ColorGreen";
                    break;
                case Colors.Orange:
                    ret = "MFM_ColorOrange";
                    break;
                case Colors.Pink:
                    ret = "MFM_ColorPink";
                    break;
                case Colors.Purple:
                    ret = "MFM_ColorPurple";
                    break;
                case Colors.Red:
                    ret = "MFM_ColorRed";
                    break;
                case Colors.White:
                    ret = "MFM_ColorWhite";
                    break;
                case Colors.Yellow:
                    ret = "MFM_ColorYellow";
                    break;
                case Colors.Cyan:
                    ret = "MFM_ColorCyan";
                    break;
                default:
                case Colors.Black:
                    ret = "MFM_ColorBlack";
                    break;
            }

            return ret.Translate();
        }

        
        public static Color getGearColor(int color)
        {
            Color ret;
            switch ((Colors)color)
            {
                case Colors.Blue:
                    ret = new Color(0.2f, 0.345f, 0.756f,1.0f);
                    break;
                case Colors.Gray:
                    ret = new Color(0.515f, 0.515f, 0.515f, 1.0f);
                    break;
                case Colors.Green:
                    ret = new Color(0.133f, 0.623f, 0.113f, 1.0f);
                    break;
                case Colors.Orange:
                    ret = new Color(0.949f, 0.603f, 0.109f,1.0f);
                    break;
                case Colors.Pink:
                    ret = new Color(0.956f, 0.341f, 0.850f,1.0f);
                    break;
                case Colors.Purple:
                    ret = new Color(0.486f, 0.113f, 0.647f,1.0f);
                    break;
                case Colors.Red:
                    ret = new Color(0.941f, 0f, 0.137f, 1.0f);
                    break;
                case Colors.White:
                    ret = new Color(1f, 1f, 1f, 1f);
                    break;
                case Colors.Yellow:
                    ret = new Color(0.921f, 0.8f, 0.117f,1f);
                    break;
                case Colors.Cyan:
                    ret = new Color(0.180f, 0.898f, 0.890f, 1f);
                    break;
                default:
                case Colors.Black:
                    ret = new Color(0.25f, 0.25f, 0.25f,1.0f);
                    break;
            }

            return ret;
        }

        public static bool anyPlayerColonnyHasEnoughtSilver(int price)
        {
            foreach(var map in Find.Maps)
            {
                if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, price))
                {
                    return true;
                }
            }
            return false;
        }

        public static void anyPlayerColonnyPaySilver(int price)
        {
            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, price))
                {
                    TradeUtility.LaunchSilver(map, price);
                    break;
                }
            }
        }

        /*
         * Calculation of the pawn score
         */
        public static float getPawnScore(Pawn pawn)
        {
            float score;
            //Calculation of raw score
            score = pawn.kindDef.combatPower * pawn.health.summaryHealth.SummaryHealthPercent;

            //the adjustment is only carried out on 50% of the score we bring the adjustment to a value between 0 and 1
            float adjust = Math.Min(1.0f, getPawnScoreBasic(pawn) + 0.5f);

            //Log.Message(pawn.LabelCap+" " + adjust.ToString()+" ( "+lvlConsciousness+" "+lvlManipulation+" "+lvlMoving+" "+lvlSight+" )");

            return score * adjust;
        }

        public static float getPawnScoreBasic(Pawn pawn)
        {
            //Smoothing of the score with the potential defaults at the level of the capacities (consciousness, manipulation, Moving, Sight) of the pawn
            float lvlConsciousness = Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Consciousness), 0.15f);
            float lvlManipulation = Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Manipulation), 0.15f);
            float lvlMoving = Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Moving), 0.15f);
            float lvlSight = Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Sight), 0.15f);

            return (lvlConsciousness * lvlManipulation * lvlMoving * lvlSight);
        }

        public static void startPowerBeamAt(Map map, IntVec3 pos)
        {
            PowerBeam powerBeam = (PowerBeam)GenSpawn.Spawn(ThingDefOf.PowerBeam, pos, map, WipeMode.Vanish);
            powerBeam.duration = 600;
            powerBeam.StartStrike();
        }

        public static bool isPlayerMercenary(Pawn pawn)
        {
            foreach (var m in Find.ColonistBar.Entries)
            {
                if (m.pawn == pawn)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<Pawn> getPlayerMercenaries()
        {
            List<Pawn> ret = new List<Pawn>();
            foreach (var m in Find.ColonistBar.Entries)
            {
                Comp_USFM comp = m.pawn.TryGetComp<Comp_USFM>();
                if(comp != null && comp.isMercenary && comp.hiredByPlayer)
                {
                    ret.Add(m.pawn);
                }
            }

            //Added prisoners having hiredByPlayer == true
            foreach (var map in Find.Maps)
            {
                foreach(var p in map.mapPawns.PrisonersOfColonySpawned)
                {
                    Comp_USFM comp = p.TryGetComp<Comp_USFM>();
                    if (comp != null && comp.hiredByPlayer)
                        ret.Add(p);
                }
            }

            return ret;
        }


        public static void showDiscountCodeList(Action<string> action)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();

            foreach (var m in Utils.GCMFM.getDiscountList())
            {
                opts.Add(new FloatMenuOption(m.Key, delegate { action(m.Key); }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            if (opts.Count != 0)
            {
                //Clear option
                opts.Add(new FloatMenuOption("MFM_ClearDiscount".Translate(), delegate { action(""); }, MenuOptionPriority.Default, null, null, 0f, null, null));

                FloatMenu floatMenuMap = new FloatMenu(opts);
                Find.WindowStack.Add(floatMenuMap);
            }
        }


        public static string generateDiscountCode(uint number)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < 5; ++i)
            {
                b.Append(ALPHABET[(int)number & ((1 << 5) - 1)]);
                number = number >> 5;
            }
            return b.ToString();
        }

        /*
         * Increment the associated skill of a rented mercenary
         */
        public static void incMercSkillXP(Pawn merc, int nbHours)
        {
            Comp_USFM comp = merc.TryGetComp<Comp_USFM>();
            if (comp == null)
                return;

            SkillRecord skill = getAssociatedSkill(merc, comp.type);
            float xp = (Rand.Range(Settings.minXPRentedMercs/24, Settings.maxXPRentedMercs/24) * skill.LearnRateFactor()) * nbHours;
            skill.Learn(xp);
        }

        public static bool counterOfferInProgressMsg(Pawn pawn) {
            bool ret = Utils.GCMFM.CounterOfferInProgress;
            if (ret)
                Messages.Message("MFM_MsgCannotDropWeaponDuringCounterOffer".Translate(pawn.LabelCap), MessageTypeDefOf.NegativeEvent);

            return ret;
        }

        public static bool modernUSFM()
        {
            if (Settings.currentEpoch == 0)
            {
                if (microElecResearchDef == null)
                    microElecResearchDef = DefDatabase<ResearchProjectDef>.GetNamed("MicroelectronicsBasics");

                if (microElecResearchDef != null && microElecResearchDef.IsFinished)
                    return true;
                else
                    return false;
            }
            else if (Settings.currentEpoch == 1 && GCMFM.factionIronAlliance != null)
                return false;
            else
                return true;
        }

        public static string getUSFMLabel()
        {
            string usfmTitle;
            if (Utils.modernUSFM())
                usfmTitle = "MFM_USFMTitle".Translate();
            else
                usfmTitle = "MFM_USFMMedievalTitle".Translate();

            return usfmTitle;
        }

        public static IntVec3 spawnMercOnMap(Map map, List<Pawn> toDeliver)
        {
            IntVec3 dropCellNear = default(IntVec3);
            if (modernUSFM())
            {
                //if(!RCellFinder.TryFindRandomPawnEntryCell(out dropCellNear, map, CellFinder.EdgeRoadChance_Hostile, false, null))
                dropCellNear = DropCellFinder.FindRaidDropCenterDistant(map);
                if(!dropCellNear.IsValid)
                    dropCellNear = CellFinder.RandomCell(map);

                DropPodUtility.DropThingsNear(dropCellNear, map, toDeliver.Cast<Thing>(), 100, false, false, false);
            }
            else
            {
                IntVec3 intVec;

                Predicate<IntVec3> baseValidator = (IntVec3 x) => x.Standable(map) && !x.Fogged(map);
                Faction hostFaction = map.ParentFaction;
                IntVec3 root;
                if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 x) => baseValidator(x) && ((hostFaction != null && map.reachability.CanReachFactionBase(x, hostFaction)) || (hostFaction == null && map.reachability.CanReachBiggestMapEdgeDistrict(x))), map, CellFinder.EdgeRoadChance_Neutral, out root))
                {
                    intVec = CellFinder.RandomClosewalkCellNear(root, map, 5, null);
                }
                else if (CellFinder.TryFindRandomEdgeCellWith((IntVec3 x) => baseValidator(x), map, CellFinder.EdgeRoadChance_Neutral, out root))
                {
                    intVec = CellFinder.RandomClosewalkCellNear(root, map, 5, null);
                }
                else if (CellFinder.TryFindRandomEdgeCellWith(baseValidator, map, CellFinder.EdgeRoadChance_Neutral, out root))
                {
                    intVec = CellFinder.RandomClosewalkCellNear(root, map, 5, null);
                }
                else
                {
                    intVec = CellFinder.RandomCell(map);
                }

                for (int i = 0; i < toDeliver.Count; i++)
                {
                    dropCellNear = CellFinder.RandomClosewalkCellNear(intVec, map, 5, null);
                    GenSpawn.Spawn(toDeliver[i], dropCellNear, map, Rot4.Random, WipeMode.Vanish, false);
                }
            }

            return dropCellNear;
        }

        public static bool spawnMedievalCaravan(Map map, List<Thing> thingsToSpawn, out IntVec3 spawnedThings)
        {
            spawnedThings = default(IntVec3);
            IntVec3 spawnCenter;
            Faction faction = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("USFM_FactionAOS"));

            RCellFinder.TryFindRandomPawnEntryCell(out spawnCenter, map, CellFinder.EdgeRoadChance_Neutral, false, null);

            IntVec3 travelDest;
            if (!RCellFinder.TryFindTravelDestFrom(spawnCenter, map, out travelDest))
            {
                Log.Warning("Failed to do traveler incident from " + spawnCenter + ": Couldn't find anywhere for the traveler to go.");
                return false;
            }

            int points = 0;
            //Calculation of points of brought elements
            foreach (var el in thingsToSpawn)
            {
                if (el.def.defName == "silver")
                    points += el.stackCount;
                else
                    points += (int) el.def.BaseMarketValue * el.stackCount;
            }

            points = points / 50;
            if (points < 50)
                points = 50;
            //points = 1000;

            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = faction,
                points = points
            }, false).ToList<Pawn>();

            //Activate mercenary mode to display icon
            foreach (var el in list)
            {
                Comp_USFM comp = el.TryGetComp<Comp_USFM>();
                if(comp != null)
                {
                    comp.type = (MercenaryType)(-1);
                    comp.isMercenary = true;
                }
            }

            //Spawn of travelers
            foreach (Pawn current in list)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(spawnCenter, map, 5, null);
                GenSpawn.Spawn(current, loc, map, WipeMode.Vanish);
            }
            //Spawn things
            DropPodUtility.DropThingsNear(spawnCenter, map, thingsToSpawn, 0, true, false, false);

            /*foreach (var ts in thingsToSpawn)
            {
                
                GenSpawn.Spawn(ts, spawnCenter, map, WipeMode.Vanish);
            }*/

            if (list.Count == 0)
            {
                return false;
            }

            LordJob_TravelAndExit lordJob = new LordJob_TravelAndExit(travelDest);
            LordMaker.MakeNewLord(faction, lordJob, map, list);
            //PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter_Send(list, "LetterRelatedPawnsNeutralGroup".Translate(Faction.OfPlayer.def.pawnsPlural), LetterDefOf.NeutralEvent, true, true);

            spawnedThings = spawnCenter;
            return true;
        }


        /*
         * Obtaining a caravan parked on a payment site
         */
        public static Caravan caravanOfPlayerOverSiteOfPayment()
        {
            foreach (var caravan in Find.WorldObjects.Caravans)
            {
                int cTile = caravan.Tile;
                foreach(var wobj in Find.WorldObjects.AllWorldObjects)
                {
                    if(wobj.def.defName == "MFM_PlaceOfPayment" && cTile == wobj.Tile)
                    {
                        return caravan;
                    }
                }
            }

            return null;
        }

        public static int moneyInCaravan(Caravan caravan)
        {
            int sum = 0;
            foreach (var el in caravan.AllThings)
            {
                if (el.def == ThingDefOf.Silver)
                    sum += el.stackCount;
            }

            return sum;
        }

        public static bool ColonyHasEnoughLocalSilver(Map map, int fee)
        {
            return (from t in AllLocalSilverForTrade(map)
                    where t.def == ThingDefOf.Silver
                    select t).Sum((Thing t) => t.stackCount) >= fee;
        }


        public static IEnumerable<Thing> AllLocalSilverForTrade(Map map)
        {
            HashSet<Thing> yieldedThings = new HashSet<Thing>();
            foreach (Zone current in map.zoneManager.AllZones)
            {
                foreach (IntVec3 c in current.Cells)
                {
                    List<Thing> thingList = c.GetThingList(map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Thing t = thingList[i];
                        if (t.def.category == ThingCategory.Item && PlayerSellableNow(t) && !yieldedThings.Contains(t))
                        {
                            yieldedThings.Add(t);
                            yield return t;
                        }
                    }
                }
            }
        }

        public static bool PlayerSellableNow(Thing t)
        {
            t = t.GetInnerIfMinified();
            if (!TradeUtility.EverPlayerSellable(t.def))
            {
                return false;
            }
            if (t.IsNotFresh())
            {
                return false;
            }
            Apparel apparel = t as Apparel;
            return apparel == null || !apparel.WornByCorpse;
        }


        public static void payLocalSilver(int debt, Map map, ITrader trader)
        {
            while (debt > 0)
            {
                Thing thing = null;
                foreach (Zone current in map.zoneManager.AllZones)
                {
                    foreach (IntVec3 current2 in current.Cells)
                    {
                        foreach (Thing current3 in map.thingGrid.ThingsAt(current2))
                        {
                            if (current3.def == ThingDefOf.Silver)
                            {
                                thing = current3;
                                goto IL_CC;
                            }
                        }
                    }
                }
            IL_CC:
                if (thing == null)
                {
                    Log.Error("Could not find any " + ThingDefOf.Silver + " to transfer to trader.");
                    break;
                }
                int num = Math.Min(debt, thing.stackCount);
                if (trader != null)
                {
                    trader.GiveSoldThingToTrader(thing, num, TradeSession.playerNegotiator);
                }
                else
                {
                    thing.SplitOff(num).Destroy(DestroyMode.Vanish);
                }
                debt -= num;
            }
        }

        public static void caravanPayCost(Caravan caravan, int cost)
        {
            //we withdraw the money from the caravan
            int debt = cost;
            while (debt > 0)
            {
                Thing thing = null;
                foreach (var el in caravan.AllThings)
                {
                    if (el.def == ThingDefOf.Silver)
                    {
                        thing = el;
                        break;
                    }
                }
                int num = Math.Min(debt, thing.stackCount);
                thing.SplitOff(num).Destroy(DestroyMode.Vanish);
                debt -= num;
            }
        }

        /*
         * type is used to discriminate siteofpayments
         */
        public static void createSiteOfPayment(Map map, int type=-1)
        {
            WorldObject sop = WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("MFM_PlaceOfPayment"));
            int dstTile = 0;
            TileFinder.TryFindNewSiteTile(out dstTile, 2, 5, true, TileFinderMode.Near, map.Tile);
            sop.Tile = dstTile;
            if(type != -1)
                sop.creationGameTicks = type;
            Find.WorldObjects.Add(sop);
        }

        public static bool anySiteOfPayment()
        {
            foreach (var wobj in Find.WorldObjects.AllWorldObjects)
            {
                if (wobj.def.defName == "MFM_PlaceOfPayment")
                {
                    return true;
                }
            }
            return false;
        }

        public static void clearAllMedievalSiteOfPayment(int type=-1)
        {
            //Check if no current events requiring sop
            if (Utils.GCMFM.CounterOfferInProgress || Utils.GCMFM.MercWantJoinInProgress || Utils.GCMFM.BillInProgress)
                return;

            List<WorldObject> toDel = new List<WorldObject>();
            foreach (var wobj in Find.WorldObjects.AllWorldObjects)
            {
                if (wobj.def.defName == "MFM_PlaceOfPayment")
                {
                    if(type == -1 || type == wobj.creationGameTicks)
                        toDel.Add(wobj);
                }
            }
            foreach (var e in toDel)
            {
                Find.WorldObjects.Remove(e);
            }
        }

        public static bool isSS_Slave(Pawn pawn)
        {
            return pawn.IsSlave || pawn.health.hediffSet.HasHediff(SS_slaveHediff, false);
        }

        public static void removeKeysStartingWith(Dictionary<string, int> data, string key)
        {
            List<string> toDel = null;
            foreach(var el in data)
            {
                if (el.Key.StartsWith(key))
                {
                    if (toDel == null)
                        toDel = new List<string>();
                    toDel.Add(el.Key);
                }
            }

            if(toDel != null)
            {
                foreach(var e in toDel)
                {
                    data.Remove(e);
                }
            }
        }

        public static string getFirstKeyStartingWith(Dictionary<string, int> data, string key)
        {
            foreach (var el in data)
            {
                if (el.Key.StartsWith(key))
                {
                    return el.Key;
                }
            }
            return null;
        }


        public static void refreshUSFMStuffRule()
        {
            if (Settings.preventNonMercenariesPawnSpawnWithUSFMStuff)
            {
                ThingDef td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt");
                if (td != null)
                {
                    if (td.apparel.tags.Contains("IndustrialBasic"))
                        td.apparel.tags.Remove("IndustrialBasic");
                }

                td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirtCivil");
                if (td != null)
                {
                    if (td.apparel.tags.Contains("IndustrialBasic"))
                        td.apparel.tags.Remove("IndustrialBasic");
                }
            }
            else
            {
                ThingDef td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt");
                if (td != null)
                {
                    if(!td.apparel.tags.Contains("IndustrialBasic"))
                        td.apparel.tags.Add("IndustrialBasic");
                }

                td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirtCivil");
                if (td != null)
                {
                    if (!td.apparel.tags.Contains("IndustrialBasic"))
                        td.apparel.tags.Add("IndustrialBasic");
                }
            }
        }

        public static void setRentedMercSalary(Pawn merc)
        {
            Comp_USFM comp = merc.TryGetComp<Comp_USFM>();

            switch (Utils.getLevelFromSkill(merc, comp.type))
            {
                case MercenaryLevel.Recruit:
                    comp.salary = Settings.priceRecruit;
                    break;
                case MercenaryLevel.Confirmed:
                    comp.salary = Settings.priceConfirmed;
                    break;
                case MercenaryLevel.Veteran:
                    comp.salary = Settings.priceVeteran;
                    break;
                case MercenaryLevel.Elite:
                    comp.salary = Settings.priceElite;
                    break;
                case MercenaryLevel.Cyborg:
                    comp.salary = Settings.priceCyborg;
                    break;
            }

            comp.salary -= (int)(comp.salary * Settings.rateDecreaseAppliedToRentedMercSalary);

            bool isSlave = Utils.isSS_Slave(merc);

            if (isSlave)
            {
                comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
                comp.salary -= ((int)(comp.salary * Settings.percentIncomeDecreaseSlave));
            }
        }

        public static void setHiredMercSalary(Pawn merc)
        {
            Comp_USFM comp = merc.TryGetComp<Comp_USFM>();

            switch (comp.Level)
            {
                case MercenaryLevel.Recruit:
                    comp.salary = Settings.priceRecruit;
                    break;
                case MercenaryLevel.Confirmed:
                    comp.salary = Settings.priceConfirmed;
                    break;
                case MercenaryLevel.Veteran:
                    comp.salary = Settings.priceVeteran;
                    break;
                case MercenaryLevel.Elite:
                    comp.salary = Settings.priceElite;
                    break;
                case MercenaryLevel.Cyborg:
                    comp.salary = Settings.priceCyborg;
                    break;
            }
        }

        /*
         * Check if the player has any imprisoned mercenaries (not belonging to him)
         */
        public static bool isThereImprisonedMercs(Map map, Caravan caravan)
        {
            //Caravan
            if(caravan != null)
            {
                foreach(var p in caravan.pawns)
                {
                    if(p.def.race != null && p.def.race.intelligence == Intelligence.Humanlike)
                    {
                        Comp_USFM comp = p.TryGetComp<Comp_USFM>();
                        if(comp != null && comp.isMercenary && !comp.hiredByPlayer)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (var p in map.mapPawns.FreeColonistsAndPrisonersSpawned)
                {
                    Comp_USFM comp = p.TryGetComp<Comp_USFM>();
                    if (comp != null && p.Faction != Faction.OfPlayer && comp.isMercenary && !comp.hiredByPlayer)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /*
         * Check if the clothes can be worn by the pawn
         */
        public static bool apparelCanBeWearedBy(ThingDef apparelDef, Pawn pawn)
        {
            if (apparelDef == null || apparelDef.apparel == null || !apparelDef.IsApparel)
                return false;

            //Check if the clothes can be worn by the surrogate
            string path = "";
            if (apparelDef.apparel.LastLayer == ApparelLayerDefOf.Overhead)
            {
                path = apparelDef.apparel.wornGraphicPath;
            }
            else
            {
                path = apparelDef.apparel.wornGraphicPath + "_" + pawn.story.bodyType.defName + "_south";
            }

            Texture2D appFoundTex = null;
            Texture2D appTex = null;
            //Check in texture mods
            for (int j = LoadedModManager.RunningModsListForReading.Count - 1; j >= 0; j--)
            {
                appTex = LoadedModManager.RunningModsListForReading[j].GetContentHolder<Texture2D>().Get(path);
                if (appTex != null)
                {
                    appFoundTex = appTex;
                    break;
                }
            }
            //Check RW texture mods
            if (appFoundTex == null)
            {
                path = GenFilePaths.ContentPath<Texture2D>() + path;
                appFoundTex = (Texture2D)((object)Resources.Load<Texture2D>(path));
            }

            return appFoundTex != null;
        }

        private static ResearchProjectDef microElecResearchDef;
    }
}
