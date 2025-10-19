using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Building_CommsConsole), nameof(Building_CommsConsole.GetFloatMenuOptions))]
public class Building_CommsConsole_GetFloatMenuOption
{
    public static void Postfix(Building_CommsConsole __instance, ref IEnumerable<FloatMenuOption> __result,
        Pawn myPawn)
    {
        var failureReason =
            (FloatMenuOption)Traverse.Create(__instance).Method("GetFailureReason", myPawn).GetValue();

        if (failureReason != null)
        {
            return;
        }

        var opt = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(
            "CallOnRadio".Translate("USFM (United Space Force Mercenaries)"), delegate
            {
                var job = new Job(DefDatabase<JobDef>.GetNamed("MFM_JobUseCommsConsoleUSFM"), __instance);
                myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);

                //__instance.GiveUseCommsJob(myPawn, Utils.commUSFM);
            }, Tex.USFMFactionTex, Color.red), myPawn, __instance);

        //Add dialog menu allowing to insult the interlocutor just before the finish button
        __result = __result.AddItem(opt);
    }
}