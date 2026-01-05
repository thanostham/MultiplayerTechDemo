using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }
    [SerializeField] private GameObject mainMenu;

    private void Awake()
    {
        Instance = this;
        ToggleMainMenu(true);
    }

    public void ToggleMainMenu(bool statues)
    {
        mainMenu.SetActive(statues);
    }
}