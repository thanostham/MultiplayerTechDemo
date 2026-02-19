using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameViewManager;

public class MainGameView : View
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text boostText;

    [SerializeField] private TMP_Text winnerText;


    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        if (winnerText)
        {
            winnerText.gameObject.SetActive(false);
        }
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1.buildIndex == 1) winnerText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<MainGameView>();

        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {

    }

    public void UpdateHealth(int health)
    {
        healthText.text = $"Health: " + health.ToString() + " / 100";
    }

    public void UpdateBoostStatus()
    {
        if (MovementLogic.foodCounter >= MovementLogic.requiredAmount)
        {
            boostText.text = ($"Ready: {MovementLogic.foodCounter}");
            boostText.color = Color.green;
        }
        else
        {
            boostText.text = ($"Req 50: {MovementLogic.foodCounter}");
            boostText.color = Color.red;
        }
    }

    public void ShowWinner(string playerName)
    {
        if (winnerText != null)
        {
            winnerText.text = $"GAME OVER!\nPlayer {playerName} Wins!";
            winnerText.gameObject.SetActive(true); 
        }
    }
}
