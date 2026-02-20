using PurrNet;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private SyncDictionary<PlayerID, ScoreData> score = new();

    private int killsToWin = 10;

    private bool gameEnded = false;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
        score.onChanged += OnScoreChanged;
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<ScoreManager>();
        score.onChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(SyncDictionaryChange<PlayerID, ScoreData> change)
    {

        if (InstanceHandler.TryGetInstance(out ScoreBoardView scoreBoardView))
        {
            scoreBoardView.SetData(score.ToDictionary());
        }
    }

    private void CheckForDictionaryEntry(PlayerID playerID)
    {
        if (!score.ContainsKey(playerID))
            score.Add(playerID, new ScoreData());
    }

    public void AddKill(PlayerID playerID)
    {
        if (gameEnded)
            return;

        CheckForDictionaryEntry(playerID);

        var scoreData = score[playerID];
        scoreData.kills++;
        score[playerID] = scoreData;

        if (scoreData.kills >= killsToWin)
        {
            gameEnded = true;
            RpcAnnounceWinner(playerID);
        }
    }

    public void AddDeath(PlayerID playerID)
    {
        CheckForDictionaryEntry(playerID);

        var scoreData = score[playerID];
        scoreData.deaths++;
        score[playerID] = scoreData;
    }


    public struct ScoreData
    {
        public int kills;
        public int deaths;

        public override string ToString()
        {
            return $"{kills}/{deaths}";
           
        }
    }

    [ObserversRpc]
    private void RpcAnnounceWinner(PlayerID winnerID)
    {
        if (InstanceHandler.TryGetInstance(out MainGameView view))
        {
            view.ShowWinner(winnerID.ToString());
        }

        StartCoroutine(EndGameRoutine());
        //Debug.Log($"GAME OVER! Player {winnerID} Wins!");
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(3f);

        if (isServer)
        {
            networkManager.StopServer();
        }
        networkManager.StopClient();

        SceneManager.LoadScene("Menu");

        if (InstanceHandler.TryGetInstance(out MainGameView view))
        {
            view.gameObject.SetActive(false);
        }

        if (InstanceHandler.TryGetInstance(out ScoreBoardView boardview))
        {
            boardview.gameObject.SetActive(false);
        }

        
        //enable roompanel
        //disable main view 
    }
}
