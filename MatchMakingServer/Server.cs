using System;
using Riptide;
using Riptide.Utils;

namespace MatchMakingServer
{
	public class MMServer
	{
		public ushort port;
        public static Server? server;
		bool isRunning;

		public MMServer()
		{
		}

		public void SetupServer()
        {
            Console.WriteLine("MatchMaking Server Initializing...");

            port = 8888;
            RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);
            server = new Server();
            server.Start(port, 100, 1);

			Console.WriteLine("MatchMaking Server Started!");

            server.ClientConnected += (s, e) => onconnect(s, e);
            server.ClientDisconnected += (s, e) => ondisconnect(s, e);
            isRunning = true;

			while (isRunning)
			{
				server.Update();
			}

			server.Stop();
			Console.WriteLine("MatchMaking Server Stopped!");
        }

        private void ondisconnect(object s, ServerDisconnectedEventArgs e)
        {
        }

        private void onconnect(object s, ServerConnectedEventArgs e)
        {
			Console.WriteLine("Client Connected! INFO : " + e.Client.ToString());
		
        }

		static List<ushort> openServers = new List<ushort>();

		static List<ushort> avaiblePlayers = new List<ushort>();

		[MessageHandler((ushort)MMCodes.SERVER_OPEN, 1)]
        private static void NewOpenServer(ushort fromuser, Message message)
        {
            Console.WriteLine("New Server Opened!");
			openServers.Add(fromuser);
        }

		[MessageHandler((ushort)MMCodes.SERVER_CLOSED, 1)]
        private static void ClosedServer(ushort fromuser, Message message)
        {
            Console.WriteLine("One Server is Closed!");
			openServers.Remove(fromuser);
        }

		[MessageHandler((ushort)MMCodes.PLAYER_JOIN, 1)]
		private static void NewPlayer(ushort fromuser, Message message)
		{
			Console.WriteLine("New Player Joined!");
			avaiblePlayers.Add(fromuser);

			if (openServers.Count > 0 && avaiblePlayers.Count >= 2)
			{
				ushort player1 = avaiblePlayers[0];
				ushort player2 = avaiblePlayers[1];
				ushort server = openServers[0];
				string ipToGive = GetIpFromId(server);
				SendString(ipToGive, player1, MMCodes.REDIRECT_PLAYER);
				SendString(ipToGive, player2, MMCodes.REDIRECT_PLAYER);
				avaiblePlayers.Remove(player1);
				avaiblePlayers.Remove(player2);
				openServers.Remove(server);
			}
		}

		static string GetIpFromId(ushort id)
		{
			if (server == null)
				return "";
			string hostname = server.Clients.Where(x => x.Id == id).FirstOrDefault().ToString();
			string[] split = hostname.Split(':');
			return split[0];
		}

		static Connection? GetConnectionById(ushort idToFind)
		{
			if (server == null)
				return null;
			return server.Clients.Where(x => x.Id == idToFind).FirstOrDefault();
		}


		
		[MessageHandler((ushort)MMCodes.ASK_ENEMY_NAME, 1)]
        private static void EnemyPlayerAsked(ushort fromuser, Message message)
        {
            Console.WriteLine("Enemy name asked !");
			string username = message.GetString();
			getenemy(fromuser, username);
        }

		static async void getenemy(ushort fromuser, string username) 
		{
			var PlyrSave = await DatabaseGestor.GetAllSavesNameByUsername("UsernamePlyrOne", username);
			if (PlyrSave == null) 
			{
				PlyrSave = await DatabaseGestor.GetAllSavesNameByUsername("UsernamePlyrTwo", username);
			}
			if (PlyrSave == null) 
			{
				Console.WriteLine("No save found for this user");
				SendString("NOSAVEFOUND", fromuser, MMCodes.GET_ENEMY_NAME);
				return;
			}
			string enemyname = PlyrSave.UsernamePlyrOne == username ? PlyrSave.UsernamePlyrTwo : PlyrSave.UsernamePlyrOne;
			SendString(enemyname, fromuser, MMCodes.GET_ENEMY_NAME);
		}


		public static void SendMessage(MMCodes MMCode, ushort toSend)
		{
			if (server == null) return;
			Message mes = Message.Create(MessageSendMode.Reliable, (ushort)MMCode);
			server.Send(mes,toSend);
		}

        public static void SendString(string stringToSend, ushort toSend, MMCodes MMCode)
        {
            if (server == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)MMCode);
            mes.Add(stringToSend);
            server.Send(mes,toSend, true);
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
	}

}

