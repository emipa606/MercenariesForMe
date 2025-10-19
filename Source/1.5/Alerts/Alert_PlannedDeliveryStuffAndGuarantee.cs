using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class Alert_PlannedDeliveryStuffAndGuarantee : Alert
    {
        public Alert_PlannedDeliveryStuffAndGuarantee()
        {
            this.defaultPriority = AlertPriority.High;
        }

        public override AlertReport GetReport()
        {
            this.defaultExplanation = "MFM_AlertPlannedDeliveryStuffAndGuaranteeDesc".Translate(Utils.getUSFMLabel());

            Dictionary<int, string> pending = Utils.GCMFM.getPendingStuffAndGuarantee();
            int CGT = Find.TickManager.TicksGame;

            if (pending != null && pending.Count > 0)
            {
                defaultLabel = "";
                foreach (var entry in pending)
                {
                    if (defaultLabel != "")
                        defaultLabel += "\n";
                    defaultLabel += "MFM_AlertPlannedDeliveryStuffAndGuarantee".Translate((entry.Key - CGT).ToStringTicksToPeriodVerbose());
                }
                return true;
            }
            else
                return false;
        }
    }
}
