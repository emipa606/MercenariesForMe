using System.Collections.Generic;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Settlement), nameof(Settlement.GetCaravanGizmos))]
public class Settlement_GetCaravanGizmos
{
    public static void Postfix(Settlement __instance, ref IEnumerable<Gizmo> __result, Caravan caravan)
    {
        if (__instance.Faction.def.defName != "USFM_FactionAOS")
        {
            return;
        }

        __result ??= new List<Gizmo>();

        var el = new Command_Action
        {
            icon = Tex.medievalHireRent,
            defaultLabel = "MFM_MedievalHireRent".Translate(),
            defaultDesc = "MFM_MedievalHireRentDesc".Translate(),
            action = delegate { Find.WindowStack.Add(new Dispatcher(null, null, caravan)); }
        };

        __result = __result.AddItem(el);
    }
}