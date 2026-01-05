using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }
    [SerializeField] private GameObject canvas;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        ToggleCanvas(true);
    }

    public void ToggleCanvas(bool statues)
    {
        canvas.SetActive(statues);
    }
}