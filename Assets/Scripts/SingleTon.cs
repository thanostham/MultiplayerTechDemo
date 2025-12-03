using UnityEngine;

public class SingleTon : MonoBehaviour
{
    public static GameObject Instance { get; private set; }

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this.gameObject;

        DontDestroyOnLoad(gameObject);
    }
}
