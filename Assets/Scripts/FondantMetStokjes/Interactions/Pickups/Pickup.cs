using FondantMetStokjes.InteractionSystem;
using UnityEngine;

namespace FondantMetStokjes.Interactions.Pickups
{
    public class Pickup : Interactable
    {
        [field: Header("Pickup Settings")]
        [field: SerializeField] public InventoryItem PickupItem { get; private set; }
    
        public override void OnInteract(Interactor interactor)
        {
            if (interactor.GetComponent<Inventory>().TryPickupItem(PickupItem))
            {
                Destroy(gameObject);
            }
        }
    }
}