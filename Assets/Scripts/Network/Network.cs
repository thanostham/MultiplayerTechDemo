//Contribution Both

using System.Collections;
using UnityEngine;
using PurrNet;

public class Network : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float spawnRangeX = 30f;
    [SerializeField] private float spawnRangeY = 30f;
    
    private NetworkManager networkManager;
    private NetworkIdentity localPlayerIdentity;

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void StartHost()
    {
        if (!networkManager.isServer)
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
        // Wait for network to fully initialize
        yield return new WaitForSeconds(0.5f);
        
        // Extra check to ensure we're connected
        while (!networkManager.isClient && !networkManager.isServer)
        {
            yield return null;
        }
        
        SpawnPlayer();
    }
    
    private void SpawnPlayer()
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
        
        //Get the NetworkIdentity
        localPlayerIdentity = playerObj.GetComponent<NetworkIdentity>();
        
        //Give Ownership
        if (localPlayerIdentity != null)
        {
            localPlayerIdentity.GiveOwnership(networkManager.localPlayer);
            Debug.Log($"Player spawned with ownership: {localPlayerIdentity.isOwner}");
            
            StartCoroutine(CheckOwnershipDelayed());
        }
        else
        {
            Debug.LogError("PlayerPrefab is missing NetworkIdentity/NetworkBehaviour component!");
        }
    }
    
    private IEnumerator CheckOwnershipDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log($"Ownership check - isOwner: {localPlayerIdentity.isOwner}, Owner: {localPlayerIdentity.owner}");
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