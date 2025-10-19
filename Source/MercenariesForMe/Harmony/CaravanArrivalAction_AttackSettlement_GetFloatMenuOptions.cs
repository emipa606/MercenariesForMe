using System.Collections.Generic;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(CaravanArrivalAction_AttackSettlement),
    nameof(CaravanArrivalAction_AttackSettlement.GetFloatMenuOptions))]
public class CaravanArrivalAction_AttackSettlement_GetFloatMenuOptions
{
    [HarmonyPostfix]
    public static void Postfix(ref IEnumerable<FloatMenuOption> __result, Settlement settlement)
    {
        if (settlement.Faction.def.defName == "USFM_FactionAOS")
        {
            __result = new List<FloatMenuOption>();
        }
    }
}