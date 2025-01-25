using FondantMetStokjes.InteractionSystem;
using UnityEngine;
using UnityEngine.Events;

namespace FondantMetStokjes.Interactions.Pickups
{
    public class Pickup : Interactable
    {
        [field: SerializeField] public GameObject DisplayObject { get; private set; }
        [field: SerializeField] public UnityEvent<Transform> OnPickup { get; private set; }
        [field: SerializeField] public UnityEvent OnDrop { get; private set; }
        public override void OnInteract(Interactor interactor)
        {
            if (interactor.GetComponent<Inventory>().TryPickupItem(this, out Transform display))
            {
                OnPickup.Invoke(display);
                interactor.PlayPickupAnimation();
                gameObject.SetActive(false);
            }
        }

        public bool ForcePickup(Interactor interactor)
        {
            if (interactor.GetComponent<Inventory>().TryPickupItem(this, out Transform display))
            {
                OnPickup.Invoke(display);
                interactor.PlayPickupAnimation();
                gameObject.SetActive(false);
                return true;
            }
            return false;
        }

        public void NotifyDropped(Interactor interactor)
        {
            interactor.PlayDropAnimation();
            OnDrop.Invoke();
        }
    }
}