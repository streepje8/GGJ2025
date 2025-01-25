using System;
using FondantMetStokjes.Interactions.Pickups;
using FondantMetStokjes.Player;
using UnityEngine;

[RequireComponent(typeof(InputWrapper))]
public class Inventory : MonoBehaviour
{
    private static readonly int IsHolding = Animator.StringToHash("IsHolding");
    [field: SerializeField] public Transform HoldingPoint { get; private set; }
    public InventoryItem CurrentlyHolding { get; private set; }
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

    private void DropItem()
    {
        Instantiate(CurrentlyHolding.DroppedItem, HoldingPoint.position, Quaternion.identity);
        Destroy(currentDisplay);
        currentDisplay = null;
        CurrentlyHolding = null;
    }

    public bool TryPickupItem(InventoryItem item)
    {
        if (!IsHoldingSomething)
        {
            CurrentlyHolding = item;
            currentDisplay = Instantiate(item.HeldItem);
            currentDisplay.transform.localPosition = Vector3.zero;
            currentDisplay.transform.localRotation = Quaternion.identity;
            return true;
        }
        return false;
    }
}