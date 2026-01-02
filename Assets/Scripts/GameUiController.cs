using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;

    private void Awake()
    {
        ToggleMainMenu(true);
    }

    public void ToggleMainMenu(bool statues)
    {
        mainMenu.SetActive(statues);
    }
}