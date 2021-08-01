using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using System.Text;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_Raid : IncidentWorker_RaidEnemy
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!Settings.enableIncidentRaid)
                return false;

            return base.CanFireNowSub(parms);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {

            if (!Settings.enableIncidentRaid)
                return false;

            Map map = (Map)parms.target;
            float initPoints = parms.points;

            //If in middle age mode we force the arrival by edge
            if (!Utils.modernUSFM())
            {
                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            }

            parms.raidNeverFleeIndividual = true;
            parms.points = parms.points * Rand.Range(1.0f, 1.6f);
            if(! (PawnGroupMakerUtility.TryGetRandomFactionForCombatPawnGroup(parms.points, out parms.faction, (Faction f) => this.FactionCanBeGroupSource(f, map, false), true, true, true, true) || PawnGroupMakerUtility.TryGetRandomFactionForCombatPawnGroup(parms.points, out parms.faction, (Faction f) => this.FactionCanBeGroupSource(f, map, true), true, true, true, true)))
            {
                return false;
            }

            this.ResolveRaidPoints(parms);
            /*if (!this.TryResolveRaidFaction(parms))
            {
                return false;
            }*/
            PawnGroupKindDef combat = PawnGroupKindDefOf.Combat;
            this.ResolveRaidStrategy(parms, combat);
            this.ResolveRaidArriveMode(parms);
            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                return false;
            }
            parms.points = IncidentWorker_Raid.AdjustedRaidPoints(parms.points, parms.raidArrivalMode, parms.raidStrategy, parms.faction, combat);
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms, false);
            List<Pawn> list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms, true).ToList<Pawn>();
            if (list.Count == 0)
            {
                //Log.Error("Got no pawns spawning raid from parms " + parms, false);
                return false;
            }

            //Uniform deduction
            int gear = 0;
            if (Rand.Chance(0.15f) && initPoints >= 1450)
                gear = 3;
            else if (Rand.Chance(0.35f) && initPoints >= 750)
                gear = 2;
            else
                gear = 1;

            //Weapons deduction
            int weapon = 0;
            if (Rand.Chance(0.08f) && initPoints >= 2800)
                weapon = 4;
            else if (Rand.Chance(0.15f) && initPoints >= 1000)
                weapon = 3;
            else if (Rand.Chance(0.35f))
                weapon = 2;
            else 
                weapon = 1;

            Colors selColor = (Colors)Rand.Range(1, 12);

            //Mercenary mode activation on group pawns
            foreach (var merc in list)
            {
                if (merc == null || (merc.def.race != null && !merc.RaceProps.Humanlike))
                    continue;

                Comp_USFM comp = merc.TryGetComp<Comp_USFM>();
                if (comp == null)
                    continue;

                comp.isMercenary = true;
                SkillRecord melee;
                SkillRecord shoot;

                //Reset child/adult hood
                Backstory bs = null;
                BackstoryDatabase.TryGetWithIdentifier("MercenaryRecruit18", out bs);
                merc.story.adulthood = bs;
                BackstoryDatabase.TryGetWithIdentifier("Mercenary55", out bs);
                merc.story.childhood = bs;

                Utils.ResetCachedIncapableOf(merc);

                //Log.Message("=>" + merc.def.defName);
                melee = Utils.getAssociatedSkill(merc, MercenaryType.Melee);
                shoot = Utils.getAssociatedSkill(merc, MercenaryType.Ranged);

                if (melee.levelInt > shoot.levelInt)
                    comp.type = MercenaryType.Melee;
                else
                    comp.type = MercenaryType.Ranged;

                if(merc.inventory != null)
                    merc.inventory.DestroyAll();
                if(merc.equipment != null)
                    merc.equipment.DestroyAllEquipment();
                if(merc.apparel != null)
                    merc.apparel.DestroyAll();

                try
                {
                    Utils.processMercGear(merc, comp.type, gear, (int)selColor);
                }
                catch (Exception)
                {

                }
                try
                {
                    Utils.processMercWeapon(merc, comp.type, weapon);
                }
                catch (Exception)
                {

                }
            }

            parms.raidArrivalMode.Worker.Arrive(list, parms);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Points = " + parms.points.ToString("F0"));
            foreach (Pawn current in list)
            {
                string str = (current.equipment == null || current.equipment.Primary == null) ? "unarmed" : current.equipment.Primary.LabelCap;
                stringBuilder.AppendLine(current.KindLabel + " - " + str);
            }
            TaggedString letterLabel = this.GetLetterLabel(parms);
            TaggedString letterText = this.GetLetterText(parms, list);
            PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(list, ref letterLabel, ref letterText, this.GetRelatedPawnsInfoLetterText(parms), true, true);
            List<TargetInfo> list2 = new List<TargetInfo>();
            if (parms.pawnGroups != null)
            {
                List<List<Pawn>> list3 = IncidentParmsUtility.SplitIntoGroups(list, parms.pawnGroups);
                List<Pawn> list4 = list3.MaxBy((List<Pawn> x) => x.Count);
                if (list4.Any<Pawn>())
                {
                    list2.Add(list4[0]);
                }
                for (int i = 0; i < list3.Count; i++)
                {
                    if (list3[i] != list4)
                    {
                        if (list3[i].Any<Pawn>())
                        {
                            list2.Add(list3[i][0]);
                        }
                    }
                }
            }
            else if (list.Any<Pawn>())
            {
                list2.Add(list[0]);
            }
            Find.LetterStack.ReceiveLetter("MFM_LetterMercRaid".Translate(), "MFM_LetterMercRaidDesc".Translate(parms.faction.Name, Utils.getUSFMLabel(), "\n" +letterText), this.GetLetterDef(), list2, parms.faction);
            parms.raidStrategy.Worker.MakeLords(parms, list);
            return true;
        }
    }
}