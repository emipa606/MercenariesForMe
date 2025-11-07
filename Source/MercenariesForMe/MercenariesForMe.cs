using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

[StaticConstructorOnStartup]
internal class MercenariesForMe : Mod
{
    private const string ID_EPOE = "Expanded Prosthetics and Organ Engineering";
    private const string ID_RSBE = "Rah's Bionics and Surgery Expansion";
    private const string ID_RSBE_HARD = "RBSE Hardcore Edition";
    private const string ID_EVO = "Evolved Organs";
    private const string ID_CONN = "Cybernetic Organism and Neural Network";
    private const string ID_MEDIEVAL_TIMES = "Medieval Times";
    private const string ID_GUARDS_FOR_ME = "Guards For Me";

    public MercenariesForMe(ModContentPack content) : base(content)
    {
        //Log.Message("Init MFM");
        GetSettings<Settings>();

        var assemblyCE = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.FullName.ToLower().StartsWith("combatextended"));
        if (assemblyCE != null)
        {
            Utils.CELOADED = true;
            Log.Message("[MFM] CE found");
        }

        var assemblyMSE = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(assembly => assembly.FullName.ToLower().StartsWith("orenomse"));
        if (assemblyMSE != null)
        {
            Utils.MSELOADED = true;
            Log.Message("[MFM] MSE found");
        }

        //EPOE Expanded Prosthetics and Organ Engineering
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == ID_EPOE))
        {
            Utils.EPOELOADED = true;
            Log.Message("[MFM] EPOE found");
        }

        //RSBE
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == ID_RSBE || x.Name == ID_RSBE_HARD))
        {
            Utils.RSBELOADED = true;
            Log.Message("[MFM] RSBE found");
        }

        //Check which one is first 
        if (Utils.EPOELOADED && Utils.RSBELOADED)
        {
            foreach (var m in LoadedModManager.RunningModsListForReading)
            {
                if (m.Name == ID_EPOE)
                {
                    Utils.RSBELOADED = false;
                    break;
                }

                if (m.Name != ID_RSBE && m.Name != ID_RSBE_HARD)
                {
                    continue;
                }

                Utils.EPOELOADED = false;
                break;
            }
        }

        //CONN
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == ID_CONN))
        {
            Utils.CONNLOADED = true;
            Log.Message("[MFM] CONN found");
        }

        //Evolved Organs
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == ID_EVO))
        {
            Utils.EVOLOADED = true;
            Log.Message("[MFM] EVO found");
        }

        //Medieval times
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == ID_MEDIEVAL_TIMES))
        {
            Utils.MEDIEVALTIMESLOADED = true;
            Log.Message("[MFM] MedievalTimes found");
        }

        //Guards For Me
        if (!LoadedModManager.RunningModsListForReading.Any(x => x.Name == ID_GUARDS_FOR_ME))
        {
            return;
        }

        Log.Message("[MFM] Guards For Me found");
    }

    public void Save()
    {
        LoadedModManager.GetMod<MercenariesForMe>().GetSettings<Settings>().Write();
    }

    public override string SettingsCategory()
    {
        return "Mercenaries For Me";
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Settings.DoSettingsWindowContents(inRect);
    }
}