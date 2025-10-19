using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.SetGuestStatus))]
public class Pawn_GuestTracker_SetGuestStatus
{
    public static void Postfix(GuestStatus guestStatus, Pawn ___pawn)
    {
        var comp = ___pawn.TryGetComp<Comp_USFM>();

        //If a mercenary then we stop the fact that he is a mercenary
        if (guestStatus != GuestStatus.Slave && guestStatus != GuestStatus.Prisoner || comp == null)
        {
            return;
        }

        if (!comp.isMercenary)
        {
            return;
        }

        comp.isMercenary = false;
        comp.hiredByPlayer = false;
    }
}