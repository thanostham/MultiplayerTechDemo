using UnityEngine;
using PurrNet.StateMachine;
using System.Collections.Generic;

public class RoundRunningState : StateNode<List<PlayerHealth>>
{
    private int playerAlive;

    public override void Enter(List<PlayerHealth> data, bool asServer)
    {
        base.Enter(data, asServer);

        if (!asServer)
            return;


        playerAlive = data.Count;

        foreach (var player in data)
        {
            player.OnDeath_Server += OnPlayerDeath;
        }
    }

    private void OnPlayerDeath(PlayerHealth deadPlayer)
    {
        deadPlayer.OnDeath_Server -= OnPlayerDeath;
        playerAlive--;

        if (playerAlive <= 1)
        {
            Debug.Log("Someone won");
        }
    }
}
