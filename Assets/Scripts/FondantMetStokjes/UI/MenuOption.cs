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
                GameManager.Instance.SwitchToGame();
                GameManager.Instance.NewGame();
                GameManager.Instance.StartGame();
                break;
            case MenuOptions.Credits:
                GameObject.Find("Balzak").transform.GetChild(0).gameObject.SetActive(true);
                break;
            case MenuOptions.Quit:
                Application.Quit();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
