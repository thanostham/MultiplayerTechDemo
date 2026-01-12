using PurrNet;
using PurrNet.Transports;
using UnityEngine;

public class RefManager : NetworkBehaviour
{
  [SerializeField] private GameObject ScoreBoard;
  [SerializeField] private GameObject PlayerUI;

  protected override void OnSpawned()
  {
    PlayerUI.SetActive(true);
  }
  public void ShowTheTabs()
  {
    if (!isController) return;

    if (ScoreBoard != null && PlayerUI != null)
    {
      if(Input.GetKeyDown(KeyCode.Tab)) ScoreBoard.SetActive(true);
      if (Input.GetKeyUp(KeyCode.Tab)) ScoreBoard.SetActive(false);
    }

  }
}
