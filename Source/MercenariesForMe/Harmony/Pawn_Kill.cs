using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
public class Pawn_Kill
{
    public static void Postfix(Pawn __instance)
    {
        //If VIP killed stop the job of guardian of all the mercenaries the guardan

        var comp = __instance?.TryGetComp<Comp_USFM>();
        if (comp == null)
        {
            return;
        }

        if (comp.Level != MercenaryLevel.Cyborg)
        {
            return;
        }

        var lst = new List<HediffWithComps>();
        __instance.health.hediffSet.GetHediffs(ref lst);
        //If cyborg belonging to the player ==> removal of all organs
        foreach (var h in lst)
        {
            __instance.health.hediffSet.hediffs.Remove(h);
        }
    }
}