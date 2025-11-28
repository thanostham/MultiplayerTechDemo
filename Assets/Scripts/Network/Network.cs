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
        networkManager = FindObjectOfType<NetworkManager>();
        foodManager = FindObjectOfType<FoodManager>();
        StartHost();
    }

    public void StartHost()
    {
        if (!networkManager.isHost)
        {
            networkManager.StartHost();
            StartCoroutine(WaitAndSpawn());
        }
        
    }

    public void JoinGame()
    {
        if (!networkManager.isClient)
        {
            networkManager.StartClient();
            StartCoroutine(WaitAndSpawn());
        }
    }
    
    private IEnumerator WaitAndSpawn()
    {
        //Wait for network to fully initialize
        yield return new WaitForSeconds(0.5f);
        
        //Extra check to ensure we're connected
        while (!networkManager.isClient && !networkManager.isServer)
        {
            yield return null;
        }

        SpawnPlayer();
        foodManager.StartFood();
    }

    public void SpawnPlayer()
    {
        if (localPlayerIdentity != null)
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

        networkManager.Spawn(playerObj);

        localPlayerIdentity = playerObj.AddComponent<NetworkIdentity>();

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