using System;
using Verse;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class Dispatcher : Window
    {
        protected string curName;

        public Map map;
        public Pawn actor;
        public Caravan caravan;
        public ITrader trader;

        public static Vector2 scrollPosition = Vector2.zero;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(820f, 715f);
            }
        }

        public Dispatcher(Pawn actor, Map map,Caravan caravan, ITrader trader=null)
        {
            this.trader = trader;
            this.actor = actor;
            this.map = map;
            this.caravan = caravan;
            this.forcePause = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = true;

            if (Utils.modernUSFM())
            {
                soundAmbient = SoundDefOf.RadioComms_Ambience;
                soundClose = SoundDefOf.CommsWindow_Close;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            inRect.yMin += 15f;
            inRect.yMax -= 15f;
            var defaultColumnWidth = (inRect.width - 50);

            //Image logo
            if(Utils.modernUSFM())
                Widgets.ButtonImage(new Rect(0, 0, 800, 595), Tex.dispatcher, Color.white, Color.white);
            else
                Widgets.ButtonImage(new Rect(0, 0, 800, 595), Tex.medievalDispatcher, Color.white, Color.white);

            float buttonWidth = 260f;
            if (!Utils.modernUSFM())
            {
                buttonWidth = 390f;
            }

            if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(Find.CurrentMap))
            {
                GUI.color = Color.green;
                Widgets.ButtonText(new Rect(0f, 610f, 780f, 35f), "MFM_DialogUnavailableOnSOS2Rimnauts2Map".Translate());

                GUI.color = Color.red;
                string textBtnCancel;
                if (Utils.modernUSFM())
                    textBtnCancel = "Disconnect".Translate();
                else
                    textBtnCancel = "CancelButton".Translate();

                if (Widgets.ButtonText(new Rect(0f, 645f, 780f, 35f), textBtnCancel))
                {
                    Find.WindowStack.TryRemove(this);
                }
                GUI.color = Color.white;
                return;
            }
            GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(0f, 610f, buttonWidth, 35f), "MFM_HireMerc".Translate()))
            {
                Find.WindowStack.Add(new CentralHubHire(actor, map, caravan, trader));
            }
            if (Utils.modernUSFM() && Widgets.ButtonText(new Rect(buttonWidth, 610f, buttonWidth, 35f), "MFM_PowerBeam".Translate()))
            {
                //Destination listing map
                List<FloatMenuOption> opts = new List<FloatMenuOption>();
                string lib = "";
                foreach (var m in Find.Maps)
                {
                    if (m == Find.CurrentMap)
                        lib = "MFM_ThisCurrentMap".Translate(m.Parent.Label);
                    else
                        lib = m.Parent.Label;

                    opts.Add(new FloatMenuOption(lib, delegate
                    {
                        Find.WindowStack.TryRemove(this);
                        Current.Game.CurrentMap = m;
                        Designator_BeamTarget x = new Designator_BeamTarget();
                        Find.DesignatorManager.Select(x);

                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                if (opts.Count != 0)
                {
                    FloatMenu floatMenuMap = new FloatMenu(opts);
                    Find.WindowStack.Add(floatMenuMap);
                }
            }
            float tmp = 0;
            if (Utils.modernUSFM())
                tmp = buttonWidth * 2;
            else
                tmp = buttonWidth;
            if (Widgets.ButtonText(new Rect(tmp, 610f, buttonWidth, 35f), "MFM_RentMerc".Translate()))
            {
                Find.WindowStack.Add(new CentralHubRent(actor, map, caravan,trader));
                //Find.WindowStack.TryRemove(this);
            }

            Rect cancelRect;
            //Imprisoned mercenaries
            //MFM_ReturnPrisonedMercs
            if (Utils.isThereImprisonedMercs(map, caravan))
            {
                GUI.color = Color.white;
                this.windowRect.height = 750f;
                cancelRect = new Rect(0f, 680f, 780f, 35f);
                if (Widgets.ButtonText(new Rect(0f, 645f, 780f, 35f), "MFM_ReturnPrisonedMercs".Translate()))
                {
                    Find.WindowStack.Add(new RestitutionOfPrisoners(actor, map, caravan, trader));
                    //Find.WindowStack.TryRemove(this);
                }
            }
            else
            {
                this.windowRect.height = 715f;
                cancelRect = new Rect(0f, 645f, 780f, 35f);
            }

            GUI.color = Color.red;
            string textBtn;
            if (Utils.modernUSFM())
                textBtn = "Disconnect".Translate();
            else
                textBtn = "CancelButton".Translate();

            if (Widgets.ButtonText(cancelRect, textBtn))
            {
                Find.WindowStack.TryRemove(this);
            }
            GUI.color = Color.white;
        }
    }
}