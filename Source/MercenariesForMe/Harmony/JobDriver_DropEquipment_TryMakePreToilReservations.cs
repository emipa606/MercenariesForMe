using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(JobDriver_DropEquipment), nameof(JobDriver_DropEquipment.TryMakePreToilReservations))]
public class JobDriver_DropEquipment_TryMakePreToilReservations
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