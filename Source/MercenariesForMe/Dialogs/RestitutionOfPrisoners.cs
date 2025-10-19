using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace aRandomKiwi.MFM;

public class RestitutionOfPrisoners : Window
{
    private readonly Pawn actor;
    private readonly Caravan caravan;
    private readonly Map map;
    private readonly List<Pawn> wanted = [];
    public int delivery = 3;

    private Vector2 scrollPosition = Vector2.zero;

    public RestitutionOfPrisoners(Pawn actor, Map map, Caravan caravan)
    {
        this.actor = actor;
        this.caravan = caravan;
        if (map == null)
        {
            if (Utils.modernUSFM())
            {
                map = Utils.getRandomMapOfPlayer();
            }
            else
            {
                map = actor != null ? actor.Map : Utils.getRandomMapOfPlayer();
            }
        }

        this.map = map;
        forcePause = true;
        doCloseX = true;
        absorbInputAroundWindow = true;
        closeOnAccept = false;
        closeOnClickedOutside = true;
    }

    public override Vector2 InitialSize => new(820f, 700f);

    public override void DoWindowContents(Rect inRect)
    {
        List<Pawn> toDel = null;
        inRect.yMin += 15f;
        inRect.yMax -= 15f;
        var defaultColumnWidth = inRect.width - 20;
        var list = new Listing_Standard { ColumnWidth = defaultColumnWidth };


        //Image logo
        Widgets.ButtonImage(new Rect(0, 0, 800, 170),
            Utils.modernUSFM() ? Tex.restitutionOfPrisoners : Tex.medievalRestitutionOfPrisoners, Color.white,
            Color.white);

        var outRect = new Rect(inRect.x, inRect.y + 180, inRect.width, inRect.height - 200);
        var scrollRect = new Rect(0f, 180f, inRect.width - 16f, inRect.height + (60 * wanted.Count) + 50);
        outRect.height -= 60;

        Widgets.BeginScrollView(outRect, ref scrollPosition, scrollRect);
        list.Begin(scrollRect);

        if (list.ButtonText("MFM_AddPrisonerMerc".Translate()))
        {
            showPrisonerMercsList();
        }

        var salary = 0;

        foreach (var entry in wanted)
        {
            var comp = entry.TryGetComp<Comp_USFM>();
            if (comp == null)
            {
                continue;
            }

            var csalary = 0;
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
            var health = entry.health.summaryHealth.SummaryHealthPercent * Utils.getPawnScoreBasic(entry);

            var healthComp = "";
            if (health < 1.0f)
            {
                healthComp = "MFM_HealthImpacted".Translate((int)((1.0f - health) * csalary));
                healthComp = $" {healthComp}";
            }

            list.Label(entry.LabelCap + healthComp);

            salary += (int)(csalary * health);

            GUI.color = Color.red;
            if (list.ButtonText("MFM_RemovePrisonerMerc".Translate()))
            {
                toDel ??= [];

                toDel.Add(entry);
            }

            GUI.color = Color.white;
            list.GapLine();
        }

        if (toDel != null)
        {
            foreach (var p in toDel)
            {
                if (wanted.Contains(p))
                {
                    wanted.Remove(p);
                }
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

        GUI.color = wanted.Count == 0 ? Color.gray : Color.green;

        if (Widgets.ButtonText(new Rect(0f, 630f, 390f, 30f), "OK".Translate()))
        {
            if (wanted.Count != 0)
            {
                //Transfer of prisoners
                foreach (var m in wanted)
                {
                    m.TryGetComp<Comp_USFM>();

                    if (caravan != null)
                    {
                        if (caravan.pawns.Contains(m))
                        {
                            caravan.pawns.Remove(m);
                        }

                        //If the caravan is empty, we delete it
                        if (caravan.pawns.Count == 0)
                        {
                            Find.WorldObjects.Remove(caravan);
                        }
                    }

                    if (m.Spawned)
                    {
                        m.DeSpawn();
                    }
                }

                //Money shipments
                var thing = ThingMaker.MakeThing(ThingDefOf.Silver);
                if (Utils.modernUSFM())
                {
                    thing.stackCount = salary;
                    var dropSpot = DropCellFinder.TradeDropSpot(map);
                    if (!dropSpot.IsValid)
                    {
                        dropSpot = DropCellFinder.FindRaidDropCenterDistant(map);
                    }

                    TradeUtility.SpawnDropPod(dropSpot, map, thing);
                    Find.LetterStack.ReceiveLetter("MFM_LetterImprisonedMercPayment".Translate(Utils.getUSFMLabel()),
                        "MFM_LetterImprisonedMercPaymentDesc".Translate(salary, Utils.getUSFMLabel()),
                        LetterDefOf.PositiveEvent, new LookTargets(dropSpot, map));
                }
                else
                {
                    //Planning caravan delivery
                    var data = new Dictionary<string, int>
                    {
                        ["map"] = map.uniqueID,
                        ["silver"] = salary
                    };

                    Utils.GCMFM.addPendingMedievalCaravan(0, data,
                        "MFM_LetterImprisonedMercPaymentDesc".Translate(salary, Utils.getUSFMLabel()));
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
        var opts = new List<FloatMenuOption>();
        IEnumerable<Pawn> pawnSource;

        if (actor == null && caravan == null)
        {
            return;
        }

        if (actor == null && caravan != null)
        {
            pawnSource = caravan.pawns;
        }
        else
        {
            pawnSource = map.mapPawns.FreeColonistsAndPrisonersSpawned;
        }

        foreach (var pawn in pawnSource.OrderBy(p => p.LabelCap))
        {
            var comp = pawn.TryGetComp<Comp_USFM>();

            if (actor != pawn && !wanted.Contains(pawn) && pawn.Faction != Faction.OfPlayer && comp is
                {
                    isMercenary: true, hiredByPlayer: false
                })
            {
                opts.Add(new FloatMenuOption(pawn.LabelCap, delegate { wanted.Add(pawn); }, MenuOptionPriority.Default,
                    null, null, 29f,
                    rect => Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f), pawn)));
            }
        }

        if (opts.Count == 0)
        {
            return;
        }

        var floatMenuMap = new FloatMenu(opts);
        Find.WindowStack.Add(floatMenuMap);
    }
}