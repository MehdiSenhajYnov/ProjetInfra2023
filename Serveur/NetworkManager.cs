
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Riptide;
using Riptide.Utils;

namespace Serveur
{
    public class NetworkManager
    {
        public static NetworkManager? Singleton;
        ushort port;
        ushort maxClientCount;
        public static void SetSingleton(NetworkManager instance)
        {
            Singleton = instance;

        }

        public NetworkManager()
        {
            SetSingleton(this);
            port = 7777;
            maxClientCount = 5;
        }


        
        public Server? server;
        public bool isRunning;
        public void Start()
        {
            Console.WriteLine("NetworkManager started");
            RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);
            server = new Server();
            server.Start(port, maxClientCount);
            server.ClientDisconnected += (s, e) => ondisconnect(s, e);
            isRunning = true;
            while (isRunning)
            {
                server.Update();
            }

            server.Stop();
        }

        void ondisconnect(object sender, EventArgs e)
        {
            Console.WriteLine("Client disconnected " + e.ToString());
            if (sender != null)
            {
                Console.WriteLine("SENDER Client disconnected " + sender.ToString());
            }
        }

        

        [MessageHandler((ushort)whatmessage.welcome)]
        private static void HandleSomeMessageFromServer(ushort fromuser, Message message)
        {
            string somestr = message.GetString();
            Console.WriteLine("Received : "+somestr);
            // Do stuff with the retrieved data here
        }

        [MessageHandler((ushort)whatmessage.clientname)]
        private static void HandleUsername(ushort fromuser, Message message)
        {
            string somestr = message.GetString();
            Console.WriteLine("USERNAME : "+somestr);
            // Do stuff with the retrieved data here
        }

    }


}

*/
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