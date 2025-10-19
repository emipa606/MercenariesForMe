using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class IncidentWorker_Raid : IncidentWorker_RaidEnemy
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return Settings.enableIncidentRaid && base.CanFireNowSub(parms);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!Settings.enableIncidentRaid)
        {
            return false;
        }

        var map = (Map)parms.target;
        if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
        {
            Log.Warning("[MFM] Raid incident prevented on SOS2 or Rimnauts 2 map.");
            return false;
        }

        var initPoints = parms.points;

        //If in middle age mode we force the arrival by edge
        if (!Utils.modernUSFM())
        {
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
        }

        parms.raidNeverFleeIndividual = true;
        parms.points *= Rand.Range(1.0f, 1.6f);
        if (!(PawnGroupMakerUtility.TryGetRandomFactionForCombatPawnGroup(parms.points, out parms.faction,
                  f => FactionCanBeGroupSource(f, parms), true, true, true) ||
              PawnGroupMakerUtility.TryGetRandomFactionForCombatPawnGroup(parms.points, out parms.faction,
                  f => FactionCanBeGroupSource(f, parms, true), true, true, true)))
        {
            return false;
        }

        ResolveRaidPoints(parms);
        /*if (!this.TryResolveRaidFaction(parms))
        {
            return false;
        }*/
        var combat = PawnGroupKindDefOf.Combat;

        ResolveRaidStrategy(parms, combat);
        ResolveRaidArriveMode(parms);

        //Prevent siege attack
        if (parms.raidStrategy is { defName: "Siege" })
        {
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        }

        if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
        {
            return false;
        }

        parms.points = AdjustedRaidPoints(parms.points, parms.raidArrivalMode, parms.raidStrategy, parms.faction,
            combat, map);
        var defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(combat, parms);
        var list = PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
        if (list.Count == 0)
        {
            //Log.Error("Got no pawns spawning raid from parms " + parms, false);
            return false;
        }

        //Uniform deduction
        int gear;
        if (Rand.Chance(0.15f) && initPoints >= 1450)
        {
            gear = 3;
        }
        else if (Rand.Chance(0.35f) && initPoints >= 750)
        {
            gear = 2;
        }
        else
        {
            gear = 1;
        }

        //Weapons deduction
        int weapon;
        if (Rand.Chance(0.08f) && initPoints >= 2800)
        {
            weapon = 4;
        }
        else if (Rand.Chance(0.15f) && initPoints >= 1000)
        {
            weapon = 3;
        }
        else if (Rand.Chance(0.35f))
        {
            weapon = 2;
        }
        else
        {
            weapon = 1;
        }

        var selColor = (Colors)Rand.Range(1, 12);

        //Mercenary mode activation on group pawns
        foreach (var merc in list)
        {
            if (merc == null || merc.def.race != null && !merc.RaceProps.Humanlike)
            {
                continue;
            }

            var comp = merc.TryGetComp<Comp_USFM>();
            if (comp == null)
            {
                continue;
            }

            comp.isMercenary = true;

            //Reset child/adulthood
            merc.story.Adulthood = DefDatabase<BackstoryDef>.GetNamed("Mercenary4");
            merc.story.Childhood = DefDatabase<BackstoryDef>.GetNamed("MercenaryRecruit36");

            Utils.ResetCachedIncapableOf(merc);

            //Log.Message("=>" + merc.def.defName);
            var melee = Utils.getAssociatedSkill(merc, MercenaryType.Melee);
            var shoot = Utils.getAssociatedSkill(merc, MercenaryType.Ranged);

            comp.type = melee.levelInt > shoot.levelInt ? MercenaryType.Melee : MercenaryType.Ranged;

            merc.inventory?.DestroyAll();

            merc.equipment?.DestroyAllEquipment();

            merc.apparel?.DestroyAll();

            try
            {
                Utils.processMercGear(merc, comp.type, gear, (int)selColor);
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                Utils.processMercWeapon(merc, comp.type, weapon);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        parms.raidArrivalMode.Worker.Arrive(list, parms);
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Points = {parms.points:F0}");
        foreach (var current in list)
        {
            var str = current.equipment == null || current.equipment.Primary == null
                ? "unarmed"
                : current.equipment.Primary.LabelCap;
            stringBuilder.AppendLine($"{current.KindLabel} - {str}");
        }

        TaggedString letterLabel = GetLetterLabel(parms);
        TaggedString letterText = GetLetterText(parms, list);
        PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(list, ref letterLabel, ref letterText,
            GetRelatedPawnsInfoLetterText(parms), true);
        var list2 = new List<TargetInfo>();
        if (parms.pawnGroups != null)
        {
            var list3 = IncidentParmsUtility.SplitIntoGroups(list, parms.pawnGroups);
            var list4 = list3.MaxBy(x => x.Count);
            if (list4.Any())
            {
                list2.Add(list4[0]);
            }

            foreach (var pawns in list3)
            {
                if (pawns == list4)
                {
                    continue;
                }

                if (pawns.Any())
                {
                    list2.Add(pawns[0]);
                }
            }
        }
        else if (list.Any())
        {
            list2.Add(list[0]);
        }

        Find.LetterStack.ReceiveLetter("MFM_LetterMercRaid".Translate(),
            "MFM_LetterMercRaidDesc".Translate(parms.faction.Name, Utils.getUSFMLabel(), "\n" + letterText),
            GetLetterDef(), list2, parms.faction);
        parms.raidStrategy.Worker.MakeLords(parms, list);
        return true;
    }
}