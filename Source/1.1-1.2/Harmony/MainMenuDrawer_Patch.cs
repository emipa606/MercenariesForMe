using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using RimWorld;
using HarmonyLib;
using Verse;
using Verse.Sound;
using System.Reflection;

namespace aRandomKiwi.MFM
{
    [HarmonyPatch(typeof(MainMenuDrawer), "DoMainMenuControls"), StaticConstructorOnStartup]
    class DoMainMenuControls_Patch
    {
        [HarmonyPostfix]
        static void Postfix(Rect rect, bool anyMapFiles)
        {
            //Display update information if applicable
            if (Settings.lastVersionInfo != Utils.releaseInfo)
            {
                try
                {
                    Find.WindowStack.Add(new Dialog_Update());
                    Settings.lastVersionInfo = Utils.releaseInfo;
                    Utils.currentModInst.WriteSettings();
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}