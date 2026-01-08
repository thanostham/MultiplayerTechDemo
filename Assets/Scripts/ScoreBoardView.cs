using PurrNet;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameViewManager;

public class ScoreBoardView : View
{
    [SerializeField] private Transform scoreboardEntriesParent;
    [SerializeField] private ScoreBoardEntry scoreBoardEntryPrefab;

    private GameViewManager gameViewManager;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
    }

    private void Start()
    {
        gameViewManager = InstanceHandler.GetInstance<GameViewManager>();
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<ScoreBoardView>();
    }

    public void SetData(Dictionary<PlayerID, ScoreManager.ScoreData> data)
    {
        foreach (Transform child in scoreboardEntriesParent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var playerScore in data)
        {
            var entry = Instantiate(scoreBoardEntryPrefab, scoreboardEntriesParent);
            entry.SetData(playerScore.Key.id.ToString(), playerScore.Value.kills, playerScore.Value.deaths);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("dosent show up");
            gameViewManager.ShowView<ScoreBoardView>(false);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            gameViewManager.HideView<ScoreBoardView>();
        }
    }

    public override void OnShow()
    {
        
    }

    public override void OnHide()
    {
        
    }
}
