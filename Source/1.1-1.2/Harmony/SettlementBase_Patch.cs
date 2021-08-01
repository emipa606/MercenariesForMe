using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;

namespace aRandomKiwi.MFM
{
    internal class SettlementBase_Patch
    {
        [HarmonyPatch(typeof(Settlement), "GetCaravanGizmos")]
        public class MakeDowned
        {
            [HarmonyPostfix]
            public static void Listener(Settlement __instance, ref IEnumerable<Gizmo> __result, Caravan caravan)
            {
                if(__instance.Faction.def.defName == "USFM_FactionAOS")
                {
                    if(__result == null)
                        __result = new List<Gizmo>();
                    Command_Action el = new Command_Action
                    {
                        icon = Tex.medievalHireRent,
                        defaultLabel = "MFM_MedievalHireRent".Translate(),
                        defaultDesc = "MFM_MedievalHireRentDesc".Translate(),
                        action = delegate ()
                        {
                            Find.WindowStack.Add(new Dispatcher(null, null, caravan));
                        }
                    };

                    __result = __result.AddItem(el);
                }
            }
        }
    }
}