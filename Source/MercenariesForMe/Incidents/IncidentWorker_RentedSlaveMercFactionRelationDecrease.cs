using System;
using System.Linq;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class IncidentWorker_RentedSlaveMercFactionRelationDecrease : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return Utils.GCMFM.playerHaveRentedSlaveMerc();
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (!Utils.GCMFM.playerHaveRentedSlaveMerc())
        {
            return false;
        }

        var merc = Utils.GCMFM.getRandomRentedSlaveMerc();
        if (merc == null)
        {
            return false;
        }

        try
        {
            (from x in Find.FactionManager.AllFactions
                where !x.IsPlayer && x.def.humanlikeFaction && !x.defeated && !x.def.hidden &&
                      x.def.defName != "USFM_FactionAOS"
                select x).TryRandomElement(out var fac);

            if (fac == null)
            {
                return false;
            }

            fac.TryAffectGoodwillWith(Faction.OfPlayer, -1 * Rand.Range(5, 21));
            Find.LetterStack.ReceiveLetter("MFM_LetterRentedSlaveMercFactionRelationDecrease".Translate(),
                "MFM_LetterRentedSlaveMercFactionRelationDecreaseDesc".Translate(fac.Name, merc.LabelCap),
                LetterDefOf.NegativeEvent);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}