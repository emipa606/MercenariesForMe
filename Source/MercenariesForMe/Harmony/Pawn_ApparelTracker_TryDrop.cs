using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.TryDrop))]
[HarmonyPatch([typeof(Apparel), typeof(Apparel), typeof(IntVec3), typeof(bool)],
    [ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal])]
public class Pawn_ApparelTracker_TryDrop
{
    public static bool Prefix(Pawn ___pawn)
    {
        var comp = ___pawn?.TryGetComp<Comp_USFM>();
        if (comp is { isMercenary: true, hiredByPlayer: true })
        {
            return !Utils.counterOfferInProgressMsg(___pawn);
        }

        return true;
    }
}