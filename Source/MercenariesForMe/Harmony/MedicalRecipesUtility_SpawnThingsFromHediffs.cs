using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(MedicalRecipesUtility), nameof(MedicalRecipesUtility.SpawnThingsFromHediffs))]
public class MedicalRecipesUtility_SpawnThingsFromHediffs
{
    public static bool Prefix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
    {
        if (!pawn.health.hediffSet.GetNotMissingParts().Contains(part))
        {
            return true;
        }

        var comp = pawn.TryGetComp<Comp_USFM>();
        //Si mercenaire du joueur alors affichage msg d'erreur
        if (comp is { isMercenary: true, hiredByPlayer: true })
        {
            return !HarmonyPatches.RemovePartFail(pos, map);
        }

        return true;
    }
}