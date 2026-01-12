using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameUiController : MonoBehaviour
{
    public static GameUiController Instance { get; set; }
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

    public void returnToMenu()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadBoots()
    {
        SceneManager.LoadScene(0);
    }
}