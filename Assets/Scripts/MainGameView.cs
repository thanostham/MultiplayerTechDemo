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
}
