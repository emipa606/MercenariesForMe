using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace aRandomKiwi.MFM;

public class Comp_USFM : ThingComp
{
    public int firedGT = 0;
    public int guarantee;
    public bool hiredByPlayer;
    public bool isMercenary;
    public bool killedDuringSrv;
    public int nbQuad;
    public int nbQuadHappy;
    public int nbQuadUnHappy;
    public float origScore;

    private Dictionary<string, int> origSkills = new();

    //Number of hours constituting the quadrum
    public int quadNbHour;

    //Allows to compare the current CGT with the previous one to know how many hours have elapsed to increment quadNbHour
    private int quadNbHourLastCGT;

    //Number of hours during the current quadrum where the mercenary was unhappy
    public int quadNbHourMoodBad;

    //Number of hours during the current quadrum where the mercenary was happy
    public int quadNbHourMoodOK;
    public int salary;
    public float slaveDecreaseIncome;
    private bool spawned;

    public int startingRentGT = -1;
    public MercenaryType type;

    public int xpEarnedLastCT = -1;

    public MercenaryLevel Level
    {
        get
        {
            var pawn = (Pawn)parent;
            return Utils.getLevelFromSkill(pawn, type);
        }
    }

    public override void PostDraw()
    {
        Material avatar = null;

        //If mercenary icon display allowed
        if (Settings.hideMercenariesIcon)
        {
            return;
        }

        Vector3 vector;

        if (isMercenary)
        {
            avatar = Utils.getMercenaryIcon(type);
        }

        if (avatar != null)
        {
            vector = parent.TrueCenter();
            vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28125f;
            vector.z += 1.4f;
            vector.x += parent.def.size.x / 2f;

            Graphics.DrawMesh(MeshPool.plane08, vector, Quaternion.identity, avatar, 0);
        }

        if (!isMercenary || Settings.hideMercenariesLevel)
        {
            return;
        }

        switch (Level)
        {
            case MercenaryLevel.Confirmed:
                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += parent.def.size.x / 2f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);
                break;
            case MercenaryLevel.Veteran:
                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += (parent.def.size.x / 2f) - 0.15f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += (parent.def.size.x / 2f) + 0.15f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                break;
            case MercenaryLevel.Elite:
                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += (parent.def.size.x / 2f) - 0.3f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += parent.def.size.x / 2f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += (parent.def.size.x / 2f) + 0.3f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                break;
            case MercenaryLevel.Cyborg:
                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += (parent.def.size.x / 2f) - 0.3f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.levelCyborg, 0);

                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += parent.def.size.x / 2f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.levelCyborg, 0);

                vector = parent.TrueCenter();
                vector.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28127f;
                vector.z += 1.025f;
                vector.x += (parent.def.size.x / 2f) + 0.3f;
                Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.levelCyborg, 0);

                break;
        }
    }

    public override void PostExposeData()
    {
        base.PostExposeData();

        Scribe_Values.Look(ref slaveDecreaseIncome, "MFM_slaveDecreaseIncome");

        Scribe_Values.Look(ref xpEarnedLastCT, "MFM_xpEarnedLastCT", -1);
        Scribe_Values.Look(ref spawned, "spawned");
        Scribe_Values.Look(ref hiredByPlayer, "MFM_hiredByPlayer");
        Scribe_Values.Look(ref type, "MFM_type");
        Scribe_Values.Look(ref salary, "MFM_salary");
        Scribe_Values.Look(ref guarantee, "MFM_guarantee");
        Scribe_Values.Look(ref nbQuad, "MFM_nbQuad");
        Scribe_Values.Look(ref nbQuadHappy, "MFM_nbQuadHappy");
        Scribe_Values.Look(ref nbQuadUnHappy, "MFM_nbQuadUnHappy");

        Scribe_Values.Look(ref killedDuringSrv, "MFM_killedDuringSrv");
        Scribe_Values.Look(ref isMercenary, "MFM_isMercenary");
        Scribe_Values.Look(ref quadNbHourLastCGT, "MFM_quadNbHourLastCGT");
        Scribe_Values.Look(ref quadNbHour, "MFM_quadNbHour");
        Scribe_Values.Look(ref quadNbHourMoodOK, "MFM_quadNbHourMoodOK");
        Scribe_Values.Look(ref quadNbHourMoodBad, "MFM_quadNbHourMoodBad");
        Scribe_Collections.Look(ref origSkills, "MFM_origSkills", LookMode.Value);
        Scribe_Values.Look(ref origScore, "MFM_origScore");
        Scribe_Values.Look(ref startingRentGT, "MFM_startingRentGT", -1);


        origSkills ??= new Dictionary<string, int>();
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);

        var pawn = (Pawn)parent;
        if (pawn == null || pawn.Faction == null || !pawn.RaceProps.Humanlike ||
            pawn.Faction.def.defName != "USFM_FactionAOS")
        {
            return;
        }

        if (pawn.trader != null)
        {
            type = (MercenaryType)(-2);
        }
        else
        {
            type = (MercenaryType)(-1);
        }

        isMercenary = true;
    }

    public override void CompTick()
    {
        base.CompTick();

        if (isMercenary && parent.Faction == Faction.OfPlayer && !spawned)
        {
            spawned = true;
            initWorkAssignment();
        }

        if (killedDuringSrv)
        {
            killedDuringSrv = false;
            HealthUtility.DamageUntilDead((Pawn)parent);
            return;
        }

        var CGT = Find.TickManager.TicksGame;

        if (CGT % 180 == 0)
        {
            if (isMercenary && (parent.Faction == Faction.OfPlayer && (int)type == -1 || (int)type == -2))
            {
                isMercenary = false;
            }
        }

        //Every hour
        if (!isMercenary || CGT % 2500 != 0)
        {
            return;
        }

        var increment = 1;
        if (quadNbHourLastCGT != 0)
        {
            increment = (CGT - quadNbHourLastCGT) / 2500;
        }

        //Inc nb hours worked during the quadrum
        quadNbHour += increment;

        //Inc nb hour or correct mood if applicable
        var pawn = (Pawn)parent;
        if (pawn.needs.mood.CurLevel >= 0.65f)
        {
            quadNbHourMoodOK += increment;
        }

        if (pawn.needs.mood.CurLevel <= 0.35f)
        {
            quadNbHourMoodBad += increment;
        }
    }


    public override string CompInspectStringExtra()
    {
        var ret = "";

        //If map not defined or pawn host has not learned to hunt, we quit
        if (parent.Map == null)
        {
            return base.CompInspectStringExtra();
        }

        var pawn = (Pawn)parent;

        if (isMercenary)
        {
            var isOwnedByPlayer = pawn.Faction == Faction.OfPlayer;

            var specialJob = "";

            //Added mercenary type
            ret += "MFM_Job".Translate($"{Utils.getReadableType(type)} {specialJob}") + "\n";
            //Addition of salary
            if (isOwnedByPlayer)
            {
                ret += "MFM_Salary".Translate(salary) + "\n";
            }

            ret += "MFM_Level".Translate(Utils.getReadableLevel(Level)) + "\n";

            if (isOwnedByPlayer)
            {
                //Affinity deduction
                string affinity;
                if (quadNbHourMoodBad >= Settings.badMoodNbhPerQuadFloor)
                {
                    affinity = "MFM_AffinityBad".Translate();
                }
                else if (quadNbHourMoodOK >= Settings.goodMoodNbhPerQuadFloor)
                {
                    affinity = "MFM_AffinityOK".Translate();
                }
                else
                {
                    affinity = "MFM_AffinityNeutral".Translate();
                }

                ret += "MFM_Affinity".Translate(affinity) + "\n";

                var newScore = Utils.getPawnScore((Pawn)parent);
                var percent = newScore / origScore;
                float newGuarantee;
                if (percent > 1.0f)
                {
                    percent = 1.0f;
                }

                if (origScore > newScore)
                {
                    newGuarantee = guarantee - (int)(guarantee * ((origScore - newScore) / origScore));
                }
                else
                {
                    newGuarantee = guarantee;
                }

                ret += "MFM_GuaranteeCS".Translate((int)(percent * 100), newGuarantee, guarantee) + "\n";

                //Log.Message("NbhOK = "+quadNbHourMoodOK+" ( "+ ((Pawn)parent).needs.mood.CurLevel + " ) NbhBad = "+quadNbHourMoodBad);
            }
        }

        var CGT = Find.TickManager.TicksGame;

        if (firedGT > CGT)
        {
            ret += "MFM_FiredCountDown".Translate(Utils.getUSFMLabel(), (firedGT - CGT).ToStringTicksToPeriodVerbose());
        }

        return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
    }


    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        //vehicle framework patch
        if (parent.def.race != null && parent.def.race.intelligence != Intelligence.Humanlike)
        {
            yield break;
        }

        var pawn = (Pawn)parent;

        if (isMercenary && pawn.Faction == Faction.OfPlayer)
        {
            //Fire a mercenary
            yield return new Command_Action
            {
                icon = Tex.fired,
                defaultLabel = "MFM_Fired".Translate(),
                defaultDesc = "MFM_FiredDesc".Translate(Utils.getUSFMLabel()),
                action = delegate
                {
                    Find.WindowStack.Add(new Dialog_Msg("MFM_DialogConfirmFired".Translate(),
                        "MFM_DialogConfirmFiredDesc".Translate(Utils.getUSFMLabel()), delegate
                        {
                            guarantee = 0;
                            parent.SetFaction(Faction.OfAncients);
                            var lordJob = new LordJob_ExitMapBest(LocomotionUrgency.Sprint, true);
                            var lord = LordMaker.MakeNewLord(Faction.OfAncients, lordJob, Current.Game.CurrentMap);

                            lord?.AddPawn(pawn);
                        }));
                }
            };
        }
    }

    /*
     * Saving of the parent's current skills as well as his vital score (which will be used for the return of the deposit)
     */
    public void saveOrigSkills()
    {
        var pawn = (Pawn)parent;
        foreach (var s in pawn.skills.skills)
        {
            origSkills[s.def.defName] = s.levelInt;
        }

        origScore = Utils.getPawnScore(pawn);
    }

    public void restoreOrigSkills()
    {
        var pawn = (Pawn)parent;
        if (pawn.skills == null || pawn.skills.skills == null)
        {
            return;
        }

        foreach (var s in pawn.skills.skills)
        {
            if (origSkills.TryGetValue(s.def.defName, out var skill))
            {
                s.levelInt = skill;
            }
        }
    }

    /*
     * Init skills according to the specified params
     */
    public void initSkills(MercenaryType mercType, MercenaryLevel level)
    {
        var pawn = (Pawn)parent;

        //Skill definition associated with the mercenary's jobs
        var skill = Utils.getAssociatedSkill(pawn, mercType);
        if (skill == null)
        {
            return;
        }

        switch (level)
        {
            case MercenaryLevel.Recruit:
                skill.levelInt = Rand.Range(4, 7);
                if (Rand.Chance(0.15f))
                {
                    skill.passion = Passion.Major;
                }
                else if (Rand.Chance(0.3f))
                {
                    skill.passion = Passion.Minor;
                }

                break;
            case MercenaryLevel.Confirmed:
                skill.levelInt = Rand.Range(8, 11);
                if (Rand.Chance(0.35f))
                {
                    skill.passion = Passion.Major;
                }
                else if (Rand.Chance(0.4f))
                {
                    skill.passion = Passion.Minor;
                }

                break;
            case MercenaryLevel.Veteran:
                skill.levelInt = Rand.Range(12, 16);
                if (Rand.Chance(0.35f))
                {
                    skill.passion = Passion.Major;
                }
                else if (Rand.Chance(0.5f))
                {
                    skill.passion = Passion.Minor;
                }

                break;
            case MercenaryLevel.Elite:
                skill.levelInt = Rand.Range(17, 19);
                if (Rand.Chance(0.45f))
                {
                    skill.passion = Passion.Major;
                }
                else if (Rand.Chance(0.6f))
                {
                    skill.passion = Passion.Minor;
                }

                break;
            case MercenaryLevel.Cyborg:
                skill.levelInt = 20;
                if (Rand.Chance(0.75f))
                {
                    skill.passion = Passion.Major;
                }
                else if (Rand.Chance(0.9f))
                {
                    skill.passion = Passion.Minor;
                }

                break;
        }

        //Reset all skills too unable to
        foreach (var s in pawn.skills.skills)
        {
            if (s == skill)
            {
                continue;
            }

            if (!Settings.allowNonZeroOtherSkillsMerc)
            {
                s.levelInt = 0;
                s.xpSinceLastLevel = 0;
                s.xpSinceMidnight = 0;
                s.passion = Passion.None;
            }
            else
            {
                if (s.levelInt < skill.levelInt)
                {
                    continue;
                }

                s.levelInt = skill.levelInt - 2;
                s.xpSinceLastLevel = 0;
                s.xpSinceMidnight = 0;
            }
        }
    }


    private void initWorkAssignment()
    {
        var p = (Pawn)parent;
        p.workSettings ??= new Pawn_WorkSettings(p);

        //Clear all
        foreach (var current in from w in DefDatabase<WorkTypeDef>.AllDefs
                 where !p.WorkTypeIsDisabled(w)
                 select w)
        {
            //If not clear base stains
            if (!Utils.workTypeDefsToNotClear.Contains(current.defName))
            {
                p.workSettings.Disable(current);
            }
            else
            {
                p.workSettings.SetPriority(current, 3);
            }

            //Activation at 1 only of their job
            switch (current.defName)
            {
                case "Doctor":
                    if (type == MercenaryType.Medical && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Warden":
                    if (type == MercenaryType.Speaker && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Handling":
                    if (type == MercenaryType.Trainer && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Cooking":
                    if (type == MercenaryType.Cooker && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Hunting":
                    if (type == MercenaryType.Ranged && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Construction":
                    if (type == MercenaryType.Builder && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Growing":
                    if (type == MercenaryType.Farmer && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Mining":
                    if (type == MercenaryType.Miner && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "PlantCutting":
                    if (type == MercenaryType.Farmer && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Smithing":
                case "Tailoring":
                    if (type == MercenaryType.Tech && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Art":
                    if (type == MercenaryType.Artist && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Crafting":
                    if (type == MercenaryType.Tech && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
                case "Research":
                    if (type == MercenaryType.Scientist && !p.WorkTypeIsDisabled(current))
                    {
                        p.workSettings.SetPriority(current, 1);
                    }

                    break;
            }

            //Then set to 1 basic priorities (rest/firefight/patient)
            if (!Settings.setBasicWorkTypeToOne)
            {
                continue;
            }

            if (!p.WorkTypeIsDisabled(CWorkTypeDefOf.Firefighter))
            {
                p.workSettings.SetPriority(CWorkTypeDefOf.Firefighter, 1);
            }

            if (!p.WorkTypeIsDisabled(CWorkTypeDefOf.Patient))
            {
                p.workSettings.SetPriority(CWorkTypeDefOf.Patient, 1);
            }

            if (!p.WorkTypeIsDisabled(CWorkTypeDefOf.PatientBedRest))
            {
                p.workSettings.SetPriority(CWorkTypeDefOf.PatientBedRest, 1);
            }
        }
    }

    public void rentedMercAdvanceBioAge()
    {
        var cp = (Pawn)parent;
        if (startingRentGT == -1 || cp.ageTracker == null)
        {
            return;
        }

        cp.ageTracker.AgeBiologicalTicks += Find.TickManager.TicksGame - startingRentGT;
        startingRentGT = -1;
    }

    /*
     * Operation precedent a hired mercenary return
     */
    public static void prepareBringBackRentedMerc()
    {
    }
}