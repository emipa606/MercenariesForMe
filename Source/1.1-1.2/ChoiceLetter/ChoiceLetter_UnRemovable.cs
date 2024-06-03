using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace aRandomKiwi.MFM
{
    public class ChoiceLetter_UnRemovable : ChoiceLetter
    {
        private bool particularCaseCond = false;
        public override IEnumerable<DiaOption> Choices => throw new NotImplementedException();

        public override void Removed()
        {
            base.Removed();


            if (Find.TickManager.TicksGame > disappearAtTick && !particularCaseCond)
            {
                particularCaseCond = true;
                OpenLetter();
            }
        }

        public override void DrawButtonAt(float topY)
        {
            float num = (float)UI.screenWidth - 38f - 12f;
            Rect rect = new Rect(num, topY, 38f, 30f);
            Rect rect2 = new Rect(rect);
            float num2 = Time.time - this.arrivalTime;
            Color color = this.def.color;
            if (num2 < 1f)
            {
                rect2.y -= (1f - num2) * 200f;
                color.a = num2 / 1f;
            }
            if (!Mouse.IsOver(rect) && this.def.bounce && num2 > 15f && num2 % 5f < 1f)
            {
                float num3 = (float)UI.screenWidth * 0.06f;
                float num4 = 2f * (num2 % 1f) - 1f;
                float num5 = num3 * (1f - num4 * num4);
                rect2.x -= num5;
            }
            if (Event.current.type == EventType.Repaint)
            {
                if (this.def.flashInterval > 0f)
                {
                    float num6 = Time.time - (this.arrivalTime + 1f);
                    if (num6 > 0f && num6 % this.def.flashInterval < 1f)
                    {
                        GenUI.DrawFlash(num, topY, (float)UI.screenWidth * 0.6f, Pulser.PulseBrightness(1f, 1f, num6) * 0.55f, this.def.flashColor);
                    }
                }
                GUI.color = color;
                Widgets.DrawShadowAround(rect2);
                GUI.DrawTexture(rect2, this.def.Icon);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperRight;
                string text = this.PostProcessedLabel();
                Vector2 vector = Text.CalcSize(text);
                float x = vector.x;
                float y = vector.y;
                Vector2 vector2 = new Vector2(rect2.x + rect2.width / 2f, rect2.center.y - y / 2f + 4f);
                float num7 = vector2.x + x / 2f - (float)(UI.screenWidth - 2);
                if (num7 > 0f)
                {
                    vector2.x -= num7;
                }
                Rect position = new Rect(vector2.x - x / 2f - 6f - 1f, vector2.y, x + 12f, 16f);
                GUI.DrawTexture(position, TexUI.GrayTextBG);
                GUI.color = new Color(1f, 1f, 1f, 0.75f);
                Rect rect3 = new Rect(vector2.x - x / 2f, vector2.y - 3f, x, 999f);
                Widgets.Label(rect3, text);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            if (Widgets.ButtonInvisible(rect2, false))
            {
                this.OpenLetter();
                Event.current.Use();
            }
        }
    }
}
