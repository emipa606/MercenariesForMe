using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

public class Dispatcher : Window
{
    public static Vector2 scrollPosition = Vector2.zero;
    private readonly Pawn actor;
    private readonly Caravan caravan;

    private readonly Map map;
    private readonly ITrader trader;
    protected string curName;

    public Dispatcher(Pawn actor, Map map, Caravan caravan, ITrader trader = null)
    {
        this.trader = trader;
        this.actor = actor;
        this.map = map;
        this.caravan = caravan;
        forcePause = true;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
        closeOnClickedOutside = true;

        if (!Utils.modernUSFM())
        {
            return;
        }

        soundAmbient = SoundDefOf.RadioComms_Ambience;
        soundClose = SoundDefOf.CommsWindow_Close;
    }

    public override Vector2 InitialSize => new(820f, 715f);

    public override void DoWindowContents(Rect inRect)
    {
        inRect.yMin += 15f;
        inRect.yMax -= 15f;

        //Image logo
        Widgets.ButtonImage(new Rect(0, 0, 800, 595), Utils.modernUSFM() ? Tex.dispatcher : Tex.medievalDispatcher,
            Color.white, Color.white);

        var buttonWidth = 260f;
        if (!Utils.modernUSFM())
        {
            buttonWidth = 390f;
        }

        if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(Find.CurrentMap))
        {
            GUI.color = Color.green;
            Widgets.ButtonText(new Rect(0f, 610f, 780f, 35f), "MFM_DialogUnavailableOnSOS2Rimnauts2Map".Translate());

            GUI.color = Color.red;
            string textBtnCancel = Utils.modernUSFM() ? "Disconnect".Translate() : "CancelButton".Translate();

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

        if (Utils.modernUSFM() &&
            Widgets.ButtonText(new Rect(buttonWidth, 610f, buttonWidth, 35f), "MFM_PowerBeam".Translate()))
        {
            //Destination listing map
            var opts = new List<FloatMenuOption>();
            foreach (var m in Find.Maps)
            {
                if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(m))
                {
                    continue;
                }

                string lib;
                if (m == Find.CurrentMap)
                {
                    lib = "MFM_ThisCurrentMap".Translate(m.Parent.Label);
                }
                else
                {
                    lib = m.Parent.Label;
                }

                opts.Add(new FloatMenuOption(lib, delegate
                {
                    Find.WindowStack.TryRemove(this);
                    Current.Game.CurrentMap = m;
                    var x = new Designator_BeamTarget();
                    Find.DesignatorManager.Select(x);
                }));
            }

            if (opts.Count != 0)
            {
                var floatMenuMap = new FloatMenu(opts);
                Find.WindowStack.Add(floatMenuMap);
            }
        }

        float tmp;
        if (Utils.modernUSFM())
        {
            tmp = buttonWidth * 2;
        }
        else
        {
            tmp = buttonWidth;
        }

        if (Widgets.ButtonText(new Rect(tmp, 610f, buttonWidth, 35f), "MFM_RentMerc".Translate()))
        {
            Find.WindowStack.Add(new CentralHubRent(actor, map, caravan, trader));
            //Find.WindowStack.TryRemove(this);
        }

        Rect cancelRect;
        //Imprisoned mercenaries
        //MFM_ReturnPrisonedMercs
        if (Utils.isThereImprisonedMercs(map, caravan))
        {
            GUI.color = Color.white;
            windowRect.height = 750f;
            cancelRect = new Rect(0f, 680f, 780f, 35f);
            if (Widgets.ButtonText(new Rect(0f, 645f, 780f, 35f), "MFM_ReturnPrisonedMercs".Translate()))
            {
                Find.WindowStack.Add(new RestitutionOfPrisoners(actor, map, caravan));
                //Find.WindowStack.TryRemove(this);
            }
        }
        else
        {
            windowRect.height = 715f;
            cancelRect = new Rect(0f, 645f, 780f, 35f);
        }

        GUI.color = Color.red;
        string textBtn = Utils.modernUSFM() ? "Disconnect".Translate() : "CancelButton".Translate();

        if (Widgets.ButtonText(cancelRect, textBtn))
        {
            Find.WindowStack.TryRemove(this);
        }

        GUI.color = Color.white;
    }
}