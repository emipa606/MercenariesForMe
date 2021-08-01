using RimWorld.Planet;
using System;
using Verse;
using Verse.Sound;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class MainButtonWorker_Synth : MainButtonWorker
    {
        public override void Activate()
        {
            ChoiceLetter_Bill cl = new ChoiceLetter_Bill();
            cl.init();
            Find.WindowStack.Add(new Bill(cl, true));
        }
    }
}