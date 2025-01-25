using System;
using UnityEngine;

public enum MenuOptions 
{
    Play,
    Credits,
    Quit
}

public class MenuOption : MonoBehaviour
{
    [field: SerializeField]public MenuOptions Option { get; private set; }

    public void SelectOption()
    {
        switch (Option)
        {
            case MenuOptions.Play:
                GameManager.Instance.NewGame();
                GameManager.Instance.StartGame();
                break;
            case MenuOptions.Credits:
                Debug.Log("Credits!!");
                break;
            case MenuOptions.Quit:
                Application.Quit();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
