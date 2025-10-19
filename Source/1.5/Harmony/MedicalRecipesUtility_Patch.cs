using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Linq;

namespace aRandomKiwi.MFM
{
    internal class MedicalRecipesUtility_Patch
    {
        [HarmonyPatch(typeof(MedicalRecipesUtility), "SpawnThingsFromHediffs")]
        public class SpawnThingsFromHediffs_SpawnThingsFromHediffs
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
            {
                if (!pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Contains(part))
                {
                    return true;
                }
                Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();
                //Si mercenaire du joueur alors affichage msg d'erreur
                if (comp != null && comp.isMercenary && comp.hiredByPlayer)
                {
                    return !removePartFail(pos, map);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MedicalRecipesUtility), "SpawnNaturalPartIfClean")]
        public class SpawnThingsFromHediffs_SpawnNaturalPartIfClean
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
            {
                if (MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
                {
                    Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();
                    //Si mercenaire du joueur alors affichage msg d'erreur
                    if (comp != null && comp.isMercenary && comp.hiredByPlayer)
                    {
                        return !removePartFail(pos, map);
                    }
                }
                return true;
            }
        }

        private static bool removePartFail(IntVec3 pos, Map map)
        {
            //Exlosion de l'élément enlevé
            if (Rand.Chance(Settings.riskCyborgRemovingPartExplode))
            {
                Find.LetterStack.ReceiveLetter("MFM_LetterMercPartRemovingExplosion".Translate(), "MFM_LetterMercPartRemovingExplosionDesc".Translate(), LetterDefOf.NegativeEvent, new LookTargets(pos, map));
                GenExplosion.DoExplosion(pos, map, 1.9f, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1,null, false, null, 0f, 1, 0f, false);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}