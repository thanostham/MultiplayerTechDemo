using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Api_Controller : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    
    public GameObject LoginPanel;
    public GameObject RoomPanel;

    [Header("API Settings")]
    public string baseUrl = "https://rest-db-production.up.railway.app/api/Api";
    
    public void OnRegisterPressed()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            Debug.LogWarning("Username or Password cannot be empty!");
            return;
        }
        
        StartCoroutine(RegisterUser(user, pass));
    }

    public void OnLoginPressed()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            Debug.LogWarning("Username or Password cannot be empty!");
            return;
        }
        
        StartCoroutine(LoginUser(user, pass));
    }
    
    private IEnumerator RegisterUser(string username, string password)
    {
        string jsonData = $"{{\"Name\":\"{username}\", \"Pass\":\"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/add_user", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            
            if (request.responseCode == 200)
            {
                Debug.Log("User created successfully!");
                LoginPanel.SetActive(false);
                RoomPanel.SetActive(true);
            }
            else if (request.responseCode == 409)
            {
                Debug.LogWarning("User already exists. Try a different name.");
            }
            else
            {
                Debug.LogError($"Error: {request.responseCode} - {request.error}");
            }
        }
    }

    private IEnumerator LoginUser(string username, string password)
    {
        string jsonData = $"{{\"Name\":\"{username}\", \"Pass\":\"{password}\"}}";
        
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                Debug.Log("Login Successful!");
                LoginPanel.SetActive(false);
                RoomPanel.SetActive(true);
            }
            else if (request.responseCode == 404 || request.responseCode == 401)
            {
                Debug.LogWarning("Login failed: Incorrect username or password.");
            }
            else
            {
                Debug.LogError($"Error: {request.responseCode} - {request.error}");
            }
        }
    }
}