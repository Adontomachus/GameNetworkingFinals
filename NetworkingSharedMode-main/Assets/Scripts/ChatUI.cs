using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using System;


public class ChatUI : NetworkBehaviour
{
    public TextMeshProUGUI messages;
    public TextMeshProUGUI input;
    public TextMeshProUGUI usernameInput;

    public GameObject chatUI;
    public event Action<string> OnMessageSent;
    public string username = "Player";
    // Start is called before the first frame update
    void Start()
    {
        chatUI.SetActive(false);
        username = "Player";
    }
    private void Awake()
    {

    }

    public void SendMessage()
    {
        string message = input.text;
        chatUI.SetActive(false);
        RPC_SendMessage(message, username);
    }
    private void Update()
    {
        if (UnityEngine.Input.GetKeyUp(KeyCode.Alpha1))
        {
            chatUI.SetActive(true);
            input.text = "";
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string username, string message, RpcInfo rpcInfo = default)
    {
        messages.text += $"{message}: {username}\n";
    }
}
