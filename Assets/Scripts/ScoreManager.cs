using PurrNet;
using UnityEngine;
using System;

public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private SyncDictionary<PlayerID, ScoreData> score = new();

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
        CheckForDictionaryEntry(playerID);

        var scoreData = score[playerID];
        scoreData.kills++;
        score[playerID] = scoreData;
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
        //public int killRatio;

        public override string ToString()
        {
            return $"{kills}/{deaths}";
           
        }
    }
}
