using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), nameof(InteractionWorker_RecruitAttempt.DoRecruit))]
[HarmonyPatch([typeof(Pawn), typeof(Pawn), typeof(string), typeof(string), typeof(bool), typeof(bool)],
[
    ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Normal,
    ArgumentType.Normal
])]
public class InteractionWorker_RecruitAttempt_DoRecruit
{
    public static void Postfix(Pawn recruitee)
    {
        var comp = recruitee?.TryGetComp<Comp_USFM>();
        //If it is not a mercenary belonging to the player ==> it is defined as no longer a mercenary
        if (comp is not { hiredByPlayer: false })
        {
            return;
        }

        comp.isMercenary = false;
        comp.restoreOrigSkills();
    }
}