using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

[StaticConstructorOnStartup]
internal static class Tex
{
    public static readonly Material artist = MaterialPool.MatFrom("UI/MFM_Artist", ShaderDatabase.MetaOverlay);
    public static readonly Material builder = MaterialPool.MatFrom("UI/MFM_Builder", ShaderDatabase.MetaOverlay);
    public static readonly Material cooker = MaterialPool.MatFrom("UI/MFM_Cooker", ShaderDatabase.MetaOverlay);
    public static readonly Material farmer = MaterialPool.MatFrom("UI/MFM_Farmer", ShaderDatabase.MetaOverlay);
    public static readonly Material medical = MaterialPool.MatFrom("UI/MFM_Medical", ShaderDatabase.MetaOverlay);
    public static readonly Material miner = MaterialPool.MatFrom("UI/MFM_Miner", ShaderDatabase.MetaOverlay);
    public static readonly Material scientist = MaterialPool.MatFrom("UI/MFM_Scientist", ShaderDatabase.MetaOverlay);
    public static readonly Material melee = MaterialPool.MatFrom("UI/MFM_Melee", ShaderDatabase.MetaOverlay);
    public static readonly Material ranged = MaterialPool.MatFrom("UI/MFM_Ranged", ShaderDatabase.MetaOverlay);
    public static readonly Material speak = MaterialPool.MatFrom("UI/MFM_Speak", ShaderDatabase.MetaOverlay);
    public static readonly Material tech = MaterialPool.MatFrom("UI/MFM_Tech", ShaderDatabase.MetaOverlay);
    public static readonly Material trainer = MaterialPool.MatFrom("UI/MFM_Trainer", ShaderDatabase.MetaOverlay);
    public static readonly Material guardian = MaterialPool.MatFrom("UI/MFM_Guardian", ShaderDatabase.MetaOverlay);

    public static readonly Material ironAlliance =
        MaterialPool.MatFrom("UI/MFM_IronAlliance", ShaderDatabase.MetaOverlay);

    public static readonly Material ironAllianceTrader =
        MaterialPool.MatFrom("UI/MFM_IronAllianceTrader", ShaderDatabase.MetaOverlay);


    public static readonly Material level1 = MaterialPool.MatFrom("UI/MFM_Level", ShaderDatabase.MetaOverlay);

    public static readonly Material
        levelCyborg = MaterialPool.MatFrom("UI/MFM_LevelCyborg", ShaderDatabase.MetaOverlay);


    public static readonly Texture2D beamTarget = ContentFinder<Texture2D>.Get("UI/MFM_BeamTarget");
    public static readonly Texture2D medievalHireRent = ContentFinder<Texture2D>.Get("UI/MFM_HireRent");

    public static readonly Texture2D catArtist = ContentFinder<Texture2D>.Get("UI/Cats/CatArtist");
    public static readonly Texture2D catBuilder = ContentFinder<Texture2D>.Get("UI/Cats/CatBuilder");
    public static readonly Texture2D catFarmer = ContentFinder<Texture2D>.Get("UI/Cats/CatFarmer");
    public static readonly Texture2D catScientist = ContentFinder<Texture2D>.Get("UI/Cats/CatScientist");
    public static readonly Texture2D catMiner = ContentFinder<Texture2D>.Get("UI/Cats/CatMiner");
    public static readonly Texture2D catSpeak = ContentFinder<Texture2D>.Get("UI/Cats/CatSpeak");
    public static readonly Texture2D catMedical = ContentFinder<Texture2D>.Get("UI/Cats/CatMedical");
    public static readonly Texture2D catTrainer = ContentFinder<Texture2D>.Get("UI/Cats/CatTrainer");
    public static readonly Texture2D catTech = ContentFinder<Texture2D>.Get("UI/Cats/CatTech");
    public static readonly Texture2D catCooker = ContentFinder<Texture2D>.Get("UI/Cats/CatCooker");
    public static readonly Texture2D catSoldier = ContentFinder<Texture2D>.Get("UI/Cats/CatSoldier");
    public static readonly Texture2D catSoldier2 = ContentFinder<Texture2D>.Get("UI/Cats/CatSoldier2");
    public static readonly Texture2D catShipping = ContentFinder<Texture2D>.Get("UI/Cats/CatShipping");
    public static readonly Texture2D catGears = ContentFinder<Texture2D>.Get("UI/Cats/CatGears");
    public static readonly Texture2D catWeapons = ContentFinder<Texture2D>.Get("UI/Cats/CatWeapons");
    public static readonly Texture2D catCurrentOrders = ContentFinder<Texture2D>.Get("UI/Cats/CatCurrentOrders");

    public static readonly Texture2D
        catStuffAndGuarantee = ContentFinder<Texture2D>.Get("UI/Cats/CatStuffAndGuarantee");

    public static readonly Texture2D catRecall = ContentFinder<Texture2D>.Get("UI/Cats/CatRecall");
    public static readonly Texture2D catRent = ContentFinder<Texture2D>.Get("UI/Cats/CatRent");
    public static readonly Texture2D catPreferences = ContentFinder<Texture2D>.Get("UI/Cats/CatPreferences");


    public static readonly Texture2D fired = ContentFinder<Texture2D>.Get("UI/MFM_Fired");

    public static readonly Texture2D dispatcher = ContentFinder<Texture2D>.Get("UI/Dispatcher");
    public static readonly Texture2D centralHubHire = ContentFinder<Texture2D>.Get("UI/Banner/CentralHubHire");
    public static readonly Texture2D centralHubRent = ContentFinder<Texture2D>.Get("UI/Banner/CentralHubRent");
    public static readonly Texture2D summary = ContentFinder<Texture2D>.Get("UI/Banner/Summary");
    public static readonly Texture2D quadrumBill = ContentFinder<Texture2D>.Get("UI/Banner/QuadrumBill");

    public static readonly Texture2D restitutionOfPrisoners =
        ContentFinder<Texture2D>.Get("UI/Banner/RestitutionOfPrisoners");

    public static readonly Texture2D medievalDispatcher = ContentFinder<Texture2D>.Get("UI/DispatcherMedieval");

    public static readonly Texture2D medievalCentralHubHire =
        ContentFinder<Texture2D>.Get("UI/BannerMedieval/CentralHubHire");

    public static readonly Texture2D medievalCentralHubRent =
        ContentFinder<Texture2D>.Get("UI/BannerMedieval/CentralHubRent");

    public static readonly Texture2D medievalSummary = ContentFinder<Texture2D>.Get("UI/BannerMedieval/Summary");

    public static readonly Texture2D
        medievalQuadrumBill = ContentFinder<Texture2D>.Get("UI/BannerMedieval/QuadrumBill");

    public static readonly Texture2D medievalRestitutionOfPrisoners =
        ContentFinder<Texture2D>.Get("UI/BannerMedieval/RestitutionOfPrisoners");

    public static readonly Texture2D update = ContentFinder<Texture2D>.Get("UI/MFM_Update");

    public static readonly Texture2D USFMFactionTex = ContentFinder<Texture2D>.Get("World/USFMR");

    static Tex()
    {
    }
}