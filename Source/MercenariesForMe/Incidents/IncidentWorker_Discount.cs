using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class IncidentWorker_Discount : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return true;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var discount = Rand.Range(Settings.minDiscount, Settings.maxDiscount);
        var code = Utils.generateDiscountCode((uint)Find.TickManager.TicksGame);

        Utils.GCMFM.addDiscount(code, discount);

        Find.LetterStack.ReceiveLetter("MFM_LetterDiscount".Translate(Utils.getUSFMLabel()),
            "MFM_LetterDiscountDesc".Translate((int)(discount * 100), code), LetterDefOf.PositiveEvent);

        return true;
    }
}