using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace aRandomKiwi.MFM
{
    [StaticConstructorOnStartup]
    static class Tex
    {
        static Tex()
        {
        }

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
        public static readonly Material ironAlliance = MaterialPool.MatFrom("UI/MFM_IronAlliance", ShaderDatabase.MetaOverlay);
        public static readonly Material ironAllianceTrader = MaterialPool.MatFrom("UI/MFM_IronAllianceTrader", ShaderDatabase.MetaOverlay);



        public static readonly Material level1 = MaterialPool.MatFrom("UI/MFM_Level", ShaderDatabase.MetaOverlay);
        public static readonly Material levelCyborg = MaterialPool.MatFrom("UI/MFM_LevelCyborg", ShaderDatabase.MetaOverlay);

        
        public static readonly Texture2D beamTarget = ContentFinder<Texture2D>.Get("UI/MFM_BeamTarget", true);
        public static readonly Texture2D medievalHireRent = ContentFinder<Texture2D>.Get("UI/MFM_HireRent", true);

        public static readonly Texture2D catArtist = ContentFinder<Texture2D>.Get("UI/Cats/CatArtist", true);
        public static readonly Texture2D catBuilder = ContentFinder<Texture2D>.Get("UI/Cats/CatBuilder", true);
        public static readonly Texture2D catFarmer = ContentFinder<Texture2D>.Get("UI/Cats/CatFarmer", true);
        public static readonly Texture2D catScientist = ContentFinder<Texture2D>.Get("UI/Cats/CatScientist", true);
        public static readonly Texture2D catMiner = ContentFinder<Texture2D>.Get("UI/Cats/CatMiner", true);
        public static readonly Texture2D catSpeak = ContentFinder<Texture2D>.Get("UI/Cats/CatSpeak", true);
        public static readonly Texture2D catMedical = ContentFinder<Texture2D>.Get("UI/Cats/CatMedical", true);
        public static readonly Texture2D catTrainer = ContentFinder<Texture2D>.Get("UI/Cats/CatTrainer", true);
        public static readonly Texture2D catTech = ContentFinder<Texture2D>.Get("UI/Cats/CatTech", true);
        public static readonly Texture2D catCooker = ContentFinder<Texture2D>.Get("UI/Cats/CatCooker", true);
        public static readonly Texture2D catSoldier = ContentFinder<Texture2D>.Get("UI/Cats/CatSoldier", true);
        public static readonly Texture2D catSoldier2 = ContentFinder<Texture2D>.Get("UI/Cats/CatSoldier2", true);
        public static readonly Texture2D catShipping = ContentFinder<Texture2D>.Get("UI/Cats/CatShipping", true);
        public static readonly Texture2D catGears = ContentFinder<Texture2D>.Get("UI/Cats/CatGears", true);
        public static readonly Texture2D catWeapons = ContentFinder<Texture2D>.Get("UI/Cats/CatWeapons", true);
        public static readonly Texture2D catCurrentOrders = ContentFinder<Texture2D>.Get("UI/Cats/CatCurrentOrders", true);
        public static readonly Texture2D catStuffAndGuarantee = ContentFinder<Texture2D>.Get("UI/Cats/CatStuffAndGuarantee", true);
        public static readonly Texture2D catRecall = ContentFinder<Texture2D>.Get("UI/Cats/CatRecall", true);
        public static readonly Texture2D catRent = ContentFinder<Texture2D>.Get("UI/Cats/CatRent", true);
        public static readonly Texture2D catPreferences = ContentFinder<Texture2D>.Get("UI/Cats/CatPreferences", true);


        public static readonly Texture2D fired = ContentFinder<Texture2D>.Get("UI/MFM_Fired", true);

        public static readonly Texture2D dispatcher = ContentFinder<Texture2D>.Get("UI/Dispatcher", true);
        public static readonly Texture2D centralHubHire = ContentFinder<Texture2D>.Get("UI/Banner/CentralHubHire", true);
        public static readonly Texture2D centralHubRent = ContentFinder<Texture2D>.Get("UI/Banner/CentralHubRent", true);
        public static readonly Texture2D summary = ContentFinder<Texture2D>.Get("UI/Banner/Summary", true);
        public static readonly Texture2D quadrumBill = ContentFinder<Texture2D>.Get("UI/Banner/QuadrumBill", true);
        public static readonly Texture2D restitutionOfPrisoners = ContentFinder<Texture2D>.Get("UI/Banner/RestitutionOfPrisoners", true);

        public static readonly Texture2D medievalDispatcher = ContentFinder<Texture2D>.Get("UI/DispatcherMedieval", true);
        public static readonly Texture2D medievalCentralHubHire = ContentFinder<Texture2D>.Get("UI/BannerMedieval/CentralHubHire", true);
        public static readonly Texture2D medievalCentralHubRent = ContentFinder<Texture2D>.Get("UI/BannerMedieval/CentralHubRent", true);
        public static readonly Texture2D medievalSummary = ContentFinder<Texture2D>.Get("UI/BannerMedieval/Summary", true);
        public static readonly Texture2D medievalQuadrumBill = ContentFinder<Texture2D>.Get("UI/BannerMedieval/QuadrumBill", true);
        public static readonly Texture2D medievalRestitutionOfPrisoners = ContentFinder<Texture2D>.Get("UI/BannerMedieval/RestitutionOfPrisoners", true);

        public static readonly Texture2D update = ContentFinder<Texture2D>.Get("UI/MFM_Update", true);

        public static readonly Texture2D USFMFactionTex = ContentFinder<Texture2D>.Get("World/USFMR", true);
    }
}
