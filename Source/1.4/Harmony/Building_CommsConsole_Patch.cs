using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace aRandomKiwi.MFM
{
    internal class Building_CommsConsole_GetFloatMenuOption_Patch
    {
        [HarmonyPatch(typeof(Building_CommsConsole), "GetFloatMenuOptions")]
        public class GetFloatMenuOptions
        {
            [HarmonyPostfix]
            public static void Listener(Building_CommsConsole __instance, ref IEnumerable<FloatMenuOption> __result, Pawn myPawn)
            {
                FloatMenuOption failureReason = (FloatMenuOption)Traverse.Create(__instance).Method("GetFailureReason", myPawn).GetValue();

                if (failureReason == null)
                {
                    FloatMenuOption opt = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("CallOnRadio".Translate("USFM (United Space Force Mercenaries)"), delegate
                    {
                        Job job = new Job(DefDatabase<JobDef>.GetNamed("MFM_JobUseCommsConsoleUSFM"), __instance);
                        myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);

                        //__instance.GiveUseCommsJob(myPawn, Utils.commUSFM);
                    }, Tex.USFMFactionTex, Color.red), myPawn, __instance, "ReservedBy");

                    //Add dialog menu allowing to insult the interlocutor just before the finish button
                    __result = __result.AddItem(opt);
                }
            }
        }
    }
}