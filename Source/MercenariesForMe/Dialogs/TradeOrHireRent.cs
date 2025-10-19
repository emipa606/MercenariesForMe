using System;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

public class TradeOrHireRent : Window
{
    private readonly Action cancel;
    private readonly Action hireRent;
    private readonly Action trade;

    public TradeOrHireRent(Action t, Action hr, Action c)
    {
        forcePause = true;
        doCloseX = false;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
        closeOnClickedOutside = false;
        closeOnCancel = false;
        trade = t;
        hireRent = hr;
        cancel = c;
    }

    public override Vector2 InitialSize => new(300f, 220f);


    public override void DoWindowContents(Rect inRect)
    {
        if (Widgets.ButtonText(new Rect(0f, 30f, 260f, 30f), "CaravanMeeting_Trade".Translate()))
        {
            trade();
            Find.WindowStack.TryRemove(this);
        }

        if (Widgets.ButtonText(new Rect(0f, 80f, 260f, 30f), "MFM_MedievalHireRent".Translate()))
        {
            hireRent();
            Find.WindowStack.TryRemove(this);
        }

        GUI.color = Color.red;
        if (Widgets.ButtonText(new Rect(0f, 130f, 260f, 30f), "CancelButton".Translate()))
        {
            cancel();
            Find.WindowStack.TryRemove(this);
        }

        GUI.color = Color.white;
    }
}