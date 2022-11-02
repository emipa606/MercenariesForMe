using System;
using Verse;
using RimWorld;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class RestitutionOfPrisoners : Window
    {
        public List<Pawn> wanted = new List<Pawn>();

        public Vector2 scrollPosition = Vector2.zero;
        public Map map;
        public Pawn actor;
        public Caravan caravan;
        public ITrader trader;
        public int delivery=3;
        public Map backMap = null;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(820f, 700f);
            }
        }

        public RestitutionOfPrisoners(Pawn actor,Map map, Caravan caravan, ITrader trader = null)
        {
            this.trader = trader;
            this.actor = actor;
            this.caravan = caravan;
            if (map == null)
            {
                if(Utils.modernUSFM())
                    map = Utils.getRandomMapOfPlayer();
                else{
                    if (actor != null)
                        map = actor.Map;
                    else
                        map = Utils.getRandomMapOfPlayer();
                }
                
            }
            this.backMap = Find.CurrentMap;
            this.map = map;
            this.forcePause = true;
            this.doCloseX = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
            this.closeOnClickedOutside = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            List<Pawn> toDel = null;
            inRect.yMin += 15f;
            inRect.yMax -= 15f;
            var defaultColumnWidth = (inRect.width - 20);
            Listing_Standard list = new Listing_Standard() { ColumnWidth = defaultColumnWidth };


            //Image logo
            if (Utils.modernUSFM())
                Widgets.ButtonImage(new Rect(0, 0, 800, 170), Tex.restitutionOfPrisoners, Color.white, Color.white);
            else
                Widgets.ButtonImage(new Rect(0, 0, 800, 170), Tex.medievalRestitutionOfPrisoners, Color.white, Color.white);

            var outRect = new Rect(inRect.x, inRect.y + 180, inRect.width, inRect.height - 200);
            var scrollRect = new Rect(0f, 180f, inRect.width - 16f, inRect.height + (60*wanted.Count) + 50);
            outRect.height -= 60;

            Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect, true);
            list.Begin(scrollRect);

            if (list.ButtonText("MFM_AddPrisonerMerc".Translate()))
            {
                showPrisonerMercsList();
            }

            int salary = 0;

            foreach(var entry in wanted)
            {
                Comp_USFM comp = entry.TryGetComp<Comp_USFM>();
                if (comp == null)
                    continue;

                int csalary = 0;
                switch (Utils.getLevelFromSkill(entry, comp.type))
                {
                    case MercenaryLevel.Recruit:
                        csalary = (int)(Settings.priceRecruit * 0.5f);
                        break;
                    case MercenaryLevel.Confirmed:
                        csalary = (int)(Settings.priceConfirmed * 0.5f);
                        break;
                    case MercenaryLevel.Veteran:
                        csalary = (int)(Settings.priceVeteran * 0.5f);
                        break;
                    case MercenaryLevel.Elite:
                        csalary = (int)(Settings.priceElite * 0.5f);
                        break;
                    case MercenaryLevel.Cyborg:
                        csalary = (int)(Settings.priceCyborg * 0.5f);
                        break;
                }

                //Calculation of percentage degradation of the salary according to the state of health of the mercenary
                float health = entry.health.summaryHealth.SummaryHealthPercent * Utils.getPawnScoreBasic(entry);

                string healthComp = "";
                if(health < 1.0f)
                {
                    healthComp = "MFM_HealthImpacted".Translate(  (int)((1.0f-health)*csalary) );
                    healthComp = " " + healthComp;
                }

                list.Label(entry.LabelCap+healthComp);

                salary += (int)(csalary * health);

                GUI.color = Color.red;
                if (list.ButtonText("MFM_RemovePrisonerMerc".Translate()))
                {
                    if (toDel == null)
                        toDel = new List<Pawn>();
                    toDel.Add(entry);
                }
                GUI.color = Color.white;
                list.GapLine();
            }

            if(toDel != null)
            {
                foreach(var p in toDel)
                {
                    if (wanted.Contains(p))
                        wanted.Remove(p);
                }
            }

            list.End();
            Widgets.EndScrollView();

            
            if (wanted.Count != 0)
            {
                //Calculation of monthly salaries
                GUI.color = Color.green;
                Widgets.Label(new Rect(0f, 600f, 800f, 30f), "MFM_QuadBillIncome".Translate(salary));
                GUI.color = Color.white;
            }

            if ( (wanted.Count == 0))
                GUI.color = Color.gray;
            else
                GUI.color = Color.green;
            if (Widgets.ButtonText(new Rect(0f, 630f, 390f, 30f), "OK".Translate()))
            {
                if (wanted.Count != 0)
                {
                    //Transfer of prisoners
                    foreach (var m in wanted)
                    {
                        Comp_USFM comp = m.TryGetComp<Comp_USFM>();

                        if(caravan != null)
                        {
                            if(caravan.pawns.Contains(m))
                                caravan.pawns.Remove(m);

                            //If the caravan is empty, we delete it
                            if (caravan.pawns.Count == 0)
                            {
                                Find.WorldObjects.Remove(caravan);
                            }
                        }

                        if(m.Spawned)
                            m.DeSpawn();
                    }

                    //Money shipments
                    Thing thing = ThingMaker.MakeThing(ThingDefOf.Silver);
                    if (Utils.modernUSFM())
                    {
                        thing.stackCount = salary;
                        IntVec3 dropSpot = DropCellFinder.TradeDropSpot(map);
                        if (!dropSpot.IsValid)
                        {
                            dropSpot = DropCellFinder.FindRaidDropCenterDistant(map);
                        }
                        TradeUtility.SpawnDropPod(dropSpot, map, thing);
                        Find.LetterStack.ReceiveLetter("MFM_LetterImprisonedMercPayment".Translate(Utils.getUSFMLabel()), "MFM_LetterImprisonedMercPaymentDesc".Translate(salary, Utils.getUSFMLabel()), LetterDefOf.PositiveEvent, new LookTargets(dropSpot, map));
                    }
                    else
                    {
                        //Planning caravan delivery
                        Dictionary<string, int> data = new Dictionary<string, int>();
                        data["map"] = map.uniqueID;
                        data["silver"] = salary;

                        Utils.GCMFM.addPendingMedievalCaravan(0, data, "MFM_LetterImprisonedMercPaymentDesc".Translate(salary, Utils.getUSFMLabel()));
                    }

                    Find.WindowStack.TryRemove(this);
                }
                else
                {
                        Messages.Message("MFM_MsgNeedSelectMercToContinue".Translate(), MessageTypeDefOf.NegativeEvent);
                }
            }
            GUI.color = Color.red;
            if (Widgets.ButtonText(new Rect(390f, 630f, 390f, 30f), "GoBack".Translate()))
            {
                Find.WindowStack.TryRemove(this);
            }
            GUI.color = Color.white;
        }

        private void showPrisonerMercsList()
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            IEnumerable<Pawn> pawnSource = null;

            if (actor == null && caravan == null)
                return;

            if(actor == null && caravan != null)
            {
                pawnSource = caravan.pawns;
            }
            else
            {
                pawnSource = map.mapPawns.FreeColonistsAndPrisonersSpawned;
            }

            foreach (var pawn in pawnSource.OrderBy(p => p.LabelCap))
            {
                Comp_USFM comp = pawn.TryGetComp<Comp_USFM>();
               
                if (actor != pawn && !wanted.Contains(pawn) && pawn.Faction != Faction.OfPlayer && ( comp != null && comp.isMercenary && !comp.hiredByPlayer))
                {
                    opts.Add(new FloatMenuOption(pawn.LabelCap, delegate
                    {
                        wanted.Add(pawn);
                    }, MenuOptionPriority.Default, null, null, 29f,(Rect rect) => Widgets.InfoCardButton(rect.x + 5f, rect.y + (rect.height - 24f) / 2f, pawn), null));
                }
            }
            if (opts.Count != 0)
            {
                FloatMenu floatMenuMap = new FloatMenu(opts);
                Find.WindowStack.Add(floatMenuMap);
            }
        }
    
    }
}