using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class Alert_PlannedMercDelivery : Alert
{
    public Alert_PlannedMercDelivery()
    {
        defaultExplanation = "MFM_AlertPlannedMercDeliveryDesc".Translate();
        defaultPriority = AlertPriority.High;
    }

    public override AlertReport GetReport()
    {
        var pending = Utils.GCMFM.getPendingMercOrder();
        var CGT = Find.TickManager.TicksGame;

        if (pending is not { Count: > 0 })
        {
            return false;
        }

        defaultLabel = "";
        foreach (var entry in pending)
        {
            if (defaultLabel != "")
            {
                defaultLabel += "\n";
            }

            defaultLabel +=
                "MFM_AlertPlannedMercDelivery".Translate((entry.Key - CGT).ToStringTicksToPeriodVerbose());
        }

        return true;
    }
}