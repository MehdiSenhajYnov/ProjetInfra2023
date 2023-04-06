using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Riptide;
using Riptide.Utils;

namespace GameNetServer
{

    public class GameServer
    {
        public const int BufferSize = 2048;
        public const int Port = 7777;
        public readonly List<Socket> clientSockets = new List<Socket>();
        public readonly byte[] buffer = new byte[BufferSize];
        //public readonly List<Player> players = new List<Player>();
        //public static List<int> AllPlayers = new List<int>();
        
        public byte[] actionsCode = new byte[1];
        //public int dataSent;


        ushort port;
        public bool isRunning;
        ushort maxClientCount;

        public static Game? currentGame;
        public static SortedDictionary<ushort, int> AllPlayers = new SortedDictionary<ushort,int>();
        public static Server? server;

        public GameServer(Game newCrrentGame) {
            currentGame = newCrrentGame;
            AllPlayers.Clear();
            server = null;
        }

        public void SetupServer()
        {
            port = 7777;
            maxClientCount = 5;
            RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);
            server = new Server();
            server.Start(port, maxClientCount);
            server.ClientConnected += (s, e) => onconnect(s, e);
            server.ClientDisconnected += (s, e) => ondisconnect(s, e);
            isRunning = true;

            Task.Run(() => {
                while (isRunning)
                {
                    server.Update();
                }

                server.Stop();
            });
        }

        void ondisconnect(object? sender, EventArgs e)
        {

            ServerDisconnectedEventArgs discArg = (ServerDisconnectedEventArgs)e;
            // DISABLE THIS IF WHEN ADD SAVE SYSTEM
            Console.WriteLine("Player Disconnected : " + discArg.Client.Id + " Plyrs Count : " + AllPlayers.Count);

            if (AllPlayers.ContainsKey(discArg.Client.Id)) {
                if (AllPlayers[discArg.Client.Id] == 1) {
                    Game.SendWinner("YOU LOSE!", "YOU WIN!");
                } else if (AllPlayers[discArg.Client.Id] == 2) {
                    Game.SendWinner("YOU WIN!", "YOU LOSE!");
                }
            }

            AllPlayers.Remove(discArg.Client.Id);

            return;
        }

        void onconnect(object? sender, EventArgs e)
        {
            if (currentGame != null && currentGame.GameLoaded) return;
            ServerConnectedEventArgs connArg = (ServerConnectedEventArgs)e;


            int nb = 1;
            if (AllPlayers.ContainsValue(1)) {
                nb = 2;
            }
            Console.WriteLine("Player Connected : " + nb + " Plyrs Count : " + AllPlayers.Count);

            AllPlayers.Add(connArg.Client.Id,nb);
            SendBytes(new byte[]{(byte)nb,3,4}, (ushort)nb, whatmessage.AskPlayerName);
            //SendString("WHAT IS YOUR NAME PLAYER " + (nb == 1 ? "ONE ?" : "TWO ?"));
            string strnb = AmountInWords(nb);
            Console.WriteLine($"New Client has joined the game ID : {strnb}");

            return;
        }

        public void CloseAllSockets()
        {
            if (server == null) return;
            server.Stop();
        }

        [MessageHandler((ushort)whatmessage.welcome)]
        private static void WelcomeMessage(ushort fromuser, Message message)
        {
            string somestr = message.GetString();
            Console.WriteLine("Received : "+somestr);
            // Do stuff with the retrieved data here
        }

        [MessageHandler((ushort)whatmessage.clientname)]
        private static void GetPlayerUsername(ushort fromuser, Message message)
        {
            int idOfPlyrWhoSend = AllPlayers[fromuser];


            string newPlyrName = message.GetString();
            Console.WriteLine("USERNAME : "+ newPlyrName + " ID : " + idOfPlyrWhoSend );

            if (currentGame == null) return;
            if (currentGame.GameLoaded) {
                return;
            }

            if (idOfPlyrWhoSend == 1) {
                currentGame.plyrOneName = newPlyrName;
            } else if( idOfPlyrWhoSend == 2) {
                currentGame.plyrTwoName = newPlyrName;
            }

        }

        [MessageHandler((ushort)whatmessage.PlyrChoice)]
        private static void GetPlyrChoice(ushort fromuser, Message message)
        {
            Console.WriteLine("Received player choise");

            byte[] Data = message.GetBytes();
            int idOfPlyrWhoSend = AllPlayers[fromuser];

            currentGame?.PlyrChoice(Data[0], (byte)idOfPlyrWhoSend);

        }
    
        [MessageHandler((ushort)whatmessage.PlyrMove)]
        private static void GetPlyrMove(ushort fromuser, Message message)
        {
            int idOfPlyrWhoSend = AllPlayers[fromuser];

            Console.WriteLine("Received player move");

            byte[] Data = message.GetBytes();

            if (currentGame == null) return;
            
            if (Data[0] == 1) {
                currentGame.PlyrMoveInput = 1;
            } else if (Data[0] == 2) {
                currentGame.PlyrMoveInput = 2;
            }
            if (Data[1] == 1) {
                currentGame.CibleMovePlyr = 1;
            } else if (Data[1] == 2) {
                currentGame.CibleMovePlyr = 2;
            }
            currentGame.PlyrHasChoseMove = true;
        }


        public static void SendString(string stringToSend, ushort toReceiver, whatmessage messageSendMode)
        {
            if (server == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)messageSendMode);
            mes.Add(stringToSend);
            server.Send(mes, toReceiver);
        }

        public static void SendStringInt(string stringToSend, int intToSend, ushort toReceiver, whatmessage messageSendMode)
        {
            if (server == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)messageSendMode);
            mes.Add(stringToSend);
            mes.Add(intToSend);
            server.Send(mes, toReceiver);
        }

        public static void SendStringToAll(string stringToSend, whatmessage messageSendMode)
        {
            if (server == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)messageSendMode);
            mes.Add(stringToSend);
            server.SendToAll(mes);
        }

        public static void SendBytes(byte[] bytesToSend, ushort toReceiver, whatmessage messageSendMode)
        {
            if (server == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)messageSendMode);
            mes.Add(bytesToSend);
            server.Send(mes, toReceiver);
        }

        public static void SendBytesToAll(byte[] bytesToSend, whatmessage messageSendMode)
        {
            if (server == null) return;
            Message mes = Message.Create(MessageSendMode.Reliable, (ushort)messageSendMode);
            mes.Add(bytesToSend);
            server.SendToAll(mes);
        }

        public static string AmountInWords(double amount)
        {
            var n = (int)amount;

            if (n == 0)
                return "";
            else if (n > 0 && n <= 19)
            {
                var arr = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                return arr[n - 1] + " ";
            }
            else if (n >= 20 && n <= 99)
            {
                var arr = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
                return arr[n / 10 - 2] + " " + AmountInWords(n % 10);
            }
            else if (n >= 100 && n <= 199)
            {
                return "One Hundred " + AmountInWords(n % 100);
            }
            else if (n >= 200 && n <= 999)
            {
                return AmountInWords(n / 100) + "Hundred " + AmountInWords(n % 100);
            }
            else if (n >= 1000 && n <= 1999)
            {
                return "One Thousand " + AmountInWords(n % 1000);
            }
            else if (n >= 2000 && n <= 999999)
            {
                return AmountInWords(n / 1000) + "Thousand " + AmountInWords(n % 1000);
            }
            else if (n >= 1000000 && n <= 1999999)
            {
                return "One Million " + AmountInWords(n % 1000000);
            }
            else if (n >= 1000000 && n <= 999999999)
            {
                return AmountInWords(n / 1000000) + "Million " + AmountInWords(n % 1000000);
            }
            else if (n >= 1000000000 && n <= 1999999999)
            {
                return "One Billion " + AmountInWords(n % 1000000000);
            }
            else
            {
                return AmountInWords(n / 1000000000) + "Billion " + AmountInWords(n % 1000000000);
            }
        }
    
    }
    
    public enum ActionCodes {
        ChoosedWarrior = 1,
        ChoosedCleric = 2,
        ChoosedPaladin = 3,
        PlyrMove = 4,
    } 
}
