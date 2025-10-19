using RimWorld;
using Verse;

namespace aRandomKiwi.MFM;

public class IncidentWorker_RentedMercCounterOffer : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return !Utils.GCMFM.CounterOfferInProgress && GC_MFM.playerHaveMerc() &&
               Utils.getPlayerMercenaries().Count >= 1 && Settings.enableIncidentCounterOffer;
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        if (Utils.GCMFM.CounterOfferInProgress || !GC_MFM.playerHaveMerc() ||
            Utils.getPlayerMercenaries().Count < 1 || !Settings.enableIncidentCounterOffer)
        {
            return false;
        }

        var price = 0;

        var faction =
            //Enemy faction deduction at the origin of the counteroffer
            Find.FactionManager.RandomEnemyFaction();

        //Rogue Mercenaries Deduction
        var rogueMercs = GC_MFM.getRandomRogueMerc();

        //Deduction amount
        foreach (var merc in rogueMercs)
        {
            var comp = merc.TryGetComp<Comp_USFM>();
            price += (int)(comp.salary * Rand.Range(Settings.minRateIncSalaryIncidentCounterOffer,
                Settings.maxRateIncSalaryIncidentCounterOffer));
        }

        //If in middle age mode creation site of payment
        if (!Utils.modernUSFM() && !Utils.anySiteOfPayment())
        {
            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome && !HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
                {
                    Utils.createSiteOfPayment(map);
                }
            }
        }

        var cl = (ChoiceLetter_CounterOffer)LetterMaker.MakeLetter(
            DefDatabase<LetterDef>.GetNamed("MFM_CLCounterOffer"));
        cl.Label = "MFM_ChoiceLetterCounterOfferTitle".Translate();
        cl.Text = "MFM_ChoiceLetterCounterOffer".Translate(faction.Name, price, 3);
        cl.radioMode = true;
        cl.rogueMercs = rogueMercs;
        cl.faction = faction;
        cl.price = 0;
        cl.StartTimeout(!Utils.modernUSFM() ? 30000 : 7500);

        Utils.GCMFM.CounterOfferInProgress = true;

        Find.LetterStack.ReceiveLetter(cl);
        return true;
    }
}