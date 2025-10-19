using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_MercWantJoin : IncidentWorker
    {

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return getRandomMercFitConds() != null;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Pawn cmerc = getRandomMercFitConds();
            if (cmerc == null)
                return false;

            //If in middle age mode creation site of payment
            if (!Utils.modernUSFM() && !Utils.anySiteOfPayment())
            {
                foreach (var map in Find.Maps)
                {
                    if (map.IsPlayerHome && !HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
                        Utils.createSiteOfPayment(map);
                }
            }

            ChoiceLetter_MercWantJoin cl = (ChoiceLetter_MercWantJoin)LetterMaker.MakeLetter(DefDatabase<LetterDef>.GetNamed("MFM_CLMercWantJoin"));
            cl.Label = "MFM_ChoiceLetterMercWantJoinTitle".Translate();
            cl.Text = "MFM_ChoiceLetterMercWantJoin".Translate(cmerc.LabelCap, Utils.getUSFMLabel(), Settings.mercJoinNbSalaryToPay, cmerc.TryGetComp<Comp_USFM>().salary, cmerc.TryGetComp<Comp_USFM>().salary * Settings.mercJoinNbSalaryToPay);
            cl.radioMode = true;
            cl.merc = cmerc;
            if(Utils.modernUSFM())
                cl.StartTimeout(15000);
            else
                cl.StartTimeout(60000);
            Find.LetterStack.ReceiveLetter(cl, null);

            Utils.GCMFM.MercWantJoinInProgress = true;

            return true;
        }

        private Pawn getRandomMercFitConds()
        {
            List<Pawn> mercs = Utils.getPlayerMercenaries();

            if (mercs == null || mercs.Count == 0)
                return null;

            IEnumerable<Pawn> list = mercs.
                Where(merc => merc.TryGetComp<Comp_USFM>().nbQuad > 0 && ((merc.TryGetComp<Comp_USFM>().nbQuadHappy > merc.TryGetComp<Comp_USFM>().nbQuadUnHappy) || ( merc.TryGetComp<Comp_USFM>().nbQuadUnHappy == 0 && merc.TryGetComp<Comp_USFM>().nbQuadHappy == 0)));

            if (list == null || list.Count() == 0)
                return null;
            else
                return list.RandomElement();
        }
    }
}
