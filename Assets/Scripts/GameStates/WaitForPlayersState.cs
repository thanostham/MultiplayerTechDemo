using UnityEngine;
using PurrNet.StateMachine;
using System.Collections;

public class WaitForPlayersState : StateNode
{
    [SerializeField] private int minPlayers = 1;

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        StartCoroutine(WaitForPlayers());
    }

    private IEnumerator WaitForPlayers()
    {
        while (networkManager.players.Count < minPlayers)
            yield return null;

        machine.Next();
    }
}
