using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class Alert_PlannedMedievalDeliveryCaravan : Alert
{
    public Alert_PlannedMedievalDeliveryCaravan()
    {
        defaultExplanation = "MFM_AlertPlannedMedievalCaravanDeliveryDesc".Translate();
        defaultPriority = AlertPriority.High;
    }

    public override AlertReport GetReport()
    {
        var pending = Utils.GCMFM.getPendingMedievalDelivery();
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
                "MFM_AlertPlannedMedievalCaravanDelivery".Translate(
                    (entry.Key - CGT).ToStringTicksToPeriodVerbose());
        }

        return true;
    }
}