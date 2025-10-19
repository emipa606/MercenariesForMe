using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class Designator_BeamTarget : Designator_Zone
{
    private readonly int radius = 12;
    private Map cmap;


    private IntVec3 pos;

    public Designator_BeamTarget()
    {
        soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
        soundDragChanged = null;
        soundSucceeded = SoundDefOf.Designate_ZoneDelete;
        useMouseIcon = true;

        icon = Tex.beamTarget;
        hotKey = KeyBindingDefOf.Misc4;
    }


    public override bool DragDrawMeasurements => false;

    public override void SelectedUpdate()
    {
        base.SelectedUpdate();
        drawCircle(UI.MouseCell());
    }

    private void drawCircle(IntVec3 position)
    {
        GenDraw.DrawRadiusRing(position, radius);
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 sq)
    {
        return sq.InBounds(Map);
    }

    public override void DesignateMultiCell(IEnumerable<IntVec3> cells)
    {
        throw new NotImplementedException();
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
        pos = c;
        cmap = Current.Game.CurrentMap;
    }

    protected override void FinalizeDesignationSucceeded()
    {
        base.FinalizeDesignationSucceeded();
        Find.DesignatorManager.Deselect();

        Find.WindowStack.Add(new Dialog_Msg("MFM_DialogConfirmPowerBeam".Translate(),
            "MFM_DialogConfirmPowerBeamDesc".Translate(Settings.powerBeamCost), delegate
            {
                Map destMap = null;
                foreach (var map in Find.Maps)
                {
                    if (!map.IsPlayerHome || !TradeUtility.ColonyHasEnoughSilver(map, Settings.powerBeamCost))
                    {
                        continue;
                    }

                    destMap = map;
                    break;
                }

                if (destMap == null)
                {
                    Messages.Message("MFM_MsgNotEnoughtSilverPowerBeam".Translate(), MessageTypeDefOf.NegativeEvent);
                    return;
                }

                TradeUtility.LaunchSilver(destMap, Settings.powerBeamCost);

                //Here what we want to do
                Utils.GCMFM.addPendingPowerBeam(Find.TickManager.TicksGame + 660, cmap, pos);
            }));
    }
}