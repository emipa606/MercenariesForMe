using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace aRandomKiwi.MFM
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var inst = new Harmony("rimworld.randomKiwi.MFM");
            inst.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static FieldInfo MapFieldInfo;
    }
}
