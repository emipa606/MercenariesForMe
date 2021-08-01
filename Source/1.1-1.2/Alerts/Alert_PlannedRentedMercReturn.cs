using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class Alert_PlannedRentedMercReturn : Alert
    {
        public Alert_PlannedRentedMercReturn()
        {
            this.defaultExplanation = "MFM_AlertPlannedMercReturnDesc".Translate();
            this.defaultPriority = AlertPriority.High;
        }

        public override AlertReport GetReport()
        {
            Dictionary<int, string> pending = Utils.GCMFM.getPendingRendMercComeBack();
            int CGT = Find.TickManager.TicksGame;

            if (pending != null && pending.Count > 0)
            {
                defaultLabel = "";
                foreach (var entry in pending)
                {
                    if (defaultLabel != "")
                        defaultLabel += "\n";
                    defaultLabel += "MFM_AlertPlannedMercReturn".Translate((entry.Key - CGT).ToStringTicksToPeriodVerbose());
                }
                return true;
            }
            else
                return false;
        }
    }
}
