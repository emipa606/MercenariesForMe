using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Linq;

namespace aRandomKiwi.MFM
{
    internal class Pawn_GuestTracker_Patch
    {
        [HarmonyPatch(typeof(Pawn_GuestTracker), "SetGuestStatus")]
        public class SetGuestStatus_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Faction newHost, bool prisoner, Pawn ___pawn)
            {
                Comp_USFM comp = ___pawn.TryGetComp<Comp_USFM>();

                //If a mercenary then we stop the fact that he is a mercenary
                if (prisoner && comp != null)
                {
                    if (comp.isMercenary)
                    {
                        comp.isMercenary = false;
                        comp.hiredByPlayer = false;
                    }
                }
            }
        }
    }
}