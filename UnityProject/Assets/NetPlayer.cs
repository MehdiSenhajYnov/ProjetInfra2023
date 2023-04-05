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
        //TOADD MMSendMessage(MMCodes.PLAYER_JOIN);
    }

    private void onGameServerConnected()
    {
        connectButton.interactable = false;
        Utilities.Instance.showScreen(ScreenGame.Login);

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
        Instance.lobbyIp = ipToConnect;
        Instance.connectToServer($"{ipToConnect}:7777");
    }



    [MessageHandler((ushort)whatmessage.UpdateAndWait)]
    public static void UpdatePlayersInfo(Message message)
    {
        byte[] data = message.GetBytes();
        Game.Instance.UpdatePlyrsFromServer(data);

        Debug.Log("Update and wait");
        Utilities.Debugger("Update and wait");
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
        Utilities.Debugger("PlyrNameAsked");

        NetPlayer.SendString(SavesSystem.Instance.PlayerUniqueId, whatmessage.clientname);
        
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
}
