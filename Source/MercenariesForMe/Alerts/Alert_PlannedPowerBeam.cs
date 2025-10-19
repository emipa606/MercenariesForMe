using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace aRandomKiwi.MFM;

public class Alert_PlannedPowerBeam : Alert
{
    public Alert_PlannedPowerBeam()
    {
        defaultExplanation = "MFM_AlertPlannedPowerBeamAt".Translate();
        defaultPriority = AlertPriority.High;
    }

    public override AlertReport GetReport()
    {
        var pending = Utils.GCMFM.getPendingPowerBeam();
        var CGT = Find.TickManager.TicksGame;
        List<GlobalTargetInfo> ret = null;

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

            Utils.unserializePowerBeamAttack(entry.Value, out var map, out var pos);
            ret ??= [];

            ret.Add(new GlobalTargetInfo(pos, map));
            var sub = entry.Key - CGT;
            if (sub < 0)
            {
                sub = 0;
            }

            defaultLabel += "MFM_AlertPlannedPowerBeamAt".Translate(sub.ToStringSecondsFromTicks());
        }

        return ret == null ? true : AlertReport.CulpritsAre(ret);
    }
}