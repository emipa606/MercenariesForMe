using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_Discount : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            float discount= Rand.Range(Settings.minDiscount, Settings.maxDiscount);
            string code= Utils.generateDiscountCode((uint)Find.TickManager.TicksGame);

            Utils.GCMFM.addDiscount(code, discount);

            Find.LetterStack.ReceiveLetter("MFM_LetterDiscount".Translate(Utils.getUSFMLabel()), "MFM_LetterDiscountDesc".Translate((int)(discount*100), code), LetterDefOf.PositiveEvent);

            return true;
        }
    }
}
