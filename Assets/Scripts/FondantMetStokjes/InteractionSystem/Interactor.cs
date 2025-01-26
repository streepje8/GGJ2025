using System.Collections.Generic;
using FondantMetStokjes.Interactions.Pickups;
using FondantMetStokjes.Player;
using UnityEngine;

namespace FondantMetStokjes.InteractionSystem
{
    [RequireComponent(typeof(InputWrapper))]
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(Inventory))]
    public class Interactor : MonoBehaviour
    {
        private static readonly int Pickup = Animator.StringToHash("Pickup");
        private static readonly int Drop = Animator.StringToHash("Drop");
        private static readonly int Interacting = Animator.StringToHash("Interacting");

        [field: Header("Interactor Settings")]
        [field: SerializeField] public bool CanInteract { get; set; } = true;
        [field: SerializeField] public float MaxInteractionRange { get; set; } = 30f;
        [field: SerializeField] public LayerMask InteractableLayer { get; set; }
        public Ps4Controller Controller => input;
    
        private Ps4Controller input;
        private Animator animator;
        private PlayerController playerController;
        private Inventory inventory;
        private void Awake()
        {
            input = GetComponent<InputWrapper>().CurrentController;
            animator = GetComponentInChildren<Animator>(true);
            playerController = GetComponent<PlayerController>();
            inventory = GetComponent<Inventory>();
        }
        
        private Dictionary<Collider, Interactable> interactableCache = new Dictionary<Collider, Interactable>();
        private Collider[] colliders = new Collider[10];
        private void Update()
        {
            int interactablesInRange = Physics.OverlapSphereNonAlloc(transform.position, MaxInteractionRange, colliders, InteractableLayer);
            if (interactablesInRange > 0)
            {
                for (int i = 0; i < interactablesInRange; i++)
                {
                    var collider = colliders[i];
                    if (!interactableCache.TryGetValue(collider, out Interactable interactable))
                    {
                        interactable = colliders[i].GetComponent<Interactable>();
                        interactableCache[collider] = interactable;
                    }

                    if (interactable == null)
                    {
                        Debug.LogWarning($"Object {collider.gameObject.name} was found as an interactable but it had no interactable component...");
                        continue;
                    }
                
                    if (interactable.IsInteractable && !interactable.InInteraction)
                    {
                        var dst = Vector3.Distance(interactable.transform.position, transform.position);
                        if (dst < interactable.Range)
                        {
                            interactable.SetInRange();
                            if(input.GetButtonPressed(interactable.InteractionStartButton))interactable.OnInteract(this);
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, MaxInteractionRange);
        }

        public bool TryTakeBubble(out LiveBubble result)
        {
            result = null;
            if (!inventory.IsHoldingSomething) return false;
            result = inventory.TakeItem().GetComponent<LiveBubble>();
            return true;
        }

        public void GiveBubble(LiveBubble bubble)
        {
            bubble.GetComponent<Pickup>().ForcePickup(this);
        }
        

        public void PlayPickupAnimation()
        {
            animator.SetTrigger(Pickup);
        }
        
        public void PlayDropAnimation()
        {
            animator.SetTrigger(Drop);
        }

        public void SetInteractionAnimation(bool interacting)
        {
            playerController.Active = !interacting;
            animator.SetBool(Interacting, interacting);
        }

        public Inventory GetInventory()
        {
            return inventory;
        }
    }
}
