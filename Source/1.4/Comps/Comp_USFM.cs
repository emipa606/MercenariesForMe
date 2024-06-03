using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Verse.AI.Group;
using System.Linq;
using HarmonyLib;
using System.Reflection;

namespace aRandomKiwi.MFM
{
    public class Comp_USFM : ThingComp
    {

        public override void PostDraw()
        {
            Material avatar=null;

            //If mercenary icon display allowed
            if (!Settings.hideMercenariesIcon)
            {
                Vector3 vector;

                if (isMercenary)
                {
                    avatar = Utils.getMercenaryIcon(type);
                }

                if (avatar != null)
                {
                    vector = this.parent.TrueCenter();
                    vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
                    vector.z += 1.4f;
                    vector.x += this.parent.def.size.x / 2;

                    Graphics.DrawMesh(MeshPool.plane08, vector, Quaternion.identity, avatar, 0);
                }

                if (isMercenary && !Settings.hideMercenariesLevel)
                {
                    switch (Level)
                    {
                        case MercenaryLevel.Confirmed:
                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);
                            break;
                        case MercenaryLevel.Veteran:
                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2 - 0.15f;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2 + 0.15f;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                            break;
                        case MercenaryLevel.Elite:
                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2 - 0.3f;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2 + 0.3f;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.level1, 0);

                            break;
                        case MercenaryLevel.Cyborg:
                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2 - 0.3f;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.levelCyborg, 0);

                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.levelCyborg, 0);

                            vector = this.parent.TrueCenter();
                            vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28127f;
                            vector.z += 1.025f;
                            vector.x += this.parent.def.size.x / 2 + 0.3f;
                            Graphics.DrawMesh(MeshPool.plane03, vector, Quaternion.identity, Tex.levelCyborg, 0);

                            break;
                    }
                }

            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<float>(ref this.slaveDecreaseIncome, "MFM_slaveDecreaseIncome", 0.0f);

            Scribe_Values.Look<int>(ref this.xpEarnedLastCT, "MFM_xpEarnedLastCT", -1);
            Scribe_Values.Look<bool>(ref this.spawned, "spawned", false);
            Scribe_Values.Look<bool>(ref this.hiredByPlayer, "MFM_hiredByPlayer", false);
            Scribe_Values.Look<MercenaryType>(ref this.type, "MFM_type", 0);
            Scribe_Values.Look<int>(ref this.salary, "MFM_salary", 0);
            Scribe_Values.Look<int>(ref this.guarantee, "MFM_guarantee", 0);
            Scribe_Values.Look<int>(ref this.nbQuad, "MFM_nbQuad", 0);
            Scribe_Values.Look<int>(ref this.nbQuadHappy, "MFM_nbQuadHappy", 0);
            Scribe_Values.Look<int>(ref this.nbQuadUnHappy, "MFM_nbQuadUnHappy", 0);

            Scribe_Values.Look<bool>(ref this.killedDuringSrv, "MFM_killedDuringSrv", false);
            Scribe_Values.Look<bool>(ref this.isMercenary, "MFM_isMercenary", false);
            Scribe_Values.Look<int>(ref this.quadNbHourLastCGT, "MFM_quadNbHourLastCGT", 0);
            Scribe_Values.Look<int>(ref this.quadNbHour, "MFM_quadNbHour", 0);
            Scribe_Values.Look<int>(ref this.quadNbHourMoodOK, "MFM_quadNbHourMoodOK", 0);
            Scribe_Values.Look<int>(ref this.quadNbHourMoodBad, "MFM_quadNbHourMoodBad", 0);
            Scribe_Collections.Look(ref origSkills, "MFM_origSkills", LookMode.Value);
            Scribe_Values.Look<float>(ref this.origScore, "MFM_origScore", 0.0f);
            Scribe_Values.Look<int>(ref this.startingRentGT, "MFM_startingRentGT", -1);
            



            if (origSkills == null)
            {
                origSkills = new Dictionary<string, int>();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            Pawn pawn = (Pawn)parent;
            if(pawn != null && pawn.Faction != null && pawn.RaceProps.Humanlike && pawn.Faction.def.defName == "USFM_FactionAOS")
            {
                if(pawn.trader != null)
                    type = (MercenaryType)(-2);
                else
                    type = (MercenaryType)(-1);
                isMercenary = true;
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
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

            int CGT = Find.TickManager.TicksGame;

            if(CGT % 180 == 0)
            {
                if(isMercenary && (this.parent.Faction == Faction.OfPlayer && (int)type == -1 || (int)type == -2))
                {
                    isMercenary = false;
                }
            }

            //Every hour
            if (isMercenary && CGT % 2500 == 0)
            {
                int increment = 1;
                if (quadNbHourLastCGT != 0)
                    increment = (CGT - quadNbHourLastCGT) / 2500;

                //Inc nb hours worked during the quadrum
                quadNbHour += increment;

                //Inc nb hour or correct mood if applicable
                Pawn pawn = (Pawn)parent;
                if(pawn.needs.mood.CurLevel >= 0.65f)
                    quadNbHourMoodOK += increment;
                if (pawn.needs.mood.CurLevel <= 0.35f)
                    quadNbHourMoodBad += increment;
            }
        }


        public override string CompInspectStringExtra()
        {
            string ret = "";

            //If map not defined or pawn host has not learned to hunt, we quit
            if (parent.Map == null)
                return base.CompInspectStringExtra();

            Pawn pawn = (Pawn)parent;

            if (isMercenary)
            {
                bool isOwnedByPlayer = pawn.Faction == Faction.OfPlayer;

                string specialJob = "";

                //Added mercenary type
                ret += ("MFM_Job".Translate(Utils.getReadableType(type)+" "+specialJob)) + "\n";
                //Addition of salary
                if (isOwnedByPlayer)
                    ret += ("MFM_Salary".Translate(salary)) + "\n";
                ret += ("MFM_Level".Translate(Utils.getReadableLevel(Level))) + "\n";

                if (isOwnedByPlayer)
                {
                    //Affinity deduction
                    string affinity = "";
                    if (quadNbHourMoodBad >= Settings.badMoodNbhPerQuadFloor)
                        affinity = "MFM_AffinityBad".Translate();
                    else if (quadNbHourMoodOK >= Settings.goodMoodNbhPerQuadFloor)
                        affinity = "MFM_AffinityOK".Translate();
                    else
                        affinity = "MFM_AffinityNeutral".Translate();

                    ret += ("MFM_Affinity".Translate(affinity)) + "\n";

                    float newScore = Utils.getPawnScore((Pawn)parent);
                    float percent = (newScore / origScore);
                    float newGuarantee;
                    if (percent > 1.0f)
                        percent = 1.0f;
                    if (origScore > newScore)
                        newGuarantee = guarantee - (int)(guarantee * ((origScore - newScore) / origScore));
                    else
                        newGuarantee = guarantee;

                    ret += ("MFM_GuaranteeCS".Translate((int)(percent * 100), newGuarantee, guarantee)) + "\n";

                    //Log.Message("NbhOK = "+quadNbHourMoodOK+" ( "+ ((Pawn)parent).needs.mood.CurLevel + " ) NbhBad = "+quadNbHourMoodBad);
                }
            }

            int CGT = Find.TickManager.TicksGame;

            if (firedGT > CGT)
            {
                ret += ("MFM_FiredCountDown".Translate(Utils.getUSFMLabel(),(firedGT-CGT).ToStringTicksToPeriodVerbose()));
            }

            return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
        }




        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //vehicle framework patch
            if (this.parent.def.race != null && this.parent.def.race.intelligence != Intelligence.Humanlike)
            {
                yield break;
            }

            Pawn pawn = (Pawn)parent;
            SkillRecord skillMeleeGen = Utils.getAssociatedSkill(pawn, MercenaryType.Melee);
            SkillRecord skillShootGen = Utils.getAssociatedSkill(pawn, MercenaryType.Ranged);
            bool isNonViolent = (skillMeleeGen.TotallyDisabled && skillShootGen.TotallyDisabled);

            if (isMercenary && pawn.Faction == Faction.OfPlayer)
            {
                //Fire a mercenary
                yield return new Command_Action
                {
                    icon = Tex.fired,
                    defaultLabel = "MFM_Fired".Translate(),
                    defaultDesc = "MFM_FiredDesc".Translate(Utils.getUSFMLabel()),
                    action = delegate ()
                    {
                        Find.WindowStack.Add(new Dialog_Msg("MFM_DialogConfirmFired".Translate(), "MFM_DialogConfirmFiredDesc".Translate(Utils.getUSFMLabel()), delegate
                        {
                            guarantee = 0;
                            parent.SetFaction(Faction.OfAncients, null);
                            Lord lord = null;
                            LordJob_ExitMapBest lordJob = new LordJob_ExitMapBest(LocomotionUrgency.Sprint, true);
                            if (lordJob != null)
                                lord = LordMaker.MakeNewLord(Faction.OfAncients, lordJob, Current.Game.CurrentMap, null);

                            if (lord != null)
                                lord.AddPawn(pawn);
                        },false));
                    }
                };
            }

            yield break;
        }

        public MercenaryLevel Level
        {
            get{
                Pawn pawn = (Pawn)parent;
                return Utils.getLevelFromSkill(pawn, type);
            }
        }

        /*
         * Saving of the parent's current skills as well as his vital score (which will be used for the return of the deposit)
         */
        public void saveOrigSkills()
        {
            Pawn pawn = (Pawn)parent;
            foreach(var s in pawn.skills.skills)
            {
                origSkills[s.def.defName] = s.levelInt;
            }

            origScore = Utils.getPawnScore(pawn);
        }

        public void restoreOrigSkills()
        {
            Pawn pawn = (Pawn)parent;
            if (pawn.skills == null || pawn.skills.skills == null)
                return;

            foreach (var s in pawn.skills.skills)
            {
                if(origSkills.ContainsKey(s.def.defName))
                    s.levelInt = origSkills[s.def.defName];
            }
        }

        /*
         * Init skills according to the specified params
         */
        public void initSkills(MercenaryType type, MercenaryLevel level)
        {
            Pawn pawn = (Pawn)parent;

            //Skill definition associated with the mercenary's jobs
            SkillRecord skill = Utils.getAssociatedSkill(pawn, type);
            if (skill == null)
                return;

            switch (level)
            {
                case MercenaryLevel.Recruit:
                    skill.levelInt = Rand.Range(4, 7);
                    if (Rand.Chance(0.15f))
                        skill.passion = Passion.Major;
                    else if (Rand.Chance(0.3f))
                        skill.passion = Passion.Minor;
                    break;
                case MercenaryLevel.Confirmed:
                    skill.levelInt = Rand.Range(8, 11);
                    if (Rand.Chance(0.35f))
                        skill.passion = Passion.Major;
                    else if (Rand.Chance(0.4f))
                        skill.passion = Passion.Minor;
                    break;
                case MercenaryLevel.Veteran:
                    skill.levelInt = Rand.Range(12, 16);
                    if (Rand.Chance(0.35f))
                        skill.passion = Passion.Major;
                    else if (Rand.Chance(0.5f))
                        skill.passion = Passion.Minor;
                    break;
                case MercenaryLevel.Elite:
                    skill.levelInt = Rand.Range(17, 19);
                    if (Rand.Chance(0.45f))
                        skill.passion = Passion.Major;
                    else if(Rand.Chance(0.6f))
                        skill.passion = Passion.Minor;
                    break;
                case MercenaryLevel.Cyborg:
                    skill.levelInt = 20;
                    if (Rand.Chance(0.75f))
                        skill.passion = Passion.Major;
                    else if (Rand.Chance(0.9f))
                        skill.passion = Passion.Minor;
                    break;
            }

            //Reset all skills to unable to
            foreach (var s in pawn.skills.skills)
            {
                if (s == skill)
                    continue;

                if (!Settings.allowNonZeroOtherSkillsMerc)
                {
                    s.levelInt = 0;
                    s.xpSinceLastLevel = 0;
                    s.xpSinceMidnight = 0;
                    s.passion = Passion.None;
                }
                else
                {
                    if(s.levelInt >= skill.levelInt)
                    {
                        s.levelInt = skill.levelInt - 2;
                        s.xpSinceLastLevel = 0;
                        s.xpSinceMidnight = 0;
                    }
                }
            }

        }
        

        private void initWorkAssignment()
        {
            Pawn p = (Pawn)parent;
            if(p.workSettings == null)
                p.workSettings = new Pawn_WorkSettings(p);

            //Clear all
            foreach (WorkTypeDef current in from w in DefDatabase<WorkTypeDef>.AllDefs
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
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Warden":
                        if (type == MercenaryType.Speaker && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Handling":
                        if (type == MercenaryType.Trainer && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Cooking":
                        if (type == MercenaryType.Cooker && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Hunting":
                        if (type == MercenaryType.Ranged && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Construction":
                        if (type == MercenaryType.Builder && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Growing":
                        if (type == MercenaryType.Farmer && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Mining":
                        if (type == MercenaryType.Miner && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "PlantCutting":
                        if (type == MercenaryType.Farmer && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Smithing":
                        if (type == MercenaryType.Tech && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Tailoring":
                        if (type == MercenaryType.Tech && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Art":
                        if (type == MercenaryType.Artist && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Crafting":
                        if (type == MercenaryType.Tech && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                    case "Research":
                        if (type == MercenaryType.Scientist && !p.WorkTypeIsDisabled(current))
                            p.workSettings.SetPriority(current, 1);
                        break;
                }

                //Then set to 1 basic priorities (rest/firefight/patient)
                if (Settings.setBasicWorkTypeToOne)
                {
                    if (!p.WorkTypeIsDisabled(CWorkTypeDefOf.Firefighter))
                        p.workSettings.SetPriority(CWorkTypeDefOf.Firefighter, 1);
                    if (!p.WorkTypeIsDisabled(CWorkTypeDefOf.Patient))
                        p.workSettings.SetPriority(CWorkTypeDefOf.Patient, 1);
                    if (!p.WorkTypeIsDisabled(CWorkTypeDefOf.PatientBedRest))
                        p.workSettings.SetPriority(CWorkTypeDefOf.PatientBedRest, 1);
                }
            }
        }

        public void rentedMercAdvanceBioAge()
        {
            Pawn cp = (Pawn)parent;
            if (startingRentGT != -1 && cp.ageTracker != null)
            {
                cp.ageTracker.AgeBiologicalTicks += (Find.TickManager.TicksGame - startingRentGT);
                startingRentGT = -1;
            }
        }

        /*
         * Operation precedent a hired mercenary return
         */
        public void prepareBringBackRentedMerc()
        {
            
        }

        public int startingRentGT = -1;
        public float slaveDecreaseIncome = 0.0f;
        public bool spawned = false;
        public bool hiredByPlayer = false;
        public bool killedDuringSrv = false;
        public bool isMercenary = false;
        public MercenaryType type;
        public int salary=0;
        public int guarantee = 0;
        public float origScore = 0.0f;
        public int firedGT = 0;
        public int nbQuad = 0;
        public int nbQuadUnHappy = 0;
        public int nbQuadHappy = 0;

        public int xpEarnedLastCT = -1;

        //Number of hours during the current quadrum where the mercenary was happy
        public int quadNbHourMoodOK = 0;
        //Number of hours during the current quadrum where the mercenary was unhappy
        public int quadNbHourMoodBad = 0;
        //Number of hours constituting the quadrum
        public int quadNbHour = 0;
        //Allows to compare the current CGT with the previous one to know how many hours have elapsed to increment quadNbHour
        public int quadNbHourLastCGT = 0;
        public Dictionary<string, int> origSkills = new Dictionary<string, int>();
    }
}