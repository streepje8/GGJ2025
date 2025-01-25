using UnityEngine;

namespace FondantMetStokjes.Interactions.Pickups
{
    [CreateAssetMenu(menuName = "Gnomez/InventoryItem", fileName = "New InventoryItem", order = 0)]
    public class InventoryItem : ScriptableObject
    {
        [field: SerializeField] public GameObject HeldItem;
        [field: SerializeField] public GameObject DroppedItem;
    }
}