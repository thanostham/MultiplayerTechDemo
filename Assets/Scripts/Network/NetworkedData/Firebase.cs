using System;
using System.Collections;
using Firebase;
using Firebase.Database;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/*
 * FirebaseManager, manages data across our 2 databases. The firebase database (realtime) that saves
 * username and password under device ID (unique), And the SQLite database that manages in game data like skins.
 * Local SQLite database runs inside project files, can be found at D(isk):\Users\Banana\source\repos\RestApi\RestApi
 */
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; set; }
    
    [SerializeField] TMP_InputField Username;
    [SerializeField] TMP_InputField Password;
    
    [SerializeField] private TMP_Text ErrorText;
    [SerializeField] private TMP_Text NameToGet;
    [SerializeField] private TMP_Text PasswordToGet;

    [SerializeField] private TMP_Text Warnings;
    [SerializeField] private GameObject UserPanel;
    [SerializeField] private GameObject RoomPanel;
    public User newUser;
    public string userID;
    DatabaseReference dbReference;

    //REST API URL (runs on Swagger UI)
    private string restApiUrl = "https://localhost:7284/api/Api";

    private void Awake()
    {
        Instance = this;
        
        //There was an error about development certificates, this fixes it smh ? 
        System.Net.ServicePointManager.ServerCertificateValidationCallback = 
            delegate { return true; };
        
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("User Device ID: " + userID);
    }

    public IEnumerator CheckUserExists(Action<bool> callback)
    {
        var userData = dbReference.Child("users").Child(userID).Child("name").GetValueAsync();
        yield return new WaitUntil(predicate: () => userData.IsCompleted);

        if (userData.Result.Exists)
        {
            callback.Invoke(true);
            Warnings.text = "Error : User already exists by this name in your Computer (User exists under same UID";
        }
        else
        {
            callback.Invoke(false);
            CreateUser();
        }
    }

    public void CreateUser()
    {
        newUser = new User(Username.text, Password.text);
        string json = JsonUtility.ToJson(newUser);

        //Save to Firebase
        dbReference.Child("users").Child(userID).SetRawJsonValueAsync(json);
        
        //Sync SQLite to REST
        StartCoroutine(SyncUserToSQLite(Username.text, 0));//Default skin_ID = 0
    }

    //SQLite database
    private IEnumerator SyncUserToSQLite(string username, int skinID)
    {
        UsersDT sqliteUser = new UsersDT
        {
            Id = userID,  //Device ID as primary key
            Name = username,
            skin_ID = skinID
        };

        string json = JsonUtility.ToJson(sqliteUser);
        
        using (UnityWebRequest request = UnityWebRequest.Post(restApiUrl, json, "application/json"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User synced to SQLite: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error syncing to SQLite: " + request.error);
            }
        }
    }

    //Get data from SQLite
    public IEnumerator GetUserDataFromSQLite(Action<UsersDT> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{restApiUrl}/{userID}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                UsersDT user = JsonUtility.FromJson<UsersDT>(request.downloadHandler.text);
                callback.Invoke(user);
                Debug.Log($"Retrieved from SQLite - Name: {user.Name}, Skin: {user.skin_ID}");
            }
            else
            {
                Debug.LogError("Error getting from SQLite: " + request.error);
                callback.Invoke(null);
            }
        }
    }

    //Update skin in SQLite
    public IEnumerator UpdateSkinInSQLite(int newSkinID)
    {
        UsersDT sqliteUser = new UsersDT
        {
            Id = userID,
            Name = Username.text,
            skin_ID = newSkinID
        };

        string json = JsonUtility.ToJson(sqliteUser);
        
        using (UnityWebRequest request = UnityWebRequest.Post(restApiUrl, json, "application/json"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Skin updated in SQLite: " + newSkinID);
            }
            else
            {
                Debug.LogError("Error updating skin: " + request.error);
            }
        }
    }

    public IEnumerator GetUsername(Action<string> callback)
    {
        var UserNameData = dbReference.Child("users").Child(userID).Child("name").GetValueAsync();

        yield return new WaitUntil(predicate: () => UserNameData.IsCompleted);

        if (UserNameData != null)
        {
            DataSnapshot snapshot = UserNameData.Result;
            callback.Invoke(snapshot.Value.ToString());
        }
    }

    public IEnumerator GetPassword(Action<string> callback)
    {
        var UserPasswordData = dbReference.Child("users").Child(userID).Child("password").GetValueAsync();

        yield return new WaitUntil(predicate: (() => UserPasswordData.IsCompleted));

        if (UserPasswordData != null)
        {
            DataSnapshot snapshot = UserPasswordData.Result;
            callback.Invoke(snapshot.Value.ToString());
        }
    }

    public void GetUserInfo()
    {
        StartCoroutine(GetUsername((name) => { NameToGet.text = "Name " + name; }));
        StartCoroutine(GetPassword(password => { PasswordToGet.text = "Password " + password; }));
        
        //Get data from SQLite
        StartCoroutine(GetUserDataFromSQLite((userData) => {
            if (userData != null)
            {
                Debug.Log($"SQLite Data - Skin ID: {userData.skin_ID}");
            }
        }));
    }

    public void CheckAndCreate()
    {
        if (userID == null) return;
        
        StartCoroutine(CheckUserExists((exists) => {}));
    }

    public void ContinueToScene(bool isUser = false)
    {
        if (userID != null) isUser = true;
        if (isUser && Username.text == NameToGet.text && Password.text == PasswordToGet.text)
        {
            SwitchToPanel();
        }
        else
        {
            ErrorText.text = "User not found";
        }
        
    }

    public void SwitchToPanel()
    {
        if (userID == null) Debug.LogError("Cannot switch to panel becase User is null");
        else
        {
           UserPanel.SetActive(false);
           RoomPanel.SetActive(true);
        }
    }
}

//Firebase User struct
public struct User
{
    public string name;
    public string password;
    
    public User(string username, string userPassword)
    {
        this.name = username;
        this.password = userPassword;
    }
}

//SQLite User 
[System.Serializable]
public class UsersDT
{
    public string Id;
    public string Name;
    public int skin_ID;
}