using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNetClient;
using Riptide;
using Riptide.Utils;
using System;

public class NetPlayer : Singleton<NetPlayer>
{
    [SerializeField] string MMip;
    [SerializeField] ushort MMport;

    [SerializeField] string lobbyIp;

    [SerializeField] string ip;
    [SerializeField] ushort port;

    [SerializeField] Button connectButton;
    [SerializeField] Button searchButton;
    [SerializeField] NameGestor nameGestor;

    public static Client client;
    public static Client MMclient;

    // Start is called before the first frame update
    void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.Log, Debug.Log, false);
        client = new Client();
        MMclient = new Client();

        MMclient.Connected += (s, e) => OnMatchMakingJoined();

        //connectButton.onClick.AddListener(() => connectToServer());
        connectButton.onClick.AddListener(() => connectToMatchMaking());
        searchButton.onClick.AddListener(() => launchSearchPlayer());
        client.Connected += (s, e) => onGameServerConnected();
        client.ConnectionFailed += (s, e) => { connectButton.interactable = true; };
        client.Disconnected += (s, e) => onDisconnected();
    }

    private void launchSearchPlayer()
    {
        Utilities.Debugger("Waiting for another player ...");
        MMSendMessage(MMCodes.PLAYER_JOIN);
    }

    void connectToMatchMaking()
    {
        MMclient.Connect($"{MMip}:{MMport}", 5, 1);
        connectButton.interactable = false;
    }

    bool HaveToLoad;

    void connectToServer(string ipToConnect)
    {
        client.Connect(ipToConnect);
        connectButton.interactable = false;
        
    }

    private void FixedUpdate()
    {

    }

    private void OnApplicationQuit()
    {
        client.Disconnect();
        MMclient.Disconnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (client != null)
            client.Update();
        if (MMclient != null)
            MMclient.Update();
    }

    void OnMatchMakingJoined()
    {
        Debug.Log("MatchMaking Joined !");
        Debug.Log("Message PLAYER_JOIN Sended !");
        Utilities.Instance.showScreen(ScreenGame.Menu);

        // Can be on OnSearchButtonClicked
        Utilities.Debugger("Waiting for another player ...");
        MMSendMessage(MMCodes.PLAYER_JOIN);
    }

    private void onGameServerConnected()
    {
        connectButton.interactable = false;
        if (HaveToLoad)
        {
            Utilities.Debugger("");
            Utilities.Instance.showScreen(ScreenGame.Game);
        }
        else
        {
            Utilities.Debugger("");
            Utilities.Instance.showScreen(ScreenGame.Login);
        }

        Debug.Log("MY CONNECTED !!");
        SendString("Hello Server !", whatmessage.welcome);
    }

    public static void SendString(string stringtosend, whatmessage messageSendMode)
    {
        Message mes = Message.Create(MessageSendMode.Reliable, (ushort)messageSendMode);
        mes.Add(stringtosend);
        client.Send(mes);
    }

    public static void SendBytes(byte[] bytetabtosend, whatmessage messageSendMode)
    {
        Message mes = Message.Create(MessageSendMode.Reliable, (ushort)messageSendMode);
        mes.Add(bytetabtosend);
        client.Send(mes);
    }

    void onDisconnected()
    {
        connectButton.interactable = true;
        nameGestor.setInterctablesBtns(true);
        
    }

    [MessageHandler((ushort)MMCodes.REDIRECT_PLAYER, 1)]
    public static void GetServerToConnect(Message message)
    {
        string ipToConnect = message.GetString();
        if (ipToConnect == "NOSERVER")
        {
            Instance.connectButton.interactable = true;
        }
        if (ipToConnect.EndsWith("L"))
        {
            Instance.HaveToLoad = true;
            ipToConnect = ipToConnect.Substring(0, ipToConnect.Length-1);
        }
        Instance.lobbyIp = ipToConnect;
        Instance.connectToServer($"{ipToConnect}:7777");
    }
    
    [MessageHandler((ushort)MMCodes.GET_LOAD_SAVE, 1)]
    public static void GetSaveFromMM(Message message)
    {
        byte[] data = message.GetBytes();
        Game.Instance.UpdatePlyrsFromServer(data);

        //Debug.Log("Update Loaded from server");
        //Utilities.Debugger("Update Loaded from server");
    }




    [MessageHandler((ushort)whatmessage.UpdateAndWait)]
    public static void UpdatePlayersInfo(Message message)
    {
        
        byte[] data = message.GetBytes();
        Game.Instance.UpdatePlyrsFromServer(data);

        Debug.Log("Update and wait");
        Utilities.Debugger("Enemy's turn");
    }



    [MessageHandler((ushort)whatmessage.AskPlayerName)]
    public static void PlyrNameAsked(Message message)
    {
        byte[] data = message.GetBytes();
        byte oneByte = data[0];
        if (oneByte == 1)
        {
            Game.Instance.plyrToSet = 1;
        }
        if (oneByte == 2)
        {
            Game.Instance.plyrToSet = 2;
        }

        Debug.Log("PlyrNameAsked");
        //Utilities.Debugger("PlyrNameAsked");
        Game.Instance.CheckName(SavesSystem.Instance.PlayerUniqueId);
        SendString(SavesSystem.Instance.PlayerUniqueId, whatmessage.clientname);
        
    }

    public static void MMSendMessage(MMCodes MMCode)
    {
        if (MMclient == null) return;
        Message mes = Message.Create(MessageSendMode.Reliable, (ushort)MMCode);
        MMclient.Send(mes);
    }

    public static void MMSendString(string strToSend, MMCodes MMCode)
    {
        if (MMclient == null) return;
        Message mes = Message.Create(MessageSendMode.Reliable, (ushort)MMCode);
        mes.Add(strToSend);
        Debug.Log("i send strToSend : " + strToSend);
        MMclient.Send(mes);
    }

}
public enum whatmessage
{
    welcome,
    clientname,
    PlyrChoice,
    AskPlayerMove,
    PlyrMove,
    AskPlayerName,
    OtherClientName,
    UpdateAndWait,
    Winner,
}

public enum MMCodes
{
    SERVER_OPEN,
    SERVER_CLOSED,
    PLAYER_JOIN,
    REDIRECT_PLAYER,
    ASK_ENEMY_NAME,
    GET_ENEMY_NAME,
    ASK_LOAD_SAVE,
    GET_LOAD_SAVE,
}