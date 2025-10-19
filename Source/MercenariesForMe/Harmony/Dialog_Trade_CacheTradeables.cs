using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

[HarmonyPatch(typeof(Dialog_Trade), "CacheTradeables")]
public class Dialog_Trade_CacheTradeables
{
    public static void Postfix(ref List<Tradeable> ___cachedTradeables)
    {
        List<Tradeable> toDel = null;
        foreach (var entry in ___cachedTradeables)
        {
            foreach (var e in entry.thingsColony)
            {
                if (e is not Pawn pawn)
                {
                    continue;
                }

                var comp = pawn.TryGetComp<Comp_USFM>();
                if (comp is not { isMercenary: true, hiredByPlayer: true })
                {
                    continue;
                }

                toDel ??= [];

                toDel.Add(entry);
                break;
            }
        }

        if (toDel == null)
        {
            return;
        }

        foreach (var e in toDel)
        {
            if (___cachedTradeables.Contains(e))
            {
                ___cachedTradeables.Remove(e);
            }
        }
    }
}