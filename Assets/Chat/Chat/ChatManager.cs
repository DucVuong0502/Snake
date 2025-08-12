using Fusion;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Intance;
    public ChatUI chatUI;

    private void Awake()
    {
        Intance = this;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string senderName, string message, RpcInfo info = default)
    {
        Debug.Log($"[Chat] {senderName}: {message}");
        chatUI.chatContent.text += $"\n{senderName}: {message}";
    }

    [System.Obsolete]
    public void SendChatMessage(string message)
    {
        // Tìm NetworkPlayer local
        var playerObj = FindObjectOfType<NetworkPlayer>();
        string senderName = playerObj != null ? $"Player_{playerObj.PlayerId}" : "Unknown";

        RPC_SendMessage(senderName, message);
    }
}