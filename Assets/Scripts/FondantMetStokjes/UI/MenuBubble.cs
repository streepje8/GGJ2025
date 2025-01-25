using System;
using UnityEngine;

public class MenuBubble : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var option = other.GetComponent<MenuOption>();
        if (option != null) option.SelectOption();
    }
}
