using FondantMetStokjes.Interactions.Pickups;
using FondantMetStokjes.InteractionSystem;
using FondantMetStokjes.Player;
using UnityEngine;

[RequireComponent(typeof(InputWrapper))]
public class Inventory : MonoBehaviour
{
    private static readonly int IsHolding = Animator.StringToHash("IsHolding");
    [field: SerializeField] public Transform HoldingPoint { get; private set; }
    public Pickup CurrentlyHolding { get; private set; }
    public bool IsHoldingSomething => CurrentlyHolding != null;
    private GameObject currentDisplay;
    private Ps4Controller input;
    private Animator animator;

    private void Awake()
    {
        input = GetComponent<InputWrapper>().CurrentController;
        animator = GetComponentInChildren<Animator>(true);
    }

    private void Update()
    {
        if (input.GetButtonPressed(ControllerButton.Circle))
        {
            if (IsHoldingSomething) DropItem();
        }
        
        animator.SetFloat(IsHolding, IsHoldingSomething ? 1f : 0f);

        if (IsHoldingSomething)
        {
            currentDisplay.transform.position = HoldingPoint.transform.position;
            currentDisplay.transform.rotation = HoldingPoint.transform.rotation;
        }
    }

    public Pickup DropItem()
    {
        var pickup = CurrentlyHolding;
        CurrentlyHolding.transform.position = HoldingPoint.position;
        CurrentlyHolding.gameObject.SetActive(true);
        CurrentlyHolding.NotifyDropped(GetComponent<Interactor>());
        Destroy(currentDisplay);
        currentDisplay = null;
        CurrentlyHolding = null;
        return pickup;
    }

    public bool TryPickupItem(Pickup pickup, out Transform display)
    {
        if (!IsHoldingSomething)
        {
            CurrentlyHolding = pickup;
            Destroy(currentDisplay);
            currentDisplay = null;
            currentDisplay = Instantiate(pickup.BaseDisplayObject);
            display = currentDisplay.transform;
            currentDisplay.transform.localPosition = Vector3.zero;
            currentDisplay.transform.localRotation = Quaternion.identity;
            return true;
        }
        display = null;
        return false;
    }

    public Pickup TakeItem() //Takes the item without activating it
    {
        var pickup = CurrentlyHolding;
        CurrentlyHolding.transform.position = HoldingPoint.position;
        CurrentlyHolding.NotifyDropped(GetComponent<Interactor>());
        Destroy(currentDisplay);
        currentDisplay = null;
        CurrentlyHolding = null;
        return pickup;
    }

    public void ForcePickupItem(Pickup pickup, out Transform display)
    {
        if (IsHoldingSomething)
        {
            DropItem();
        }

        CurrentlyHolding = pickup;
        Destroy(currentDisplay);
        currentDisplay = null;
        currentDisplay = Instantiate(pickup.BaseDisplayObject);
        display = currentDisplay.transform;
        currentDisplay.transform.localPosition = Vector3.zero;
        currentDisplay.transform.localRotation = Quaternion.identity;
    }
}