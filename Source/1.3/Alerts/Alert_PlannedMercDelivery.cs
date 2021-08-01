using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class Alert_PlannedMercDelivery : Alert
    {
        public Alert_PlannedMercDelivery()
        {
            this.defaultExplanation = "MFM_AlertPlannedMercDeliveryDesc".Translate();
            this.defaultPriority = AlertPriority.High;
        }

        public override AlertReport GetReport()
        {
            Dictionary<int, string> pending = Utils.GCMFM.getPendingMercOrder();
            int CGT = Find.TickManager.TicksGame;

            if (pending != null && pending.Count > 0)
            {
                defaultLabel = "";
                foreach (var entry in pending)
                {
                    if (defaultLabel != "")
                        defaultLabel += "\n";
                    defaultLabel += "MFM_AlertPlannedMercDelivery".Translate((entry.Key - CGT).ToStringTicksToPeriodVerbose());
                }
                return true;
            }
            else
                return false;
        }
    }
}
