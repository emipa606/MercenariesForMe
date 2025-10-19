using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace aRandomKiwi.MFM;

internal static class Utils
{
    private const string ALPHABET = "AG8FOLE2WVTCPY5ZH3NIUDBXSMQK7946";

    public static HediffDef SS_slaveHediff = null;
    public static GC_MFM GCMFM;
    public static bool EVOLOADED = false;
    public static bool CONNLOADED = false;
    public static bool RSBELOADED = false;
    public static bool EPOELOADED = false;
    public static bool CELOADED = false;
    public static bool MSELOADED = false;
    public static bool MEDIEVALTIMESLOADED = false;

    private static readonly List<string> intMedievalStuffException =
        ["Apparel_BasicShirt", "Apparel_Pants", "Apparel_FlakJacket", "Apparel_FlakPants"];

    private static readonly List<string> intBlacklistedWeapons =
    [
        "TornadoGenerator", "OrbitalTargeterPowerBeam", "OrbitalTargeterBombardment", "LS_CloudSword", "LS_Nightfall",
        "LS_Executioner", "LS_Deathbringer", "LS_RedQueen"
    ];

    public static readonly List<string> workTypeDefsToNotClear =
        ["Firefighter", "Patient", "PatientBedRest", "BasicWorker", "Hauling", "Cleaning"];

    public static readonly string SEP = " : ";

    public static readonly string[] types =
    [
        "ranged", "melee", "artist", "builder", "cooker", "farmer", "medical", "miner", "scientist", "speak", "tech",
        "trainer", "carry", "clean"
    ];

    private static ResearchProjectDef microElecResearchDef;

    public static string TranslateTicksToTextIRLSeconds(int ticks)
    {
        //If less than one hour ingame then display seconds
        return ticks < 2500 ? ticks.ToStringSecondsFromTicks() : ticks.ToStringTicksToPeriodVerbose();
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
                var v = (int)type;
                if (v == -1)
                {
                    return Tex.ironAlliance;
                }

                return v == -2 ? Tex.ironAllianceTrader : null;
        }
    }

    private static WorkTags getTypeWorkTag(MercenaryType type)
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
        var skill = getAssociatedSkill(pawn, type);


        if (skill is not { Level: > 6 })
        {
            return MercenaryLevel.Recruit;
        }

        if (skill.Level <= 10)
        {
            return MercenaryLevel.Confirmed;
        }

        if (skill.Level <= 15)
        {
            return MercenaryLevel.Veteran;
        }

        if (skill.Level <= 18 || !modernUSFM())
        {
            return MercenaryLevel.Elite;
        }

        return MercenaryLevel.Cyborg;
    }

    /*
     * Obtaining the skill concerned
     */
    public static SkillRecord getAssociatedSkill(Pawn pawn, MercenaryType type)
    {
        SkillRecord skill = null;

        if (pawn.skills == null)
        {
            return null;
        }

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
    public static Pawn generateMerc(MercenaryType type, MercenaryLevel level, int gear, int weapon, int intGearColor,
        string defNameRace = null, bool purchased = false)
    {
        PawnKindDef pawnKindDef = null;

        if (defNameRace != null)
        {
            pawnKindDef = DefDatabase<PawnKindDef>.GetNamed(defNameRace);
        }

        //We randomly draw a pawnkindDef for the mercenary 
        try
        {
            if (defNameRace == null || pawnKindDef == null)
            {
                pawnKindDef = (from d in DefDatabase<PawnKindDef>.AllDefs
                    where d is { race: not null, RaceProps.Humanlike: true }
                          && !Settings.blacklistedPawnKind.Contains(d.defName)
                    select d).RandomElement();
            }
        }
        catch (Exception)
        {
            // ignored
        }

        pawnKindDef ??= PawnKindDefOf.AncientSoldier;

        //Pawn p = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfPlayer);
        var pgr = new PawnGenerationRequest(pawnKindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, false,
            false, false, false, true, 1, true, false, true, true, false)
        {
            //prevent royalty titles
            ForbidAnyTitle = true,
            //prevent ideo for mercenaries
            ForceNoIdeo = true
        };

        var p = PawnGenerator.GeneratePawn(pgr);

        if (p == null)
        {
            return null;
        }

        if (ModsConfig.IdeologyActive)
        {
            p.ideo.SetIdeo(
                null); // From feedback PawnGenerator.GeneratePawn(pgr) doesnt seem to take ForceNoIdeo into account.
        }

        var cAge = (int)(p.ageTracker.AgeChronologicalTicks / 3600000f);
        if (cAge < Settings.minAge || cAge >= Settings.maxAge)
        {
            p.ageTracker.AgeBiologicalTicks = (long)(Rand.Range(Settings.minAge, Settings.maxAge) * 3600000f);
        }

        //p.workSettings.EnableAndInitialize();

        /**************************************** Traits ***************************************************/
        //Removal of traits that do not comply with mercenary conditions (pyromaniac, nudist, ...)
        List<Trait> traitsToDel = null;
        var contradictedTrait = false;
        foreach (var s in p.story.traits.allTraits)
        {
            var ok2 = (s.def.defName == "BodyPuristDisgust" || s.def.defName == "BodyPurist") &&
                      level == MercenaryLevel.Cyborg
                      || s.def.defName == "Brawler" && type == MercenaryType.Ranged;

            if (ok2)
            {
                contradictedTrait = true;
            }

            if (!Settings.blacklistedTraitsV2.Contains(s.def.DataAtDegree(s.Degree).untranslatedLabel)
                && !ok2)
            {
                continue;
            }

            traitsToDel ??= [];

            traitsToDel.Add(s);
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
            var lst = new List<HediffWithComps>();
            p.health.hediffSet.GetHediffs(ref lst);
            foreach (var h in lst)
            {
                if (!Settings.hediffToNotClear.Contains(h.def.defName))
                {
                    p.health.hediffSet.hediffs.Remove(h);
                }
            }
        }


        var compUSFM = p.TryGetComp<Comp_USFM>();
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
                //List<Hediff> addedHeddiffs = new List<Hediff>();

                var allParts = p.def.race.body.AllParts;
                var bionicSource = Settings.getBionicSource();
                foreach (var bp in allParts)
                {
                    HediffDef h = null;
                    switch (bionicSource)
                    {
                        //VANILLA
                        case 0:
                            if (bp.def == BodyPartDefOf.Eye)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("ArchotechEye");
                            }
                            else if (bp.def == BodyPartDefOf.Heart && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("BionicHeart");
                            }
                            else if (bp.def == BodyPartDefOf.Arm)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("ArchotechArm");
                            }
                            else if (bp.def == BodyPartDefOf.Leg)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("ArchotechLeg");
                            }
                            else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("BionicStomach");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("BionicEar");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("BionicSpine");
                            }

                            break;
                        //EPOE
                        case 1:
                            if (bp.def == BodyPartDefOf.Eye)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicEye");
                            }
                            else if (bp.def == BodyPartDefOf.Heart && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticHeart");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Kidney") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticKidney");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Liver") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticLiver");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticLung");
                            }
                            else if (bp.def == BodyPartDefOf.Arm)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedPowerArm");
                            }
                            else if (bp.def == BodyPartDefOf.Leg)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicLeg");
                            }
                            else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticStomach");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicEar");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicSpine");
                            }

                            break;
                        //RSBE
                        case 2:
                            if (bp.def == BodyPartDefOf.Eye)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("ArchotechEye");
                            }
                            else if (bp.def == BodyPartDefOf.Heart && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticHeart");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Kidney") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticKidney");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Liver") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticLiver");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticLung");
                            }
                            else if (bp.def == BodyPartDefOf.Arm)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedPowerArm");
                            }
                            else if (bp.def == BodyPartDefOf.Leg)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicLeg");
                            }
                            else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("SyntheticStomach");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicEar");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("AdvancedBionicSpine");
                            }
                            else if (bp.def == BodyPartDefOf.Torso)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("ExoskeletonSuit");
                            }

                            break;
                        //CONN
                        case 3:
                            if (bp.def != null && bp.def.tags.Contains(BodyPartTagDefOf.ConsciousnessSource))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_CombatAI");
                            }
                            else if (bp.def == BodyPartDefOf.Eye)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_CyclopsVisor");
                            }
                            else if (bp.def == BodyPartDefOf.Heart && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_Lionheart");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_Shadowrunner");
                            }
                            else if (bp.def == BodyPartDefOf.Arm)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_PowerArms");
                            }
                            else if (bp.def == BodyPartDefOf.Leg)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_ElasticAchilles");
                            }
                            else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("BionicStomach");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_SensoricEarside");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("BionicSpine");
                            }
                            else if (bp.def == BodyPartDefOf.Torso)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("Trunken_hediff_ExoskeletonArmor");
                            }

                            break;
                        //Evolved Organs :
                        case 4:
                            if (bp.def != null && bp.def.tags.Contains(BodyPartTagDefOf.ConsciousnessSource))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedBrain");
                            }
                            else if (bp.def == BodyPartDefOf.Eye)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedEye");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Nose"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedNose");
                            }
                            else if (bp.def == BodyPartDefOf.Heart && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedPrimaryHeart");
                            }
                            else if (bp.def == BodyPartDefOf.Arm)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedArm");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Lung") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedLung");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Kidney") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedKidney");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Liver") && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedLiver");
                            }
                            else if (bp.def == BodyPartDefOf.Leg)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedLeg");
                            }
                            else if (p.def != null && bp.def.tags.Contains(BodyPartTagDefOf.EatingSource) && !MSELOADED)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedStomach");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Ear"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedEar");
                            }
                            else if (bp.def == DefDatabase<BodyPartDef>.GetNamed("Spine"))
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedSpine");
                            }
                            else if (bp.def == BodyPartDefOf.Torso)
                            {
                                h = DefDatabase<HediffDef>.GetNamed("EvolvedCarapace");
                            }

                            break;
                    }

                    if (h != null)
                    {
                        p.health.AddHediff(h, bp);
                    }
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
                    var val = 1;
                    if (bionicSource == 0)
                    {
                        val = 0;
                    }

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < p.health.hediffSet.hediffs.Count; i++)
                    {
                        var h = p.health.hediffSet.hediffs[i];
                        Traverse.Create(h).Field("severityInt").SetValue(val);
                        p.health.Notify_HediffChanged(h);
                    }
                }

                break;
        }

        p.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("Mercenary4");
        p.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("MercenaryRecruit36");

        //If childhood disable the function concerned or shoot and mixed then it is changed by a generic
        var skill = getAssociatedSkill(p, type);
        var skillMeleeGen = getAssociatedSkill(p, MercenaryType.Melee);
        var skillShootGen = getAssociatedSkill(p, MercenaryType.Ranged);


        if (skill == null || skillMeleeGen == null || skillShootGen == null)
        {
            //Log.Error("GenerateMerc : unable to find related skill");
            return null;
        }

        var curTypeWT = getTypeWorkTag(type);
        var condMH = Settings.mustHaveTraits.Count != 0;

        if (skill.TotallyDisabled
            || contradictedTrait
            || condMH
            || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(curTypeWT)
            || skillMeleeGen.TotallyDisabled && skillShootGen.TotallyDisabled
            || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(WorkTags.Violent)
            || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(WorkTags.Cleaning)
            || p.story.Childhood.workDisables.OverlapsWithOnAnyWorkType(WorkTags.Hauling))
        {
            if (condMH)
            {
                foreach (var trait in Settings.mustHaveTraits)
                {
                    var found = false;
                    foreach (var ct in DefDatabase<TraitDef>.AllDefs)
                    {
                        foreach (var ctd in ct.degreeDatas)
                        {
                            if (ctd.untranslatedLabel != trait)
                            {
                                continue;
                            }

                            p.story.traits.GainTrait(new Trait(ct, ctd.degree, true));
                            found = true;
                            break;
                        }

                        if (found)
                        {
                            break;
                        }
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
            Log.Message($"[MFM] {e.Message}");
        }

        try
        {
            processMercWeapon(p, type, weapon);
        }
        catch (Exception e)
        {
            Log.Message($"[MFM] {e.Message}");
        }

        //Pawn pawn = (Pawn)GenSpawn.Spawn(newThing, Find.CurrentMap.Center, Find.CurrentMap, WipeMode.Vanish);

        return p;
    }

    private static void clearMercGear(Pawn p)
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
        var selWeapon = "";
        var melee = getAssociatedSkill(p, MercenaryType.Melee);
        var shoot = getAssociatedSkill(p, MercenaryType.Ranged);

        int minRanged;
        int maxRanged;
        int minMelee;
        int maxMelee;


        switch (weapon)
        {
            case 4:
                if (Settings.onlyVanillaForWeapon)
                {
                    //If soldier type melee or stats in melee higher than ranged
                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
                    {
                        selWeapon = "ThrumboHorn";

                        //Shield
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt"),
                            GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt")));
                        if (clothe != null)
                        {
                            try
                            {
                                p.apparel.Wear(clothe, false);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    }
                    else
                    {
                        if (Rand.Chance(0.5f))
                        {
                            selWeapon = "Gun_Minigun";
                        }
                        else
                        {
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
                }
                else
                {
                    if (modernUSFM())
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

                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
                    {
                        selWeapon = getRandomWeapon(true, minMelee, maxMelee, false);
                    }
                    else
                    {
                        selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                }

                cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                if (cw != null)
                {
                    p.equipment.AddEquipment(cw);
                }

                break;
            case 3:
                if (Settings.onlyVanillaForWeapon)
                {
                    //If soldier type melee or stats in melee higher than ranged
                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
                    {
                        selWeapon = "MeleeWeapon_LongSword";

                        //Shield
                        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt"),
                            GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_ShieldBelt")));
                        p.apparel.Wear(clothe, false);
                    }
                    else
                    {
                        if (modernUSFM())
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
                        {
                            selWeapon = "Bow_Great";
                        }
                    }
                }
                else
                {
                    if (modernUSFM())
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

                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
                    {
                        selWeapon = getRandomWeapon(true, minMelee, maxMelee, false);
                    }
                    else
                    {
                        selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                }

                cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                if (cw != null)
                {
                    p.equipment.AddEquipment(cw);
                }

                break;
            case 2:
                if (Settings.onlyVanillaForWeapon)
                {
                    //If soldier type melee or stats in melee higher than ranged
                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
                    {
                        selWeapon = "MeleeWeapon_Gladius";
                    }
                    else
                    {
                        if (modernUSFM())
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
                        {
                            selWeapon = "Bow_Recurve";
                        }
                    }
                }
                else
                {
                    if (modernUSFM())
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

                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
                    {
                        selWeapon = getRandomWeapon(true, minMelee, maxMelee, false);
                    }
                    else
                    {
                        selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                }

                cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                if (cw != null)
                {
                    p.equipment.AddEquipment(cw);
                }

                break;
            case 1:
                if (Settings.onlyVanillaForWeapon)
                {
                    //If soldier type melee or stats in melee higher than ranged
                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
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
                        if (modernUSFM())
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
                        {
                            selWeapon = "Bow_Short";
                        }
                    }
                }
                else
                {
                    if (modernUSFM())
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

                    if (type == MercenaryType.Melee || type != MercenaryType.Ranged && melee.levelInt > shoot.levelInt)
                    {
                        selWeapon = getRandomWeapon(true, minMelee, maxMelee, false);
                    }
                    else
                    {
                        selWeapon = getRandomWeapon(false, minRanged, maxRanged);
                    }
                }

                cw = (ThingWithComps)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(selWeapon),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed(selWeapon)));
                if (cw != null)
                {
                    p.equipment.AddEquipment(cw);
                }

                break;
        }

        if (!CELOADED || cw == null)
        {
            return;
        }

        var ammoUser = TryGetCompByTypeName(cw, "CompAmmoUser");
        if (ammoUser == null)
        {
            return;
        }

        var props = Traverse.Create(ammoUser).Property("Props").GetValue();
        Traverse.Create(props).Field("magazineSize").GetValue<int>();
        var def = Traverse.Create(ammoUser).Field("selectedAmmo").GetValue<ThingDef>();
        if (def != null)
        {
            Traverse.Create(ammoUser).Method("ResetAmmoCount", def).GetValue();
        }
    }

    private static ThingComp TryGetCompByTypeName(ThingWithComps thing, string typeName)
    {
        return thing.AllComps.FirstOrDefault(comp => comp.GetType().Name == typeName);
    }

    public static void processMercGear(Pawn p, MercenaryType type, int gear, int intGearColor)
    {
        if (p.apparel == null)
        {
            Log.Error("[MFM] apparel == null on processMercGear");
            return;
        }

        /**************************************** Clothes *************************************************/

        var gearColor = getGearColor(intGearColor);

        //================================= Adding free basic clothing
        Apparel clothe = null;
        ThingDef onSkinTorso = null;
        var min = 0;
        var max = 0;
        var minHat = 0;
        var maxHat = 0;
        var blacklist = new List<string>();
        var whitelist = new List<string>();
        var layers = new List<ApparelLayerDef>();
        var blacklistedLayers = new List<ApparelLayerDef>();

        if (gear == 3)
        {
            if (modernUSFM())
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
            if (modernUSFM())
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
            if (modernUSFM())
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

        if (gear != 0)
        {
            if (Settings.onlyVanillaForGear)
            {
                if (modernUSFM())
                {
                    processMercGearVanilla(p, gear, intGearColor);
                }
                else
                {
                    processMercGearVanillaMedieval(p, gear, intGearColor);
                }
            }
            else
            {
                layers.Add(ApparelLayerDefOf.Overhead);
                var td = getRandomGear(p, BodyPartGroupDefOf.UpperHead, BodyPartGroupDefOf.FullHead, layers, minHat,
                    maxHat,
                    whitelist, blacklist);
                if (td != null)
                {
                    clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                    if (clothe != null)
                    {
                        if (clothe.TryGetComp<CompColorable>() != null)
                        {
                            clothe.SetColor(gearColor);
                        }

                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        if (p.outfits is { forcedHandler: not null })
                        {
                            p.outfits.forcedHandler.SetForced(clothe, true);
                        }
                    }
                }


                layers.Clear();
                layers.Add(ApparelLayerDefOf.OnSkin);
                td = getRandomGear(p, BodyPartGroupDefOf.Torso, null, layers, min, max, whitelist, blacklist, true) ??
                     getRandomGear(p, BodyPartGroupDefOf.Torso, null, layers, 1, max, whitelist, blacklist, true);

                if (td != null)
                {
                    onSkinTorso = td;
                    //Log.Message(td.defName);
                    clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                    if (clothe != null)
                    {
                        if (clothe.TryGetComp<CompColorable>() != null)
                        {
                            clothe.SetColor(gearColor);
                        }

                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        if (p.outfits is { forcedHandler: not null })
                        {
                            p.outfits.forcedHandler.SetForced(clothe, true);
                        }
                    }
                }

                layers.Clear();
                layers.Add(ApparelLayerDefOf.Shell);
                layers.Add(ApparelLayerDefOf.Middle);
                blacklistedLayers.Add(ApparelLayerDefOf.OnSkin);
                td = getRandomGear(p, BodyPartGroupDefOf.Torso, null, layers, min, max, whitelist, blacklist, false,
                    blacklistedLayers);
                if (td != null)
                {
                    //Log.Message(td.defName);
                    clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                    if (clothe != null)
                    {
                        if (clothe.TryGetComp<CompColorable>() != null)
                        {
                            clothe.SetColor(gearColor);
                        }

                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        if (p.outfits is { forcedHandler: not null })
                        {
                            p.outfits.forcedHandler.SetForced(clothe, true);
                        }
                    }
                    else
                    {
                        td = null;
                    }
                }

                //If the bodyarmo also includes the bottom, we squeeze this part
                if (onSkinTorso == null || !onSkinTorso.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs))
                {
                    var onlyOnSkin = true;
                    layers.Clear();
                    if (td == null || !td.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.Legs))
                    {
                        onlyOnSkin = false;
                        layers.Add(ApparelLayerDefOf.Middle);
                    }

                    layers.Add(ApparelLayerDefOf.OnSkin);
                    td = getRandomGear(p, BodyPartGroupDefOf.Legs, null, layers, min, max, whitelist, blacklist,
                        onlyOnSkin, null, true) ?? getRandomGear(p, BodyPartGroupDefOf.Legs, null, layers, 1, max,
                        whitelist, blacklist,
                        onlyOnSkin, null, true);

                    if (td != null)
                    {
                        //Log.Message(td.defName);
                        clothe = (Apparel)ThingMaker.MakeThing(td, GenStuff.DefaultStuffFor(td));
                        if (clothe != null)
                        {
                            if (clothe.TryGetComp<CompColorable>() != null)
                            {
                                clothe.SetColor(gearColor);
                            }

                            try
                            {
                                p.apparel.Wear(clothe, false);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                            if (p.outfits is { forcedHandler: not null })
                            {
                                p.outfits.forcedHandler.SetForced(clothe, true);
                            }
                        }
                    }
                }
            }
        }

        if (modernUSFM())
        {
            if (Settings.onlyVanillaForGear || gear == 0)
            {
                //Depending on the type of mercenary (military or civilian, they are assigned a basic outfit)
                if (type == MercenaryType.Melee || type == MercenaryType.Ranged)
                {
                    //Combat uniform
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt")));
                    if (clothe != null)
                    {
                        clothe.SetColor(Color.white);
                        p.apparel.Wear(clothe, false);
                    }
                }
                else
                {
                    //Civilian uniform
                    clothe = (Apparel)ThingMaker.MakeThing(
                        DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirtCivil"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt")));
                    var color = getColorForType(type);
                    if (clothe != null)
                    {
                        clothe.SetColor(color);
                        try
                        {
                            p.apparel.Wear(clothe, false);
                        }
                        catch (Exception)
                        {
                            // ignored
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
                clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_TribalA"),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_TribalA")));
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
                        // ignored
                    }
                }
            }
        }

        if (clothe != null && p.outfits is { forcedHandler: not null })
        {
            p.outfits.forcedHandler.SetForced(clothe, true);
        }

        if (!modernUSFM())
        {
            return;
        }

        //Add generic pants if necessary
        if ((!Settings.onlyVanillaForGear || gear == 2) && gear != 0)
        {
            return;
        }

        //Generic pants with two types of mercenaries
        clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Pants"),
            GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Pants")));
        if (clothe == null)
        {
            return;
        }

        clothe.SetColor(Color.white);
        try
        {
            p.apparel.Wear(clothe, false);
        }
        catch (Exception)
        {
            // ignored
        }

        if (p.outfits is { forcedHandler: not null })
        {
            p.outfits.forcedHandler.SetForced(clothe, true);
        }
    }

    private static void processMercGearVanilla(Pawn p, int gear, int gearColor)
    {
        Apparel clothe;
        var defColor = getGearColor(gearColor);

        switch (gear)
        {
            case 3:
                //Marine armor
                clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmor"),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmor")));
                if (clothe != null)
                {
                    clothe.SetColor(defColor);
                    try
                    {
                        p.apparel.Wear(clothe, false);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    //Marine helmet
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmorHelmet"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_PowerArmorHelmet")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                }

                break;
            case 2:
                //Flak armor
                clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_FlakJacket"),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_FlakJacket")));
                if (clothe != null)
                {
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_FlakPants"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_FlakPants")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                    //Advanced headset
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                }

                break;
            case 1:
                clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_FlakVest"),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_FlakVest")));
                if (clothe != null)
                {
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                    //Advanced headset
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                }

                break;
        }
    }

    private static void processMercGearVanillaMedieval(Pawn p, int gear, int gearColor)
    {
        Apparel clothe;
        var defColor = getGearColor(gearColor);

        switch (gear)
        {
            case 3:
                //Marine armor
                clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_PlateArmor"),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_PlateArmor")));
                if (clothe != null)
                {
                    clothe.SetColor(defColor);
                    try
                    {
                        p.apparel.Wear(clothe, false);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                }

                break;
            case 2:
                clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Parka"),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Parka")));
                if (clothe != null)
                {
                    clothe.SetColor(defColor);
                    try
                    {
                        p.apparel.Wear(clothe, false);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    //Single helmet
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_SimpleHelmet"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_AdvancedHelmet")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                    //Trousers
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Pants"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Pants")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                }

                break;
            case 1:
                clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_BasicShirt"),
                    GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_BasicShirt")));
                if (clothe != null)
                {
                    clothe.SetColor(defColor);
                    try
                    {
                        p.apparel.Wear(clothe, false);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    //Single helmet
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_WarMask"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_WarMask")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                    //Trousers
                    clothe = (Apparel)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Apparel_Pants"),
                        GenStuff.DefaultStuffFor(DefDatabase<ThingDef>.GetNamed("Apparel_Pants")));
                    clothe.SetColor(defColor);
                    p.apparel.Wear(clothe, false);
                }

                break;
        }
    }

    public static string serializeLP(List<Pawn> lp, Map backMap)
    {
        var tmp = new List<string>();
        foreach (var entry in lp)
        {
            tmp.Add(entry.GetUniqueLoadID());
        }

        tmp.Add(backMap.GetUniqueLoadID());

        return serializeLS(tmp);
    }

    public static List<Pawn> unserializeLP(string data, out Map backMap)
    {
        backMap = null;
        var tmp = unserializeLS(data);
        var ret = new List<Pawn>();

        foreach (var entry in tmp)
        {
            foreach (var p in GCMFM.getRentedPawns())
            {
                if (p.GetUniqueLoadID() == entry)
                {
                    ret.Add(p);
                    break;
                }

                if (entry.StartsWith("Map"))
                {
                    backMap = getMapFromMUID(entry);
                }
            }
        }

        return ret;
    }

    public static string serializeLS(List<string> lst)
    {
        return string.Join(",", lst.ToArray());
    }

    public static List<string> unserializeLS(string data)
    {
        var ret = data.Split(',').ToList();

        return ret;
    }

    public static void unserializePowerBeamAttack(string data, out Map map, out IntVec3 coord)
    {
        var ret = data.Split(',');

        coord = default;
        map = null;

        if (ret.Length != 4)
        {
            return;
        }

        map = getMapFromMUID(ret[0]);
        coord = new IntVec3(int.Parse(ret[1]), int.Parse(ret[2]), int.Parse(ret[3]));
    }

    public static Map getRandomMapOfPlayer()
    {
        //Commenté par Ionfrigate pour update 1.5: Cette fonction n'est pas "Random", en plus elle n'exclut pas les map SOS2 / Rimnauts 2. J'utilise ma propre fonction à la place.
        /*Map ret = null;
        foreach(var map in Find.Maps)
        {
            if (map.IsPlayerHome)
            {
                ret = map;
                break;
            }
        }
        return ret;*/
        return HarmonyUtils.GetPlayerMainColonyMap(true, false);
    }

    /*
     * Serialize a (String,Int) dictionary
     */
    public static string serializeDSI(Dictionary<string, int> data)
    {
        var ret = "";
        foreach (var entry in data)
        {
            ret += $"{entry.Key}:{entry.Value}§";
        }

        return ret;
    }

    /*
     * Unserialize a (String, Int) dictionary
     */
    public static Dictionary<string, int> unserializeDSI(string data)
    {
        if (data == "")
        {
            return null;
        }

        var els = data.Split('§');
        var ret = new Dictionary<string, int>();
        foreach (var el in els)
        {
            var p = el.Split(':');
            if (p.Length == 2)
            {
                ret[p[0]] = int.Parse(p[1]);
            }
        }

        return ret;
    }

    /*
     * Serialize a (String,Int) dictionary
     */
    public static string serializeDSS(Dictionary<string, string> data)
    {
        var ret = "";
        foreach (var entry in data)
        {
            ret += $"{entry.Key}:{entry.Value}§";
        }

        return ret;
    }

    /*
     * Unserialize a (String, Int) dictionary
     */
    public static Dictionary<string, string> unserializeDSS(string data)
    {
        if (data == "")
        {
            return null;
        }

        var els = data.Split('§');
        var ret = new Dictionary<string, string>();
        foreach (var el in els)
        {
            var p = el.Split(':');
            if (p.Length == 2)
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
        foreach (var map in Find.Maps)
        {
            if (map.uniqueID == UID)
            {
                return map;
            }
        }

        return null;
    }

    /*
     * Obtaining Map from MUID
     */
    private static Map getMapFromMUID(string MUID)
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

    private static Color getColorForType(MercenaryType type)
    {
        var color = default(Color);
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
        var incapableList = new List<string>();
        var combinedDisabledWorkTags = pawn.CombinedDisabledWorkTags;
        if (combinedDisabledWorkTags == WorkTags.None)
        {
            return;
        }

        var list = (IEnumerable<WorkTags>)typeof(CharacterCardUtility)
            .GetMethod("WorkTagsFrom", BindingFlags.Static | BindingFlags.NonPublic)
            ?.Invoke(null, [combinedDisabledWorkTags]);
        if (list == null)
        {
            return;
        }

        foreach (var tag in list)
        {
            incapableList.Add(tag.LabelTranslated().CapitalizeFirst());
        }
    }

    private static void ClearCachedDisabledWorkTypes(this Pawn pawn)
    {
        if (pawn != null)
        {
            typeof(Pawn).GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(pawn, null);
        }
    }

    private static void ClearCachedDisabledSkillRecords(this Pawn pawn)
    {
        if (pawn.skills is not { skills: not null })
        {
            return;
        }

        var field = typeof(SkillRecord).GetField("cachedTotallyDisabled",
            BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var record in pawn.skills.skills)
        {
            field?.SetValue(record, BoolUnknown.Unknown);
        }
    }

    /*
     * Check if a pawn can be recruited as a mercenary
     */
    public static bool canBeMercenary(Pawn pawn, out string msg)
    {
        var ret = true;
        msg = !Settings.allowNonViolentToBeRented
            ? "MFM_MsgPawnCannotBeRentedAsMerc".Translate(pawn.LabelCap)
            : "MFM_MsgPawnCannotBeRentedAsMerc2".Translate(pawn.LabelCap);

        //If invalid trait
        foreach (var s in pawn.story.traits.allTraits)
        {
            if (!Settings.blacklistedTraitsV2.Contains(s.def.DataAtDegree(s.Degree).untranslatedLabel))
            {
                continue;
            }

            //Log.Message("=>>" + s.def.defName);
            ret = false;
            break;
        }

        //If you must have a trait and he does not
        if (Settings.mustHaveTraits.Count != 0)
        {
            foreach (var trait in Settings.mustHaveTraits)
            {
                var found = false;
                foreach (var ct in DefDatabase<TraitDef>.AllDefs)
                {
                    foreach (var ctd in ct.degreeDatas)
                    {
                        if (ctd.untranslatedLabel != trait)
                        {
                            continue;
                        }

                        found = true;
                        break;
                    }

                    if (found)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    "MFM_MsgPawnCannotBeRentedAsMercBecauseMissingTrait".Translate(pawn.LabelCap);
                }
            }
        }

        if (!Settings.allowNonViolentToBeRented)
        {
            var skillMeleeGen = getAssociatedSkill(pawn, MercenaryType.Melee);
            var skillShootGen = getAssociatedSkill(pawn, MercenaryType.Ranged);

            //Si fragile
            if (skillMeleeGen.TotallyDisabled && skillShootGen.TotallyDisabled)
            {
                ret = false;
            }
        }

        var comp = pawn.TryGetComp<Comp_USFM>();
        //If already a recruited mercenary ==> no
        if (comp.isMercenary)
        {
            ret = false;
        }

        var CGT = Find.TickManager.TicksGame;
        //If fired ===> no
        if (comp.firedGT > CGT)
        {
            msg = "MFM_MsgCannotBeRentedAsMercBecauseFired".Translate(pawn.LabelCap, getUSFMLabel(),
                (comp.firedGT - CGT).ToStringTicksToPeriodVerbose());
            ret = false;
        }

        if (!pawn.Downed && (pawn.health == null || !(pawn.health.summaryHealth.SummaryHealthPercent < 0.75f)))
        {
            return ret;
        }

        msg = "MFM_MsgCannotBeRentedAsMercBecauseBadHealth".Translate(pawn.LabelCap);

        return false;
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
        var ret = Find.TickManager.TicksGame;
        switch (delivery)
        {
            case 2:
                ret += Rand.Range(Settings.transportQuickMinHour * 2500, Settings.transportQuickMaxHour * 2500);
                break;
            case 1:
                ret += Rand.Range(500, 1250);
                break;
            default:
                ret += Rand.Range(Settings.transportStandardMinHour * 2500, Settings.transportStandardMaxHour * 2500);
                break;
        }

        return ret;
    }


    public static void getWantedInfo(Dictionary<string, int> wanted, out int nbMerc, out int price)
    {
        nbMerc = 0;
        price = 0;

        foreach (var type in (MercenaryType[])Enum.GetValues(typeof(MercenaryType)))
        {
            var index1 = GC_MFM.buildStockIndex(type, MercenaryLevel.Recruit);
            var index2 = GC_MFM.buildStockIndex(type, MercenaryLevel.Confirmed);
            var index3 = GC_MFM.buildStockIndex(type, MercenaryLevel.Veteran);
            var index4 = GC_MFM.buildStockIndex(type, MercenaryLevel.Elite);
            var index5 = GC_MFM.buildStockIndex(type, MercenaryLevel.Cyborg);

            if (wanted[index1] != 0)
            {
                price += wanted[index1] * Settings.priceRecruit;
            }
            else if (wanted[index2] != 0)
            {
                price += wanted[index2] * Settings.priceConfirmed;
            }
            else if (wanted[index3] != 0)
            {
                price += wanted[index3] * Settings.priceVeteran;
            }
            else if (wanted[index4] != 0)
            {
                price += wanted[index4] * Settings.priceElite;
            }
            else if (wanted[index5] != 0)
            {
                price += wanted[index5] * Settings.priceCyborg;
            }

            nbMerc += wanted[index1] + wanted[index2] + wanted[index3] + wanted[index4] + wanted[index5];
        }
    }


    /*
     * Random return of a device intended for a specified part of a body and having a value between the two limits
     * And NO tribal
     */
    private static ThingDef getRandomGear(Pawn p, BodyPartGroupDef body, BodyPartGroupDef body2,
        List<ApparelLayerDef> layers, int min, int max, List<string> priceWhitelist, List<string> priceBlacklist,
        bool onlySpecifiedLayer = false, List<ApparelLayerDef> blacklistedLayers = null,
        bool onlySpecifiedBodypart = false)
    {
        var medievalMode = !modernUSFM();

        var ret = DefDatabase<ThingDef>.AllDefsListForReading.Where(delegate(ThingDef x)
        {
            if (!x.IsApparel)
            {
                return false;
            }

            var layersOk = false;
            var bodyPartOK = false;


            if (layers != null && layers.Count != 0)
            {
                if (onlySpecifiedLayer)
                {
                    layersOk = true;
                }

                foreach (var l in layers)
                {
                    if (onlySpecifiedLayer)
                    {
                        foreach (var l2 in x.apparel.layers)
                        {
                            if (layers.Contains(l2))
                            {
                                continue;
                            }

                            layersOk = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!x.apparel.layers.Contains(l))
                        {
                            continue;
                        }

                        layersOk = true;
                        break;
                    }
                }
            }
            else
            {
                layersOk = true;
            }

            if (layersOk && blacklistedLayers != null)
            {
                foreach (var el in x.apparel.layers)
                {
                    if (!blacklistedLayers.Contains(el))
                    {
                        continue;
                    }

                    layersOk = false;
                    break;
                }
            }


            if (onlySpecifiedBodypart)
            {
                foreach (var el in x.apparel.bodyPartGroups)
                {
                    if (body == el || body2 == el)
                    {
                        bodyPartOK = true;
                    }
                    else
                    {
                        bodyPartOK = false;
                        break;
                    }
                }
            }
            else
            {
                bodyPartOK = x.apparel.bodyPartGroups.Contains(body) ||
                             body2 != null && x.apparel.bodyPartGroups.Contains(body2);
            }


            var priceOK = x.BaseMarketValue >= min && x.BaseMarketValue <= max;

            /*if (layersOk
            && bodyPartOK
            && ((priceOK && !priceBlacklist.Contains(x.defName)) || (!priceOK && priceWhitelist.Contains(x.defName)))
            && ((medievalMode && x.techLevel <= TechLevel.Medieval) || (!medievalMode && (x.techLevel > TechLevel.Medieval || intMedievalStuffException.Contains(x.defName)))))
            {
                Log.Message("=>" + x.defName + " " + x.BaseMarketValue + " " + bodyPartOK + " " + layersOk + " " + (x.apparel.bodyPartGroups.Contains(body) || (body2 != null && x.apparel.bodyPartGroups.Contains(body2))) + " " + ((priceOK && !priceBlacklist.Contains(x.defName)) || (!priceOK && priceWhitelist.Contains(x.defName))) + " " + ((medievalMode && x.techLevel <= TechLevel.Medieval) || (!medievalMode && (x.techLevel > TechLevel.Medieval || intMedievalStuffException.Contains(x.defName)))));
            }*/

            return layersOk
                   && apparelCanBeWearedBy(x, p)
                   && bodyPartOK
                   && (priceOK && !priceBlacklist.Contains(x.defName) || !priceOK && priceWhitelist.Contains(x.defName))
                   && (medievalMode && x.techLevel <= TechLevel.Medieval || !medievalMode &&
                       (x.techLevel > TechLevel.Medieval || intMedievalStuffException.Contains(x.defName)));
        });
        return !ret.Any() ? null : ret.RandomElement();
    }


    /*
     * Randomly return a weapon with a value between the two limits
     * And NO tribal
     */
    private static string getRandomWeapon(bool melee, int min, int max, bool minTechLevelMedieval = true)
    {
        var medievalMode = !modernUSFM();

        var ret = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.IsWeapon
                                                                         && x.BaseMarketValue >= min &&
                                                                         x.BaseMarketValue <= max
                                                                         && (melee && x.IsMeleeWeapon ||
                                                                             !melee && x.IsRangedWeapon)
                                                                         && (medievalMode &&
                                                                             x.techLevel <= TechLevel.Medieval ||
                                                                             !medievalMode &&
                                                                             (minTechLevelMedieval &&
                                                                              x.techLevel > TechLevel.Medieval ||
                                                                              !minTechLevelMedieval))
                                                                         && !Settings.blacklistedWeapons.Contains(
                                                                             x.defName) &&
                                                                         !intBlacklistedWeapons.Contains(x.defName));

        if (!ret.Any())
        {
            return null;
        }

        var cdef = ret.RandomElement();
        //Log.Message(cdef.defName + " " + cdef.BaseMarketValue+" "+(!intBlacklistedWeapons.Contains(cdef.defName)));
        return cdef.defName;
    }

    public static string getColorText(int color)
    {
        string ret;
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


    private static Color getGearColor(int color)
    {
        Color ret;
        switch ((Colors)color)
        {
            case Colors.Blue:
                ret = new Color(0.2f, 0.345f, 0.756f, 1.0f);
                break;
            case Colors.Gray:
                ret = new Color(0.515f, 0.515f, 0.515f, 1.0f);
                break;
            case Colors.Green:
                ret = new Color(0.133f, 0.623f, 0.113f, 1.0f);
                break;
            case Colors.Orange:
                ret = new Color(0.949f, 0.603f, 0.109f, 1.0f);
                break;
            case Colors.Pink:
                ret = new Color(0.956f, 0.341f, 0.850f, 1.0f);
                break;
            case Colors.Purple:
                ret = new Color(0.486f, 0.113f, 0.647f, 1.0f);
                break;
            case Colors.Red:
                ret = new Color(0.941f, 0f, 0.137f, 1.0f);
                break;
            case Colors.White:
                ret = new Color(1f, 1f, 1f, 1f);
                break;
            case Colors.Yellow:
                ret = new Color(0.921f, 0.8f, 0.117f, 1f);
                break;
            case Colors.Cyan:
                ret = new Color(0.180f, 0.898f, 0.890f, 1f);
                break;
            default:
            case Colors.Black:
                ret = new Color(0.25f, 0.25f, 0.25f, 1.0f);
                break;
        }

        return ret;
    }

    public static bool anyPlayerColonnyHasEnoughtSilver(int price)
    {
        foreach (var map in Find.Maps)
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
            if (!map.IsPlayerHome || !TradeUtility.ColonyHasEnoughSilver(map, price))
            {
                continue;
            }

            TradeUtility.LaunchSilver(map, price);
            break;
        }
    }

    /*
     * Calculation of the pawn score
     */
    public static float getPawnScore(Pawn pawn)
    {
        var score =
            //Calculation of raw score
            pawn.kindDef.combatPower * pawn.health.summaryHealth.SummaryHealthPercent;

        //the adjustment is only carried out on 50% of the score we bring the adjustment to a value between 0 and 1
        var adjust = Math.Min(1.0f, getPawnScoreBasic(pawn) + 0.5f);

        //Log.Message(pawn.LabelCap+" " + adjust.ToString()+" ( "+lvlConsciousness+" "+lvlManipulation+" "+lvlMoving+" "+lvlSight+" )");

        return score * adjust;
    }

    public static float getPawnScoreBasic(Pawn pawn)
    {
        //Smoothing of the score with the potential defaults at the level of the capacities (consciousness, manipulation, Moving, Sight) of the pawn
        var lvlConsciousness =
            Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Consciousness),
                0.15f);
        var lvlManipulation =
            Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Manipulation),
                0.15f);
        var lvlMoving =
            Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Moving),
                0.15f);
        var lvlSight =
            Math.Max(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, PawnCapacityDefOf.Sight), 0.15f);

        return lvlConsciousness * lvlManipulation * lvlMoving * lvlSight;
    }

    public static void startPowerBeamAt(Map map, IntVec3 pos)
    {
        var powerBeam = (PowerBeam)GenSpawn.Spawn(ThingDefOf.PowerBeam, pos, map);
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
        var ret = new List<Pawn>();
        foreach (var m in Find.ColonistBar.Entries)
        {
            var comp = m.pawn.TryGetComp<Comp_USFM>();
            if (comp is { isMercenary: true, hiredByPlayer: true })
            {
                ret.Add(m.pawn);
            }
        }

        //Added prisoners having hiredByPlayer == true
        foreach (var map in Find.Maps)
        {
            foreach (var p in map.mapPawns.PrisonersOfColonySpawned)
            {
                var comp = p.TryGetComp<Comp_USFM>();
                if (comp is { hiredByPlayer: true })
                {
                    ret.Add(p);
                }
            }
        }

        return ret;
    }


    public static void showDiscountCodeList(Action<string> action)
    {
        var opts = new List<FloatMenuOption>();

        foreach (var m in GCMFM.getDiscountList())
        {
            opts.Add(new FloatMenuOption(m.Key, delegate { action(m.Key); }));
        }

        if (opts.Count == 0)
        {
            return;
        }

        //Clear option
        opts.Add(new FloatMenuOption("MFM_ClearDiscount".Translate(), delegate { action(""); }));

        var floatMenuMap = new FloatMenu(opts);
        Find.WindowStack.Add(floatMenuMap);
    }


    public static string generateDiscountCode(uint number)
    {
        var b = new StringBuilder();
        for (var i = 0; i < 5; ++i)
        {
            b.Append(ALPHABET[(int)number & ((1 << 5) - 1)]);
            number >>= 5;
        }

        return b.ToString();
    }

    /*
     * Increment the associated skill of a rented mercenary
     */
    public static void incMercSkillXP(Pawn merc, int nbHours)
    {
        var comp = merc.TryGetComp<Comp_USFM>();
        if (comp == null)
        {
            return;
        }

        var skill = getAssociatedSkill(merc, comp.type);
        var xp = Rand.Range(Settings.minXPRentedMercs / 24, Settings.maxXPRentedMercs / 24) * skill.LearnRateFactor() *
                 nbHours;
        skill.Learn(xp);
    }

    public static bool counterOfferInProgressMsg(Pawn pawn)
    {
        var ret = GCMFM.CounterOfferInProgress;
        if (ret)
        {
            Messages.Message("MFM_MsgCannotDropWeaponDuringCounterOffer".Translate(pawn.LabelCap),
                MessageTypeDefOf.NegativeEvent);
        }

        return ret;
    }

    public static bool modernUSFM()
    {
        if (Settings.currentEpoch != 0)
        {
            return Settings.currentEpoch != 1 || GCMFM.factionIronAlliance == null;
        }

        microElecResearchDef ??= DefDatabase<ResearchProjectDef>.GetNamed("MicroelectronicsBasics");

        return microElecResearchDef is { IsFinished: true };
    }

    public static string getUSFMLabel()
    {
        return modernUSFM() ? "MFM_USFMTitle".Translate() : "MFM_USFMMedievalTitle".Translate();
    }

    public static IntVec3 spawnMercOnMap(Map map, List<Pawn> toDeliver)
    {
        var dropCellNear = default(IntVec3);

        if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
        {
            Log.Warning("Failed to do traveler incident: Spawn prevented on Save Our Ship 2 space map.");
            return dropCellNear;
        }

        if (modernUSFM())
        {
            //if(!RCellFinder.TryFindRandomPawnEntryCell(out dropCellNear, map, CellFinder.EdgeRoadChance_Hostile, false, null))
            dropCellNear = DropCellFinder.FindRaidDropCenterDistant(map);
            if (!dropCellNear.IsValid)
            {
                dropCellNear = CellFinder.RandomCell(map);
            }

            DropPodUtility.DropThingsNear(dropCellNear, map, toDeliver, 100, false, false, false);
        }
        else
        {
            IntVec3 intVec;

            bool baseValidator(IntVec3 x)
            {
                return x.Standable(map) && !x.Fogged(map);
            }

            var hostFaction = map.ParentFaction;
            if (CellFinder.TryFindRandomEdgeCellWith(
                    x => baseValidator(x) &&
                         (hostFaction != null && map.reachability.CanReachFactionBase(x, hostFaction) ||
                          hostFaction == null && map.reachability.CanReachBiggestMapEdgeDistrict(x)), map,
                    CellFinder.EdgeRoadChance_Neutral, out var root))
            {
                intVec = CellFinder.RandomClosewalkCellNear(root, map, 5);
            }
            else if (CellFinder.TryFindRandomEdgeCellWith(baseValidator, map, CellFinder.EdgeRoadChance_Neutral,
                         out root))
            {
                intVec = CellFinder.RandomClosewalkCellNear(root, map, 5);
            }
            else if (CellFinder.TryFindRandomEdgeCellWith(baseValidator, map, CellFinder.EdgeRoadChance_Neutral,
                         out root))
            {
                intVec = CellFinder.RandomClosewalkCellNear(root, map, 5);
            }
            else
            {
                intVec = CellFinder.RandomCell(map);
            }

            foreach (var newThing in toDeliver)
            {
                dropCellNear = CellFinder.RandomClosewalkCellNear(intVec, map, 5);
                GenSpawn.Spawn(newThing, dropCellNear, map, Rot4.Random);
            }
        }

        return dropCellNear;
    }

    public static bool spawnMedievalCaravan(Map map, List<Thing> thingsToSpawn, out IntVec3 spawnedThings)
    {
        spawnedThings = default;

        if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
        {
            Log.Warning("Failed to do traveler incident: Spawn prevented on Save Our Ship 2 space map.");
            return false;
        }

        var faction = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("USFM_FactionAOS"));

        RCellFinder.TryFindRandomPawnEntryCell(out var spawnCenter, map, CellFinder.EdgeRoadChance_Neutral);

        if (!RCellFinder.TryFindTravelDestFrom(spawnCenter, map, out var travelDest))
        {
            Log.Warning(
                $"Failed to do traveler incident from {spawnCenter}: Couldn't find anywhere for the traveler to go.");
            return false;
        }

        var points = 0;
        //Calculation of points of brought elements
        foreach (var el in thingsToSpawn)
        {
            if (el.def.defName == "silver")
            {
                points += el.stackCount;
            }
            else
            {
                points += (int)el.def.BaseMarketValue * el.stackCount;
            }
        }

        points /= 50;
        if (points < 50)
        {
            points = 50;
        }
        //points = 1000;

        var list = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
        {
            groupKind = PawnGroupKindDefOf.Combat,
            tile = map.Tile,
            faction = faction,
            points = points
        }, false).ToList();

        //Activate mercenary mode to display icon
        foreach (var el in list)
        {
            var comp = el.TryGetComp<Comp_USFM>();
            if (comp == null)
            {
                continue;
            }

            comp.type = (MercenaryType)(-1);
            comp.isMercenary = true;
        }

        //Spawn of travelers
        foreach (var current in list)
        {
            var loc = CellFinder.RandomClosewalkCellNear(spawnCenter, map, 5);
            GenSpawn.Spawn(current, loc, map);
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

        var lordJob = new LordJob_TravelAndExit(travelDest);
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
            foreach (var wobj in Find.WorldObjects.AllWorldObjects)
            {
                if (wobj.def.defName == "MFM_PlaceOfPayment" && cTile == wobj.Tile)
                {
                    return caravan;
                }
            }
        }

        return null;
    }

    public static int moneyInCaravan(Caravan caravan)
    {
        var sum = 0;
        foreach (var el in caravan.AllThings)
        {
            if (el.def == ThingDefOf.Silver)
            {
                sum += el.stackCount;
            }
        }

        return sum;
    }

    public static bool ColonyHasEnoughLocalSilver(Map map, int fee)
    {
        return (from t in AllLocalSilverForTrade(map)
            where t.def == ThingDefOf.Silver
            select t).Sum(t => t.stackCount) >= fee;
    }


    public static IEnumerable<Thing> AllLocalSilverForTrade(Map map)
    {
        var yieldedThings = new HashSet<Thing>();
        foreach (var current in map.zoneManager.AllZones)
        {
            foreach (var c in current.Cells)
            {
                var thingList = c.GetThingList(map);
                foreach (var t in thingList)
                {
                    if (t.def.category != ThingCategory.Item || !PlayerSellableNow(t) || !yieldedThings.Add(t))
                    {
                        continue;
                    }

                    yield return t;
                }
            }
        }
    }

    private static bool PlayerSellableNow(Thing t)
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

        var apparel = t as Apparel;
        return apparel is not { WornByCorpse: true };
    }


    public static void payLocalSilver(int debt, Map map, ITrader trader)
    {
        while (debt > 0)
        {
            Thing thing = null;
            foreach (var current in map.zoneManager.AllZones)
            {
                foreach (var current2 in current.Cells)
                {
                    foreach (var current3 in map.thingGrid.ThingsAt(current2))
                    {
                        if (current3.def != ThingDefOf.Silver)
                        {
                            continue;
                        }

                        thing = current3;
                        goto IL_CC;
                    }
                }
            }

            IL_CC:
            if (thing == null)
            {
                Log.Error($"Could not find any {ThingDefOf.Silver} to transfer to trader.");
                break;
            }

            var num = Math.Min(debt, thing.stackCount);
            if (trader != null)
            {
                trader.GiveSoldThingToTrader(thing, num, TradeSession.playerNegotiator);
            }
            else
            {
                thing.SplitOff(num).Destroy();
            }

            debt -= num;
        }
    }

    public static void caravanPayCost(Caravan caravan, int cost)
    {
        //we withdraw the money from the caravan
        var debt = cost;
        while (debt > 0)
        {
            Thing thing = null;
            foreach (var el in caravan.AllThings)
            {
                if (el.def != ThingDefOf.Silver)
                {
                    continue;
                }

                thing = el;
                break;
            }

            if (thing == null)
            {
                continue;
            }

            var num = Math.Min(debt, thing.stackCount);
            thing.SplitOff(num).Destroy();
            debt -= num;
        }
    }

    /*
     * type is used to discriminate siteofpayments
     */
    public static void createSiteOfPayment(Map map, int type = -1)
    {
        var sop = WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed("MFM_PlaceOfPayment"));
        TileFinder.TryFindNewSiteTile(out var dstTile, map.Tile, 2, 5, true);
        sop.Tile = dstTile;
        if (type != -1)
        {
            sop.creationGameTicks = type;
        }

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

    public static void clearAllMedievalSiteOfPayment(int type = -1)
    {
        //Check if no current events requiring sop
        if (GCMFM.CounterOfferInProgress || GCMFM.MercWantJoinInProgress || GCMFM.BillInProgress)
        {
            return;
        }

        var toDel = new List<WorldObject>();
        foreach (var wobj in Find.WorldObjects.AllWorldObjects)
        {
            if (wobj.def.defName != "MFM_PlaceOfPayment")
            {
                continue;
            }

            if (type == -1 || type == wobj.creationGameTicks)
            {
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
        return pawn.IsSlave || pawn.health.hediffSet.HasHediff(SS_slaveHediff);
    }

    public static void removeKeysStartingWith(Dictionary<string, int> data, string key)
    {
        List<string> toDel = null;
        foreach (var el in data)
        {
            if (!el.Key.StartsWith(key))
            {
                continue;
            }

            toDel ??= [];

            toDel.Add(el.Key);
        }

        if (toDel == null)
        {
            return;
        }

        foreach (var e in toDel)
        {
            data.Remove(e);
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
            var td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt");
            if (td != null)
            {
                if (td.apparel.tags.Contains("IndustrialBasic"))
                {
                    td.apparel.tags.Remove("IndustrialBasic");
                }
            }

            td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirtCivil");
            if (td == null)
            {
                return;
            }

            if (td.apparel.tags.Contains("IndustrialBasic"))
            {
                td.apparel.tags.Remove("IndustrialBasic");
            }
        }
        else
        {
            var td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirt");
            if (td != null)
            {
                if (!td.apparel.tags.Contains("IndustrialBasic"))
                {
                    td.apparel.tags.Add("IndustrialBasic");
                }
            }

            td = DefDatabase<ThingDef>.GetNamed("MFM_Apparel_USFMBasicShirtCivil");
            if (td == null)
            {
                return;
            }

            if (!td.apparel.tags.Contains("IndustrialBasic"))
            {
                td.apparel.tags.Add("IndustrialBasic");
            }
        }
    }

    public static void setRentedMercSalary(Pawn merc)
    {
        var comp = merc.TryGetComp<Comp_USFM>();

        switch (getLevelFromSkill(merc, comp.type))
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

        var isSlave = isSS_Slave(merc);

        if (!isSlave)
        {
            return;
        }

        comp.slaveDecreaseIncome = Settings.percentIncomeDecreaseSlave;
        comp.salary -= (int)(comp.salary * Settings.percentIncomeDecreaseSlave);
    }

    public static void setHiredMercSalary(Pawn merc)
    {
        var comp = merc.TryGetComp<Comp_USFM>();

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
        if (caravan != null)
        {
            foreach (var p in caravan.pawns)
            {
                if (p.def.race is not { intelligence: Intelligence.Humanlike })
                {
                    continue;
                }

                var comp = p.TryGetComp<Comp_USFM>();
                if (comp is { isMercenary: true, hiredByPlayer: false })
                {
                    return true;
                }
            }
        }
        else
        {
            foreach (var p in map.mapPawns.FreeColonistsAndPrisonersSpawned)
            {
                var comp = p.TryGetComp<Comp_USFM>();
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
    private static bool apparelCanBeWearedBy(ThingDef apparelDef, Pawn pawn)
    {
        if (apparelDef == null || apparelDef.apparel == null || !apparelDef.IsApparel)
        {
            return false;
        }

        //Check if the clothes can be worn by the surrogate
        var path = apparelDef.apparel.LastLayer == ApparelLayerDefOf.Overhead
            ? apparelDef.apparel.wornGraphicPath
            : $"{apparelDef.apparel.wornGraphicPath}_{pawn.story.bodyType.defName}_south";

        Texture2D appFoundTex = null;
        //Check in texture mods
        for (var j = LoadedModManager.RunningModsListForReading.Count - 1; j >= 0; j--)
        {
            var appTex = LoadedModManager.RunningModsListForReading[j].GetContentHolder<Texture2D>().Get(path);
            if (appTex == null)
            {
                continue;
            }

            appFoundTex = appTex;
            break;
        }

        //Check RW texture mods
        if (appFoundTex != null)
        {
            return appFoundTex != null;
        }

        path = GenFilePaths.ContentPath<Texture2D>() + path;
        appFoundTex = Resources.Load<Texture2D>(path);

        return appFoundTex != null;
    }
}