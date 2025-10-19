using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace aRandomKiwi.MFM
{
    internal class Pawn_Patch
    {
        [HarmonyPatch(typeof(Pawn), "Kill")]
        public class Kill
        {
            [HarmonyPostfix]
            public static void Listener(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
            {
                //If VIP killed stop the job of guardian of all the mercenaries the guardan
                if (__instance != null)
                {
                    Comp_USFM comp = __instance.TryGetComp<Comp_USFM>();
                    if (comp != null)
                    {

                        if (comp.Level == MercenaryLevel.Cyborg)
                        {
                            List<HediffWithComps> lst = new List<HediffWithComps>();
                            __instance.health.hediffSet.GetHediffs<HediffWithComps>(ref lst);
                            //If cyborg belonging to the player ==> removal of all organs
                            foreach (var h in lst)
                            {
                                __instance.health.hediffSet.hediffs.Remove(h);
                            }
                        }
                    }
                }
            }
        }
    }
}