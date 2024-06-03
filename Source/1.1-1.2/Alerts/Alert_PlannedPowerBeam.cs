using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class Alert_PlannedPowerBeam : Alert
    {
        public Alert_PlannedPowerBeam()
        {
            this.defaultExplanation = "MFM_AlertPlannedPowerBeamAt".Translate();
            this.defaultPriority = AlertPriority.High;
        }

        public override AlertReport GetReport()
        {
            Dictionary<int, string> pending = Utils.GCMFM.getPendingPowerBeam();
            int CGT = Find.TickManager.TicksGame;
            List<GlobalTargetInfo> ret = null;
            
            if (pending != null && pending.Count > 0)
            {
                defaultLabel = "";
                foreach (var entry in pending)
                {
                    if (defaultLabel != "")
                        defaultLabel += "\n";

                    Map map;
                    IntVec3 pos;

                    Utils.unserializePowerBeamAttack(entry.Value, out map, out pos);
                    if (ret == null)
                        ret = new List<GlobalTargetInfo>();

                    ret.Add(new GlobalTargetInfo(pos, map));
                    int sub = entry.Key - CGT;
                    if (sub < 0)
                        sub = 0;
                    defaultLabel += "MFM_AlertPlannedPowerBeamAt".Translate((sub).ToStringSecondsFromTicks());
                }
                if (ret == null)
                    return true;
                else
                    return AlertReport.CulpritsAre(ret);
            }
            else
                return false;
        }
    }
}
