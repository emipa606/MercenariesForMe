using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class Alert_PlannedRentedMercReturn : Alert
{
    public Alert_PlannedRentedMercReturn()
    {
        defaultExplanation = "MFM_AlertPlannedMercReturnDesc".Translate();
        defaultPriority = AlertPriority.High;
    }

    public override AlertReport GetReport()
    {
        var pending = Utils.GCMFM.getPendingRendMercComeBack();
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
                "MFM_AlertPlannedMercReturn".Translate((entry.Key - CGT).ToStringTicksToPeriodVerbose());
        }

        return true;
    }
}