using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class Alert_PlannedDeliveryStuffAndGuarantee : Alert
{
    public Alert_PlannedDeliveryStuffAndGuarantee()
    {
        defaultPriority = AlertPriority.High;
    }

    public override AlertReport GetReport()
    {
        defaultExplanation = "MFM_AlertPlannedDeliveryStuffAndGuaranteeDesc".Translate(Utils.getUSFMLabel());

        var pending = Utils.GCMFM.getPendingStuffAndGuarantee();
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
                "MFM_AlertPlannedDeliveryStuffAndGuarantee".Translate(
                    (entry.Key - CGT).ToStringTicksToPeriodVerbose());
        }

        return true;
    }
}