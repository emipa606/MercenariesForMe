using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace aRandomKiwi.MFM
{
    internal class InteractionWorker_RecruitAttempt_Patch
    {
        [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "DoRecruit")]
        [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(string), typeof(string), typeof(bool), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal })]
        public class DoRecruit
        {
            [HarmonyPostfix]
            public static void Listener(Pawn recruiter, Pawn recruitee)
            {
                if (recruitee == null)
                    return;

                Comp_USFM comp = recruitee.TryGetComp<Comp_USFM>();
                //If it is not a mercenary belonging to the player ==> it is defined as no longer a mercenary
                if (comp != null && !comp.hiredByPlayer)
                {
                    comp.isMercenary = false;
                    comp.restoreOrigSkills();
                }
            }
        }
    }
}