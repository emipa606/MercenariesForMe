using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(PawnBanishUtility), nameof(PawnBanishUtility.ShowBanishPawnConfirmationDialog))]
public class PawnBanishUtility_ShowBanishPawnConfirmationDialog
{
    public static bool Prefix(Pawn pawn)
    {
        var comp = pawn.TryGetComp<Comp_USFM>();

        //Player cannot bannish mercenary
        if (comp is not { isMercenary: true })
        {
            return true;
        }

        Messages.Message("MFM_MsgCannotBannishMercenary".Translate(), MessageTypeDefOf.NegativeEvent);
        return false;
    }
}