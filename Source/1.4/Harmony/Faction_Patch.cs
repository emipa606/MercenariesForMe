using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.MFM
{
    internal class Faction_Patch
    {
        [HarmonyPatch(typeof(Faction), "CommFloatMenuOption")]
        public class MakeDowned
        {
            [HarmonyPostfix]
            public static void Listener(Faction __instance, ref FloatMenuOption __result, Building_CommsConsole console, Pawn negotiator)
            {
                if (__instance.def.defName == "USFM_FactionAOS")
                {
                    __result = null;
                }
            }
        }
    }
}