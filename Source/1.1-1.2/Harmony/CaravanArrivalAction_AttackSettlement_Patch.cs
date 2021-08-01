using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;

namespace aRandomKiwi.MFM
{
    internal class CaravanArrivalAction_AttackSettlement_Patch
    {
        [HarmonyPatch(typeof(CaravanArrivalAction_AttackSettlement), "GetFloatMenuOptions")]
        public class GetFloatMenuOptions
        {
            [HarmonyPostfix]
            public static void Listener(ref IEnumerable<FloatMenuOption> __result, Caravan caravan, Settlement settlement)
            {
                if(settlement.Faction.def.defName == "USFM_FactionAOS")
                {
                    __result = new List<FloatMenuOption>();

                }
            }
        }
    }
}