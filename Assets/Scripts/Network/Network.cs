using System.Collections;
using UnityEngine;
using PurrNet;
using PurrNet.Transports;
using TMPro;
using UnityEngine.SceneManagement;
using PurrNet.Modules;
using System.Threading.Tasks;

public class Network : MonoBehaviour
{
    public static Network NetInstance { get; set; }
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float spawnRangeX = 30f;
    [SerializeField] private float spawnRangeY = 30f;
                                                                                        
    private NetworkManager networkManager;
    private FoodManager foodManager;
    private NetworkIdentity localPlayerIdentity;
    public PurrTransport _Ptransport;

    [SerializeField]private int mainMenuSceneId = 1;
    [SerializeField]private int gameSceneId =2;

    [SerializeField] private TMP_InputField roomIField;


    private void Awake()
    {
        NetInstance = this;
        DontDestroyOnLoad(NetInstance);
        _Ptransport = GetComponent<PurrTransport>();

        SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
    }

    private void Start()
    {
        networkManager = Object.FindAnyObjectByType<NetworkManager>();
        foodManager = Object.FindAnyObjectByType<FoodManager>();
       // StartHost();
    }

    public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene newScene)
    {
        if (newScene.buildIndex != mainMenuSceneId) return;
    }

    private bool CheckRoomName()
    {
        if(!string.IsNullOrEmpty(_Ptransport.roomName)) return true;

        if (!string.IsNullOrEmpty(roomIField.text))
        {
            _Ptransport.roomName = roomIField.text; 
            return true;
        }
        Debug.Log("Room name empty");
        return false;
    }

    public void UpdateRoomName(string newRoomName)
    {
        _Ptransport.roomName = newRoomName;
    }

    public void StartHost()
    {
        if (!networkManager.isHost)
        {
            if (!CheckRoomName()) return;
            HostAsync();
            Debug.Log("trying to spawn as a host");
            StartCoroutine(WaitAndSpawn());
        }
        
    }

    public void JoinGame()
    {
        if (!networkManager.isClient)
        {
            if (!CheckRoomName()) return;
            networkManager.StartClient();
            //UIManager.Instance.ToggleMainMenu(false); RETURN TO THIS LATER
            Debug.Log("trying to spawn as a client");
            StartCoroutine(WaitAndSpawn());
        }
    }
    
    private async void HostAsync()
    {
        networkManager.StartHost();

        while (!networkManager.isHost) 
        {
            await Task.Delay(100);
        }
 
        PurrSceneSettings sceneSettings = new PurrSceneSettings();
        sceneSettings.isPublic = true;
        sceneSettings.mode = LoadSceneMode.Single;
        AsyncOperation asyncProgress = networkManager.sceneModule.LoadSceneAsync(gameSceneId, sceneSettings);
        
        await asyncProgress;
      
        //UIManager.Instance.ToggleMainMenu(false);   RETURN TO THIS LATER
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
        if (networkManager.isHost)
        {
            foodManager.StartFood();
            
        }
        else
        { 
            Debug.Log("not a hosr so no food for u !!!");
        }
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