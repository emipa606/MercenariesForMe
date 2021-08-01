using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System;

namespace aRandomKiwi.MFM
{
    internal class Pawn_ApparelTracker_Patch
    {
        [HarmonyPatch(typeof(Pawn_ApparelTracker), "TryDrop")]
        [HarmonyPatch(new Type[] { typeof(Apparel), typeof(Apparel), typeof(IntVec3), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
        public class TryDrop
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn ___pawn, Apparel ap, ref Apparel resultingAp, IntVec3 pos, bool forbid = true)
            {
                if (___pawn == null)
                    return true;
                Comp_USFM comp = ___pawn.TryGetComp<Comp_USFM>();
                if (comp != null && comp.isMercenary && comp.hiredByPlayer)
                {
                    return !Utils.counterOfferInProgressMsg(___pawn);
                }
                else
                    return true;
            }
        }
    }
}