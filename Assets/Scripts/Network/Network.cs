using System;
using System.Collections;
using UnityEngine;
using PurrNet;
using PurrNet.Transports;
using TMPro;
using UnityEngine.SceneManagement;
using PurrNet.Modules;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Network : MonoBehaviour
{
    public static Network NetInstance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float spawnRangeX = 30f;
    [SerializeField] private float spawnRangeY = 30f;

    [SerializeField] private int mainMenuSceneId = 2;
    [SerializeField] private int gameSceneId = 1;
    [SerializeField] private TMP_InputField roomIField;

    private NetworkManager networkManager;
    private FoodManager foodManager;
    public  NetworkIdentity localPlayerIdentity;

    private PurrTransport _Ptransport;

    private void Awake()
    {
        if (NetInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        NetInstance = this;
        DontDestroyOnLoad(gameObject);

        _Ptransport = GetComponent<PurrTransport>();

        SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
    }

    private void Start()
    {
        networkManager = Object.FindAnyObjectByType<NetworkManager>();
    }

    private void SceneManagerOnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex != gameSceneId)
            return;

        StartCoroutine(GameSceneReady());
    }

    private IEnumerator GameSceneReady()
    {
        yield return null;

        if (networkManager == null)
        {
            networkManager = NetInstance.GetComponent<NetworkManager>();
            foodManager = FoodManager.Instance;
        }

        if (foodManager == null)
        {
            foodManager = FoodManager.Instance;

            if (foodManager == null)
            {
                foodManager = FindAnyObjectByType<FoodManager>();
            }
        }

        if (networkManager == null)
        {
            Debug.LogError("NetworkManager not found in game scene");
            yield break;
        }

        if (localPlayerIdentity != null)
            yield break;

        yield return SpawnPlayerWithOwnership();

        if (networkManager.isHost && foodManager != null)
        {
            foodManager.StartFood();
        }
        else if (networkManager.isHost && foodManager == null)
        {
            Debug.LogError("FoodManager not found in game scene");
        }
    }

    public void StartHost()
    {
        if (networkManager.isHost)
            return;

        if (!CheckRoomName())
            return;

        HostAsync();
    }

    public void JoinGame()
    {
        if (networkManager.isClient)
            return;

        if (!CheckRoomName())
            return;

        networkManager.StartClient();
    }

    private async void HostAsync()
    {
        networkManager.StartHost();

        while (!networkManager.isHost)
            await Task.Delay(50);

        PurrSceneSettings sceneSettings = new PurrSceneSettings
        {
            isPublic = true,
            mode = LoadSceneMode.Single
        };

        await networkManager.sceneModule.LoadSceneAsync(gameSceneId, sceneSettings);
    }

    private IEnumerator SpawnPlayerWithOwnership()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-spawnRangeX / 2f, spawnRangeX / 2f),
            Random.Range(-spawnRangeY / 2f, spawnRangeY / 2f),
            1f
        );

        GameObject playerObj = Instantiate(playerPrefab, randomPos, Quaternion.identity);

        yield break;
    }

    private bool CheckRoomName()
    {
        if (!string.IsNullOrEmpty(_Ptransport.roomName))
            return true;

        if (!string.IsNullOrEmpty(roomIField.text))
        {
            _Ptransport.roomName = roomIField.text;
            return true;
        }

        return false;
    }

    public void UpdateRoomName(string newRoomName)
    {
        _Ptransport.roomName = newRoomName;
    }

    public void StopClient()
    {
        if (localPlayerIdentity != null)
        {
            Destroy(localPlayerIdentity.gameObject);
            localPlayerIdentity = null;
        }

        if (networkManager.isHost && foodManager != null)
        {
            foodManager.DespawnAllFood();
        }

        networkManager.StopClient();
    }
}
