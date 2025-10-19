using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Faction), nameof(Faction.CommFloatMenuOption))]
public class Faction_CommFloatMenuOption
{
    public static void Postfix(Faction __instance, ref FloatMenuOption __result)
    {
        if (__instance.def.defName == "USFM_FactionAOS")
        {
            __result = null;
        }
    }
}