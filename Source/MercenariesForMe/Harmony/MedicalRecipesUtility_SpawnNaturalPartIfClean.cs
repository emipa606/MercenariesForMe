using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(MedicalRecipesUtility), nameof(MedicalRecipesUtility.SpawnNaturalPartIfClean))]
public class MedicalRecipesUtility_SpawnNaturalPartIfClean
{
    public static bool Prefix(Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
    {
        if (!MedicalRecipesUtility.IsCleanAndDroppable(pawn, part))
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