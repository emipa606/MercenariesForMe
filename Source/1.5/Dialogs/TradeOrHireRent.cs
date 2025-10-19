using System;
using Verse;
using RimWorld;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class TradeOrHireRent : Window
    {
        Action trade;
        Action hireRent;
        Action cancel;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(300f, 220f);
            }
        }

        public TradeOrHireRent(Action t, Action hr, Action c)
        {
            this.forcePause = true;
            this.doCloseX = false;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = false;
            this.closeOnCancel = false;
            this.trade = t;
            this.hireRent = hr;
            this.cancel = c;
        }


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
}