using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.MFM
{
    internal class JobDriver_Equip_Patch
    {
        [HarmonyPatch(typeof(JobDriver_Equip), "TryMakePreToilReservations")]
        public class TryMakePreToilReservations
        {
            [HarmonyPostfix]
            public static void Listener(bool errorOnFailed, Pawn ___pawn, ref bool __result)
            {
                if (___pawn == null)
                    return;
                Comp_USFM comp = ___pawn.TryGetComp<Comp_USFM>();
                if (comp != null && comp.isMercenary && comp.hiredByPlayer)
                {
                    __result = !Utils.counterOfferInProgressMsg(___pawn);
                }
            }
        }
    }
}