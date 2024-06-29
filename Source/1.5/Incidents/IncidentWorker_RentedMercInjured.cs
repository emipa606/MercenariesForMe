using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace aRandomKiwi.MFM
{
    public class IncidentWorker_RentedMercInjured : IncidentWorker
    {
        private static List<string> vitalBodyParts = new List<string> { "Neck", "Skull", "Brain","Head","Torso","Heart","Stomach"};

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Utils.GCMFM.playerHaveRentedMerc();
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!Utils.GCMFM.playerHaveRentedMerc())
                return false;

            //Random selection of a mercenary
            Pawn merc = Utils.GCMFM.getRandomRentedMerc();
            Utils.GCMFM.popRentedMercenary(merc);
            Utils.GCMFM.popRentedPawn(merc);

            List<Pawn> toDeliver = new List<Pawn>();
            BodyPartRecord bpr = null;

            try
            {
                List<string> tmp = null;
                //If liver or kidney is already missing, we add them to the temporary list
                List<Hediff_MissingPart> lst = new List<Hediff_MissingPart>();
                merc.health.hediffSet.GetHediffs<Hediff_MissingPart>(ref lst);
                foreach (var entry in lst)
                {
                    if (entry.Part.def.defName == "Kidney" || entry.Part.def.defName == "Lung")
                    {
                        if (tmp == null)
                        {
                            tmp = vitalBodyParts.ToList();
                        }
                        tmp.Add(entry.Part.def.defName);
                    }
                }

                if (tmp == null)
                    tmp = vitalBodyParts;

                /*foreach(var el in tmp)
                {
                    //Log.Message(el);
                }*/


                bool ok = false;
                int i = 0;
                while (!ok && i != 350)
                {
                    bpr = merc.health.hediffSet.GetRandomNotMissingPart(DamageDefOf.Stab);
                    //Log.Message("=>"+bpr.def.defName);
                    if (!tmp.Contains(bpr.def.defName))
                        ok = true;
                    i++;
                }

                if (i == 300)
                    return false;

                Hediff h = merc.health.AddHediff(HediffDefOf.MissingBodyPart, bpr, null, null);

                merc.SetFactionDirect(Faction.OfPlayer);
                toDeliver.Add(merc);

                Comp_USFM comp = merc.TryGetComp<Comp_USFM>();
                comp.prepareBringBackRentedMerc();
                comp.rentedMercAdvanceBioAge();

            }
            catch(Exception)
            {
                //If error reintegration of the spawning mercenary
                if (merc != null)
                {
                    Utils.GCMFM.pushRentedMercenary(merc);
                    Utils.GCMFM.pushRentedPawn(merc);
                }
                return false;
            }

            //Mercenary return via droppod
            Map map = Utils.getRandomMapOfPlayer();

            //Patch avec SOS2: Si c'est une map spatiale SOS2 / Rimnauts 2, on prendra une autre --Par Ionfrigate12345:
            if (HarmonyUtils.IsSOS2OrRimNauts2SpaceMap(map))
            {
                map = HarmonyUtils.GetPlayerMainColonyMapSOS2Excluded();
            }
            if (map == null) //Si on trouve toujours pas (le joueur n'a pas de map planétaire)
            {
                Log.Warning("Cannot find a map to spawn mercenaries. The player seems to have only SOS2 space maps where the spawning is prevented.");
                return false;
            }

            IntVec3 dropCellNear = Utils.spawnMercOnMap(map, toDeliver);
            if (dropCellNear.IsValid)
                Find.LetterStack.ReceiveLetter("MFM_LetterRentedMercInjured".Translate(), "MFM_LetterRentedMercInjuredDesc".Translate(merc.Label, bpr.LabelCap), LetterDefOf.NegativeEvent, new LookTargets(dropCellNear, map));

            return true;
        }
    }
}
