using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

public class ChoiceLetter_UnRemovable : ChoiceLetter
{
    private bool particularCaseCond;
    public override IEnumerable<DiaOption> Choices => throw new NotImplementedException();

    public override void Removed()
    {
        base.Removed();


        if (Find.TickManager.TicksGame <= disappearAtTick || particularCaseCond)
        {
            return;
        }

        particularCaseCond = true;
        OpenLetter();
    }

    public override void DrawButtonAt(float topY)
    {
        var num = UI.screenWidth - 38f - 12f;
        var rect = new Rect(num, topY, 38f, 30f);
        var rect2 = new Rect(rect);
        var num2 = Time.time - arrivalTime;
        var color = def.color;
        if (num2 < 1f)
        {
            rect2.y -= (1f - num2) * 200f;
            color.a = num2 / 1f;
        }

        if (!Mouse.IsOver(rect) && def.bounce && num2 > 15f && num2 % 5f < 1f)
        {
            var num3 = UI.screenWidth * 0.06f;
            var num4 = (2f * (num2 % 1f)) - 1f;
            var num5 = num3 * (1f - (num4 * num4));
            rect2.x -= num5;
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (def.flashInterval > 0f)
            {
                var num6 = Time.time - (arrivalTime + 1f);
                if (num6 > 0f && num6 % def.flashInterval < 1f)
                {
                    GenUI.DrawFlash(num, topY, UI.screenWidth * 0.6f, Pulser.PulseBrightness(1f, 1f, num6) * 0.55f,
                        def.flashColor);
                }
            }

            GUI.color = color;
            Widgets.DrawShadowAround(rect2);
            GUI.DrawTexture(rect2, def.Icon);
            GUI.color = Color.white;
            Verse.Text.Anchor = TextAnchor.UpperRight;
            var text = PostProcessedLabel();
            var vector = Verse.Text.CalcSize(text);
            var x = vector.x;
            var y = vector.y;
            var vector2 = new Vector2(rect2.x + (rect2.width / 2f), rect2.center.y - (y / 2f) + 4f);
            var num7 = vector2.x + (x / 2f) - (UI.screenWidth - 2);
            if (num7 > 0f)
            {
                vector2.x -= num7;
            }

            var position = new Rect(vector2.x - (x / 2f) - 6f - 1f, vector2.y, x + 12f, 16f);
            GUI.DrawTexture(position, TexUI.GrayTextBG);
            GUI.color = new Color(1f, 1f, 1f, 0.75f);
            var rect3 = new Rect(vector2.x - (x / 2f), vector2.y - 3f, x, 999f);
            Widgets.Label(rect3, text);
            GUI.color = Color.white;
            Verse.Text.Anchor = TextAnchor.UpperLeft;
        }

        if (!Widgets.ButtonInvisible(rect2, false))
        {
            return;
        }

        OpenLetter();
        Event.current.Use();
    }
}