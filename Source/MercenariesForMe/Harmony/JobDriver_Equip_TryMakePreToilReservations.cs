using HarmonyLib;
using Verse;
using Verse.AI;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(JobDriver_Equip), nameof(JobDriver_Equip.TryMakePreToilReservations))]
public class JobDriver_Equip_TryMakePreToilReservations
{
    public static void Postfix(Pawn ___pawn, ref bool __result)
    {
        var comp = ___pawn?.TryGetComp<Comp_USFM>();
        if (comp is { isMercenary: true, hiredByPlayer: true })
        {
            __result = !Utils.counterOfferInProgressMsg(___pawn);
        }
    }
}