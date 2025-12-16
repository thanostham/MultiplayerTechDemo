using System;
using System.Collections;
using Unity;
using Firebase;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FirebaseManager : MonoBehaviour
{
    [SerializeField] TMP_InputField Username;
    [SerializeField] TMP_InputField Password;

    [SerializeField] private TMP_Text NameToGet;
    [SerializeField] private TMP_Text PasswordToGet;
    
    public User newUser;
    public string userID;
    DatabaseReference dbReference;

    private void Awake()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
    }

    public void CreateUser()
    {
        newUser = new User(Username.text, Password.text);
        string json = JsonUtility.ToJson(newUser);

        dbReference.Child("users").Child(userID).SetRawJsonValueAsync(json);
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
        var UserPasswordData = dbReference.Child("users").Child("password").GetValueAsync();
        
        yield return new WaitUntil(predicate:(() => UserPasswordData.IsCompleted));

        if (UserPasswordData != null)
        {
            DataSnapshot snapshot = UserPasswordData.Result;
            callback.Invoke(snapshot.Value.ToString());
        }
    }

    public void GetUserInfo()
    {
        StartCoroutine(GetUsername((string name) => {NameToGet.text = "Name "+name;}));
        
        StartCoroutine(GetPassword(password => {PasswordToGet.text = "Password "+password;}));
    }

    public void UpdateName()
    {
        dbReference.Child("users").Child(userID).Child("name").SetValueAsync(Username.text);
    }

    public void UpdatePassword()
    {
        dbReference.Child("users").Child(userID).Child("password").SetValueAsync(Username.text);
    }
    
}

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
