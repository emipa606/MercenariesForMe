using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    internal class Dialog_Trade_Patch
    {
        [HarmonyPatch(typeof(Dialog_Trade), "CacheTradeables")]
        public class CacheTradeables
        {
            [HarmonyPostfix]
            public static void Listener(ref List<Tradeable> ___cachedTradeables)
            {
                List<Tradeable> toDel = null;
                foreach (var entry in ___cachedTradeables)
                {
                    foreach(var e in entry.thingsColony)
                    {
                        if(e is Pawn)
                        {
                            Pawn pawn = (Pawn)e;
                            Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();
                            if (comp != null && comp.isMercenary && comp.hiredByPlayer) {
                                if (toDel == null)
                                    toDel = new List<Tradeable>();
                                toDel.Add(entry);
                                break;
                            }
                        }
                    }
                }

                if(toDel != null)
                {
                    foreach(var e in toDel)
                    {
                        if(___cachedTradeables.Contains(e))
                        ___cachedTradeables.Remove(e);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Dialog_Trade), "PostOpen")]
        static class Dialog_Trade_PostOpen_Postfix
        {
            [HarmonyPostfix]
            private static void PostOpen(Dialog_Trade __instance)
            {
                if (TradeSession.trader != null && TradeSession.trader.Faction != null && TradeSession.trader.Faction.def.defName == "USFM_FactionAOS")
                {
                    UnityEngine.Rect orig = __instance.windowRect;
                    __instance.windowRect = new UnityEngine.Rect(0, 0, 0, 0);
                    TradeOrHireRent diag = new TradeOrHireRent(delegate
                    {
                        __instance.windowRect = orig;
                    },delegate
                    {
                        Find.WindowStack.TryRemove(__instance);

                        ITrader trader = null;
                        Map map = null;
                        Pawn negociator = null;
                        Caravan caravan = null;

                        if(TradeSession.trader is Pawn)
                        {
                            trader = TradeSession.trader;
                            if (TradeSession.playerNegotiator == null)
                                return;
                            map = TradeSession.playerNegotiator.Map;
                            negociator = TradeSession.playerNegotiator;

                        }
                        else
                        {
                            if (TradeSession.playerNegotiator == null)
                                return;
                            caravan = TradeSession.playerNegotiator.GetCaravan();
                            if (caravan == null)
                                return;
                        }

                        Find.WindowStack.Add(new Dispatcher(negociator, map, caravan, trader));
                    },
                    delegate
                    {
                        Find.WindowStack.TryRemove(__instance);
                    });
                    diag.forcePause = true;
                    diag.focusWhenOpened = true;

                    Find.WindowStack.Add(diag);
                }
            }
        }


    }
}