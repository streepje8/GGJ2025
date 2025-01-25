using FondantMetStokjes.Interactions.Pickups;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventoryItem CurrentlyHolding { get; private set; }
    public bool IsHoldingSomething => CurrentlyHolding != null;
    public bool TryPickupItem(InventoryItem item)
    {
        if (!IsHoldingSomething)
        {
            CurrentlyHolding = item;
            return true;
        }
        return false;
    }
}