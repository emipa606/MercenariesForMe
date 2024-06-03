using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;

namespace aRandomKiwi.MFM
{
    internal class PawnBanishUtility_Patch
    {
        [HarmonyPatch(typeof(PawnBanishUtility), "ShowBanishPawnConfirmationDialog")]
        public class ShowBanishPawnConfirmationDialog
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn)
            {
                Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();

                //Player cannot bannish mercenary
                if (comp != null && comp.isMercenary)
                {
                    Messages.Message("MFM_MsgCannotBannishMercenary".Translate(), MessageTypeDefOf.NegativeEvent);
                    return false;
                }

                return true;
            }
        }
    }
}