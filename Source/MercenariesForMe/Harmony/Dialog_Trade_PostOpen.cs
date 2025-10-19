using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Dialog_Trade), "PostOpen")]
public static class Dialog_Trade_PostOpen
{
    private static void Postfix(Dialog_Trade __instance)
    {
        if (TradeSession.trader == null || TradeSession.trader.Faction == null ||
            TradeSession.trader.Faction.def.defName != "USFM_FactionAOS")
        {
            return;
        }

        var orig = __instance.windowRect;
        __instance.windowRect = new Rect(0, 0, 0, 0);
        var diag = new TradeOrHireRent(delegate { __instance.windowRect = orig; }, delegate
            {
                Find.WindowStack.TryRemove(__instance);

                ITrader trader = null;
                Map map = null;
                Pawn negociator = null;
                Caravan caravan = null;

                if (TradeSession.trader is Pawn)
                {
                    trader = TradeSession.trader;
                    if (TradeSession.playerNegotiator == null)
                    {
                        return;
                    }

                    map = TradeSession.playerNegotiator.Map;
                    negociator = TradeSession.playerNegotiator;
                }
                else
                {
                    if (TradeSession.playerNegotiator == null)
                    {
                        return;
                    }

                    caravan = TradeSession.playerNegotiator.GetCaravan();
                    if (caravan == null)
                    {
                        return;
                    }
                }

                Find.WindowStack.Add(new Dispatcher(negociator, map, caravan, trader));
            },
            delegate { Find.WindowStack.TryRemove(__instance); })
        {
            forcePause = true,
            focusWhenOpened = true
        };

        Find.WindowStack.Add(diag);
    }
}