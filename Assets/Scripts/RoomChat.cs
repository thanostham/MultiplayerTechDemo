using UnityEngine;
using TMPro;
using PurrNet;

public class RoomChat : NetworkBehaviour {
    public static RoomChat Instance{ get; private set; }
    [Header("UI References")]
    public TMP_InputField chatInput;
    public TextMeshProUGUI chatDisplay; 
    public string playerName;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnSendClicked() {
        string textToSend = chatInput.text;
        
        if (string.IsNullOrWhiteSpace(textToSend)) return;
        
        playerName = Api_Controller.LoggedInUsername;
        
        SendChatMessageServerRPC(playerName, textToSend);
        chatInput.text = "";
    }
    
    [ServerRpc]
    private void SendChatMessageServerRPC(string senderName, string message) {
        string formattedMessage = $"<b>[{senderName}]:</b> {message}";
        
        ReceiveChatMessageObserverRPC(formattedMessage);
    }
    
    [ObserversRpc]
    private void ReceiveChatMessageObserverRPC(string formattedMessage) {
        chatDisplay.text += $"\n{formattedMessage}";
    }
}