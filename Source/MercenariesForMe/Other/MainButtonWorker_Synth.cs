using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class MainButtonWorker_Synth : MainButtonWorker
{
    public override void Activate()
    {
        var cl = new ChoiceLetter_Bill();
        cl.init();
        Find.WindowStack.Add(new Bill(cl, true));
    }
}