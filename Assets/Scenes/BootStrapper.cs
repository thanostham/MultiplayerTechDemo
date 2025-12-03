//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Bootstrapper : MonoBehaviour
//{
//    const string sceneName = "TestBootScene";

//    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//    public static void Execute()
//    {
//        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
//        {
//            var canditate = SceneManager.GetSceneAt(sceneIndex);

//            if (canditate.name == sceneName) return;
//        }

//        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
//    }
//}

//public class BootstrappedData : MonoBehaviour
//{
//    public static BootstrappedData Instance { get; private set; } = null;

//    private void Awake()
//    {
//        if(Instance != null)
//        {
//            Debug.LogError("Found another bootstrapper Data on" + "["+gameObject.name+"]");
//            Destroy(gameObject);
//            return;
//        }
//        DontDestroyOnLoad(gameObject);
//    }
//}