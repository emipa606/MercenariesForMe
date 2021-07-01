using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;


namespace aRandomKiwi.MFM
{
    public class Designator_BeamTarget : Designator_Zone
    {
        public Designator_BeamTarget()
        {
            this.soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
            this.soundDragChanged = null;
            this.soundSucceeded = SoundDefOf.Designate_ZoneDelete;
            this.useMouseIcon = true;

            this.icon = Tex.beamTarget;
            this.hotKey = KeyBindingDefOf.Misc4;
        }

        public override void SelectedUpdate()
        {
            base.SelectedUpdate();
            this.drawCircle(UI.MouseCell());
        }

        private void drawCircle(IntVec3 pos)
        {
            GenDraw.DrawRadiusRing(pos, this.radius);
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 sq)
        {
            if (!sq.InBounds(base.Map))
            {
                return false;
            }

            return true;
        }

        public override int DraggableDimensions
        {
            get
            {
                return 0;
            }
        }

        public override bool DragDrawMeasurements
        {
            get
            {
                return false;
            }
        }

        public override void DesignateMultiCell(IEnumerable<IntVec3> cells)
        {
            throw new NotImplementedException();
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            this.pos = c;
            this.cmap = Current.Game.CurrentMap;
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            Find.DesignatorManager.Deselect();

            Find.WindowStack.Add(new Dialog_Msg("MFM_DialogConfirmPowerBeam".Translate(), "MFM_DialogConfirmPowerBeamDesc".Translate(Settings.powerBeamCost), delegate
            {
                Map destMap = null;
                foreach (var map in Find.Maps)
                {
                    if (map.IsPlayerHome && TradeUtility.ColonyHasEnoughSilver(map, Settings.powerBeamCost))
                    {
                        destMap = map;
                        break;
                    }
                }

                if(destMap == null)
                {
                    Messages.Message("MFM_MsgNotEnoughtSilverPowerBeam".Translate(), MessageTypeDefOf.NegativeEvent);
                    return;
                }

                TradeUtility.LaunchSilver(destMap, Settings.powerBeamCost);

                //Here what we want to do
                Utils.GCMFM.addPendingPowerBeam(Find.TickManager.TicksGame + 660, cmap, pos);
            },false));
        }


        private IntVec3 pos;
        private Map cmap;
        private int radius = 12;
    }
}

