using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("rimworld.randomKiwi.MFM").PatchAll(Assembly.GetExecutingAssembly());
    }


    public static bool RemovePartFail(IntVec3 pos, Map map)
    {
        //Exlosion de l'élément enlevé
        if (!Rand.Chance(Settings.riskCyborgRemovingPartExplode))
        {
            return false;
        }

        Find.LetterStack.ReceiveLetter("MFM_LetterMercPartRemovingExplosion".Translate(),
            "MFM_LetterMercPartRemovingExplosionDesc".Translate(), LetterDefOf.NegativeEvent,
            new LookTargets(pos, map));
        GenExplosion.DoExplosion(pos, map, 1.9f, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f,
            1, null, null, 0);

        return true;
    }
}