using System;
using FondantMetStokjes.InteractionSystem;
using UnityEngine;

namespace FondantMetStokjes.Interactions.Pickups
{
    public class Pickup : Interactable
    {
        [field: SerializeField] public GameObject BaseDisplayObject { get; private set; }
        public event Action<Transform> OnPickup;
        public event Action OnDrop;
        private Interactor currentInteractor;
        public override void OnInteract(Interactor interactor)
        {
            if (interactor.GetComponent<Inventory>().TryPickupItem(this, out Transform display))
            {
                currentInteractor = interactor;
                if(OnPickup!=null) OnPickup(display);
                interactor.PlayPickupAnimation();
                gameObject.SetActive(false);
            }
        }

        public void ForcePickup(Interactor interactor)
        {
            interactor.GetComponent<Inventory>().ForcePickupItem(this, out Transform display);
            currentInteractor = interactor;
            if(OnPickup!=null) OnPickup(display);
            interactor.PlayPickupAnimation();
            gameObject.SetActive(false);
        }

        public void NotifyDropped(Interactor interactor)
        {
            interactor.PlayDropAnimation();
            if(OnDrop != null) OnDrop();
        }
    }
}