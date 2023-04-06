using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riptide;
using Riptide.Utils;
using GameNetServer;
namespace Serveur
{
    public class MatchMakingClient
    {
        string ip;
        ushort port;
    
        public static Client? client;
        public static Game game;
        public MatchMakingClient(string ip, ushort port)
        {
            this.ip = ip;
            this.port = port;
            client = new Client();


        }

        public void SetupClient() {
            
            RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);

            Console.WriteLine("Waiting for connection with MatchMaking Server ...");

            client.Connected += (s, e) => OnConnected();
            client.Disconnected += (s, e) => onDisconnected();
    
            client.Connect($"{ip}:{port}", 5, 1);
            
            while (true)
            {
                if (client != null) {
                    client.Update();
                }
            }

            Console.WriteLine("Client MatchMaking stopped");

        }

        private void onDisconnected()
        {

        }

        private void OnConnected()
        {
            Console.WriteLine("Connected to MatchMaking Server");
            //SendMessage(MMCodes.SERVER_OPEN);
            //Console.WriteLine("Server Open Message Sended To Matchaking Server");

        }

        public static void SendMessage(MMCodes MMCode)
        {
            if (client == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)MMCode);
            client.Send(mes);
        }

        public static void SendBytes(byte[] bytesToSend, MMCodes MMCode)
        {
            if (client == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)MMCode);
            mes.Add(bytesToSend);
            client.Send(mes);
        }
        public static void SendString(string stringToSend, MMCodes MMCode)
        {
            if (client == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)MMCode);
            mes.Add(stringToSend);
            client.Send(mes);
        }

        [MessageHandler((ushort)MMCodes.GET_LOAD_SAVE, 1)]
        public static void GetSaveFromServer(Message message)
        {
            Console.WriteLine("GetSaveFromServer");
            byte[] savedata = message.GetBytes();
			int oldsaveround = message.GetInt();
			string oldSaveID = message.GetString();
			string oldplyrOneName = message.GetString();
			string oldplyrTwoName = message.GetString();
            Console.WriteLine("GetSaveFromServer 2");
            game.LoadGame(savedata, oldsaveround, oldSaveID, oldplyrOneName, oldplyrTwoName);
        }
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

}