//Contribution Both

using System.Collections;
using UnityEngine;
using PurrNet;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

public class Network : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float spawnRangeX = 30f;
    [SerializeField] private float spawnRangeY = 30f;
    
    private NetworkManager networkManager;
    private FoodManager foodManager;
    private NetworkIdentity localPlayerIdentity;

    private void Start()
    {
        networkManager = Object.FindAnyObjectByType<NetworkManager>();
        foodManager = Object.FindAnyObjectByType<FoodManager>();
       // StartHost();
    }

    public void StartHost()
    {
        if (!networkManager.isHost)
        {
            networkManager.StartHost();
            Debug.Log("trying to spawn as a host");
            StartCoroutine(WaitAndSpawn());
        }
        
    }

    public void JoinGame()
    {
        if (!networkManager.isClient)
        {
            networkManager.StartClient();
            Debug.Log("trying to spawn as a client");
            StartCoroutine(WaitAndSpawn());
        }
    }
    
    private IEnumerator WaitAndSpawn()
    {
        //Wait for network to fully initialize
        Debug.Log("Waiting for 0.5sec");
        yield return new WaitForSeconds(0.5f);
        
        //Extra check to ensure we're connected
        while (!networkManager.isClient && !networkManager.isHost && !networkManager.isServer)
        {
            Debug.Log("ALL WENT TO SHIT, connected as ????");
            yield return null;
        }

        Debug.Log("Spawning player and food...");
        SpawnPlayer();
        foodManager.StartFood();
    }

    public void SpawnPlayer()
    {
        if (localPlayerIdentity)
        {
            Debug.LogWarning("Player already spawned!");
            return;
        }

        Vector3 randomPos = new Vector3(
            Random.Range(-spawnRangeX / 2f, spawnRangeX / 2f),
            Random.Range(-spawnRangeY / 2f, spawnRangeY / 2f),
            1f
        );

        GameObject playerObj = Instantiate(playerPrefab, randomPos, Quaternion.identity);
        Debug.Log($"[player spawned :{playerObj}]");
        //networkManager.Spawn(playerObj);

        localPlayerIdentity = playerObj.GetComponent<MovementLogic>();

        if (localPlayerIdentity != null)
        {
            localPlayerIdentity.GiveOwnership(networkManager.localPlayer);
        }      
    }
    
    public void StopClient()
    {
        if (localPlayerIdentity != null)
        {
            Destroy(localPlayerIdentity.gameObject);
            localPlayerIdentity = null;
        }
        
        networkManager.StopClient();
    }
}