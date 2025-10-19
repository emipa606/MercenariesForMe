using System.Collections.Generic;
using Verse;

namespace aRandomKiwi.MFM;

public class ChoiceLetter_Bill : ChoiceLetter_UnRemovable
{
    public ChoiceLetter_Bill bill;
    public int delivery = 3;
    public string discount = "";
    public Dictionary<int, bool> pendingMercOrders = new();
    public Dictionary<Pawn, bool> rented = new();
    private List<Pawn> rentedKeys = [];
    private List<bool> rentedValues = [];
    public int showMode;
    public Dictionary<Pawn, bool> wanted = new();

    private List<Pawn> wantedKeys = [];
    private List<bool> wantedValues = [];

    public override IEnumerable<DiaOption> Choices
    {
        get
        {
            if (ArchivedOnly)
            {
                yield return Option_Close;
            }
        }
    }

    public override void Removed()
    {
        base.Removed();

        removeSOP();
    }

    private static void removeSOP()
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
            {
                continue;
            }

            wanted[m] = true;

            Utils.setHiredMercSalary(m);
        }

        //Ditto for rented mercs, we will also reassess their pay and their experience
        foreach (var m in Utils.GCMFM.getRentedMercenaries())
        {
            if (m == null)
            {
                continue;
            }

            //If already in the process of returning
            if (Utils.GCMFM.mercIsInPendingRentedMercComeBack(m.GetUniqueLoadID()))
            {
                continue;
            }

            rented[m] = true;
            //Merc salary reassessment
            Utils.setRentedMercSalary(m);
        }

        foreach (var entry in Utils.GCMFM.getPendingMercOrder())
        {
            pendingMercOrders[entry.Key] = true;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Collections.Look(ref pendingMercOrders, "pendingMercOrders", LookMode.Value, LookMode.Value);
        Scribe_Collections.Look(ref wanted, "wanted", LookMode.Reference, LookMode.Value, ref wantedKeys,
            ref wantedValues);
        Scribe_Collections.Look(ref rented, "rented", LookMode.Reference, LookMode.Value, ref rentedKeys,
            ref rentedValues);
        Scribe_Values.Look(ref showMode, "showMode");
        Scribe_Values.Look(ref delivery, "delivery", 3);
        Scribe_Values.Look(ref discount, "discount", "");

        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        pendingMercOrders ??= new Dictionary<int, bool>();

        wanted ??= new Dictionary<Pawn, bool>();

        rented ??= new Dictionary<Pawn, bool>();
    }

    public override void OpenLetter()
    {
        //if (!base.ArchivedOnly)
        Find.WindowStack.Add(new Bill(this));
    }
}