using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class IpFetcher : MonoBehaviour
{
    public static IpFetcher Instance {get; private set;}
    public string CurrentIp;
    //Site to take IP
    private string Ipify = "https://api.ipify.org?format=json";

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        StartCoroutine(GetIp(ip =>
        {
            if (!string.IsNullOrEmpty(ip))
            {
                CurrentIp = ip;
            }
            else Debug.LogWarning("Could not fetch IP");
        }));
    }

    public IEnumerator GetIp(Action<string> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(Ipify))
        {
            req.SetRequestHeader("Accept", "application/json");
            
            //Error handling
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError) Debug.Log(req.error);
            if(req.result != UnityWebRequest.Result.Success){ Debug.LogError(req.error);
                callback?.Invoke(null); 
                yield break;
            }
            //Sucess handling
            var jsonResponse = req.downloadHandler.text;
            MyIpifyResponse response;

            try
            {
                response = JsonUtility.FromJson<MyIpifyResponse>(jsonResponse);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing Json response: {e.Message}");
                callback?.Invoke(null);
                yield break;
            }
            
            callback?.Invoke(response?.ip);
        }
    }
}

[System.Serializable]
public class MyIpifyResponse
{
    public string ip;
}
