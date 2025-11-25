using UnityEngine;
using PurrNet;

public class Network : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float spawnRangeX = 30f;
    [SerializeField] private float spawnRangeY = 30f;
    
    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    public void StartHost()
    {
        if (networkManager.isServer || networkManager.isClient) return;
    
        networkManager.StartHost();
        SpawnPlayer();
    }

    public void JoinGame()
    {
        if (networkManager.isClient) return;
    
        networkManager.StartClient();
        SpawnPlayer();
    }
    
    public void StopClient()
    {
        networkManager.StopClient();
    }

    private void SpawnPlayer()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-spawnRangeX / 2f, spawnRangeX / 2f),
            Random.Range(-spawnRangeY / 2f, spawnRangeY / 2f),
            1f
        );
        
        GameObject player = Instantiate(playerPrefab, randomPos, Quaternion.identity);
        
        NetworkIdentity identity = playerPrefab.GetComponent<NetworkIdentity>();
        if (identity != null)
        {
            identity.GiveOwnership(networkManager.localPlayer);
        }
    }
}