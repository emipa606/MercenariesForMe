using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace aRandomKiwi.MFM
{
    public class ChoiceLetter_Bill : ChoiceLetter_UnRemovable
    {
        public string discount = "";
        public int showMode = 0;
        public int delivery = 3;
        public ChoiceLetter_Bill bill;
        public Dictionary<Pawn, bool> wanted = new Dictionary<Pawn, bool>();
        public Dictionary<Pawn, bool> rented = new Dictionary<Pawn, bool>();
        public Dictionary<int,bool> pendingMercOrders = new Dictionary<int, bool>();

        private List<Pawn> wantedKeys = new List<Pawn>();
        private List<bool> wantedValues = new List<bool>();
        private List<Pawn> rentedKeys = new List<Pawn>();
        private List<bool> rentedValues = new List<bool>();

        public ChoiceLetter_Bill()
        {
        }

        public override void Removed()
        {
            base.Removed();

            removeSOP();
        }

        private void removeSOP()
        {
            Utils.GCMFM.BillInProgress = false;
            Utils.clearAllMedievalSiteOfPayment();
        }

        public void init()
        {
            //We freeze at 1 a 00h the list of mercenaries to renew (allows the passage to avoid the rebilling pb of mercenary buying in the current time of the choiceletter)
            //and we revalue their pay
            foreach (var m in Utils.getPlayerMercenaries())
            {
                if (m == null)
                    continue;

                wanted[m] = true;

                Utils.setHiredMercSalary(m);
            }

            //Ditto for rented mercs, we will also reassess their pay and their experience
            foreach (var m in Utils.GCMFM.getRentedMercenaries())
            {
                if (m == null)
                    continue;

                //If already in the process of returning
                if (Utils.GCMFM.mercIsInPendingRentedMercComeBack(m.GetUniqueLoadID()))
                    continue;

                rented[m] = true;
                //Merc salary reassessment
                Utils.setRentedMercSalary(m);
            }

            foreach (var entry in Utils.GCMFM.getPendingMercOrder())
            {
                pendingMercOrders[entry.Key] = true;
            }
        }

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                if (base.ArchivedOnly)
                {
                    yield return base.Option_Close;
                }
            }
        }

        public override bool CanShowInLetterStack
        {
            get
            {
                return base.CanShowInLetterStack;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref this.pendingMercOrders, "pendingMercOrders", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref this.wanted, "wanted", LookMode.Reference, LookMode.Value, ref wantedKeys, ref wantedValues);
            Scribe_Collections.Look(ref this.rented, "rented", LookMode.Reference, LookMode.Value, ref rentedKeys, ref rentedValues);
            Scribe_Values.Look<int>(ref showMode, "showMode", 0);
            Scribe_Values.Look<int>(ref delivery, "delivery", 3);
            Scribe_Values.Look<string>(ref discount, "discount", "");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (pendingMercOrders == null)
                    pendingMercOrders = new Dictionary<int, bool>();
                if (wanted == null)
                    wanted = new Dictionary<Pawn, bool>();
                if (rented == null)
                    rented = new Dictionary<Pawn, bool>();
            }
        }

        public override void OpenLetter()
        {
            //if (!base.ArchivedOnly)
                Find.WindowStack.Add(new Bill(this));
        }
    }
}