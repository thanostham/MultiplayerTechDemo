using PurrNet;
using TMPro;
using UnityEngine;
using static GameViewManager;

public class MainGameView : View
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text boostText;


    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<MainGameView>();
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

    public void UpdateBoostStatus(bool canBoost)
    {
        if (canBoost)
        {
            boostText.text = " AVAILABLE";
            boostText.color = Color.green;
        }
        else
        {
            boostText.text = " UNAVAILABLE";
            boostText.color = Color.red;
        }
    }
}
