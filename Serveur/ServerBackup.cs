using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Riptide;
using Riptide.Utils;
using GameNetServer;
/*
namespace GameNetServerBackup
{

    public class GameServer
    {
        public const int BufferSize = 2048;
        public const int Port = 7777;
        public readonly List<Socket> clientSockets = new List<Socket>();
        public readonly byte[] buffer = new byte[BufferSize];
        //public readonly List<Player> players = new List<Player>();
        public static SortedDictionary<int, Player> AllPlayers = new SortedDictionary<int, Player>();
        public Socket serverSocket;
        public Socket current;
        public byte[] actionsCode = new byte[1];
        //public int dataSent;


        public void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("GameServer setup complete");
            Console.WriteLine("Listening on port: " + Port);
        }

        public void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }

        public void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected: " + socket.RemoteEndPoint);
            serverSocket.BeginAccept(AcceptCallback, null);
            
        }

        public void ReceiveCallback(IAsyncResult AR)
        {
            current = (Socket)AR.AsyncState;
            int received = 0;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected " + current.RemoteEndPoint);
                foreach (var plyr in AllPlayers)
                {
                    Console.WriteLine(plyr.Value.PlayerIP + "    " + current.RemoteEndPoint.ToString());

                    if (plyr.Value.PlayerIP == current.RemoteEndPoint.ToString())
                    {
                        if (plyr.Value.ID == 1) {
                            Game.plyrOneName = "";
                        }
                        if (plyr.Value.ID == 2) {
                            Game.plyrOneName = "";
                        }
                        AllPlayers.Remove(plyr.Key);
                        break;
                    }
                }
                current.Close(); // Dont shutdown because the socket may be disposed and its disconnected anyway
                Console.WriteLine("Plyrs Count3 : " + AllPlayers.Count + " " + (AllPlayers.Count > 0 ? AllPlayers.ElementAt(0).Value.ID : " "));
                clientSockets.Remove(current);
                return;
            }

            byte[] Data = new byte[received];
            Array.Copy(buffer, Data, received);

            Console.Write("Received Data ");
            foreach (var someData in Data)
            {
                Console.Write(someData + " ");
            }
            Console.Write("\n");

            if (Data.Length <= 0) {
                Console.Write("Client Disconnected");
                return;
            }

            if ( Data[0] == 1) {
                Game.PlyrChoice(Data[0], Data[1]);
            }
            if ( Data[0] == 2) {
                Game.PlyrChoice(Data[0], Data[1]);
            }
            if ( Data[0] == 3) {
                Game.PlyrChoice(Data[0], Data[1]);
            }
            if ( Data[0] == 4) {
                if (Data[1] == 1) {
                    Game.PlyrMoveInput = 1;
                } else if (Data[1] == 2) {
                    Game.PlyrMoveInput = 2;
                }
                if (Data[2] == 1) {
                    Game.CibleMovePlyr = 1;
                } else if (Data[2] == 2) {
                    Game.CibleMovePlyr = 2;
                }
                Game.PlyrHasChoseMove = true;
            }

            if (Data[0] == 48) {
                string newPlyrName = Encoding.ASCII.GetString(Data.Skip(2).Take(Data.Length - 2).ToArray());
                int idOfPlyrWhoSend = ((int)Data[1] - 48 );
                Console.WriteLine("Plyr ID : " + idOfPlyrWhoSend + " To Name : " +newPlyrName);
                if (idOfPlyrWhoSend == 1) {
                    Game.plyrOneName = newPlyrName;
                } else if( idOfPlyrWhoSend == 2) {
                    Game.plyrTwoName = newPlyrName;
                }
            }

            string text = Encoding.ASCII.GetString(Data);

            if (text.StartsWith("NewUserConnected"))
            {
                string newuser = text.Replace("newuser:", string.Empty);
                int nb = 1;
                if (AllPlayers.Keys.Contains(1))
                    nb = 2;
                Player newPlyrAdded = new Player();
                newPlyrAdded.PlayerIP = current.RemoteEndPoint.ToString(); 
                newPlyrAdded.ID = nb;
                AllPlayers.Add(nb, newPlyrAdded);
                SendByte(new byte[]{(byte)nb,3,4});
                //SendString("WHAT IS YOUR NAME PLAYER " + (nb == 1 ? "ONE ?" : "TWO ?"));
                Console.WriteLine($"New Client has joined the game ID : {nb}");
            }

            current.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
        }

        public void SendByte(byte byby) {
            actionsCode[0] = byby;
            current.Send(actionsCode);
            Console.WriteLine($"SENDED : {byby}   " + current.RemoteEndPoint);
        }

        public void SendByte(byte[] byby) {
            current.Send(byby);
            Console.WriteLine($"SENDED : {byby}   " + current.RemoteEndPoint);
        }

        public void SendString(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message.ToString());
                current.Send(data);
                // current.BeginReceive(buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
                //dataSent++;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client disconnected!" + ex.Message);
            }
            //Console.WriteLine(dataSent);
        }
    }
    
    public enum ActionCodes {
        ChoosedWarrior = 1,
        ChoosedCleric = 2,
        ChoosedPaladin = 3,
        PlyrMove = 4,
    } 

    public class Player {
        public string PlayerIP = "";
        public int ID;
    }




    public class Game 
    {
        public static Character PlayerOne;
        public static Character PlayerTwo; 
        public static Character PlyrRound = null;

        public static byte PlyrOneTypeCharacter = 0;
        public static byte PlyrTwoTypeCharacter = 0;

        public static int round = 1;
        public static string[] shieldASCII = File.ReadAllLines("Game/shieldimg.txt");
        public static string[] mageASCII = File.ReadAllLines("Game/mageimg.txt");
        public static string[] paladinASCII = File.ReadAllLines("Game/paladinimg.txt");
        public static string plyrOneName = "";
        public static string plyrTwoName = "";

        public static GameServer ? server {get; set; }

        public static void GameBegin () {


            Console.Clear();


            server = new GameServer();
            server.SetupServer();


            while (GameServer.AllPlayers.Count != 2 || plyrOneName == "" || plyrTwoName == "" || PlayerOne == null || PlayerTwo == null ) {
                Thread.Sleep(250);
                //Console.WriteLine("AllPlayers Count : " + Server.AllPlayers.Count + " plyrOneName : " + plyrOneName + " plyrTwoName : " + plyrTwoName);
            }
            
            
            Console.WriteLine("Two players Connected GameBegin");
        

            byte[] P1NByte = Encoding.ASCII.GetBytes(((byte)0).ToString() + plyrOneName);
            byte[] P2NByte = Encoding.ASCII.GetBytes(((byte)0).ToString() + plyrTwoName);


            server.clientSockets[0].SendTo(P2NByte, 0, P2NByte.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
            server.clientSockets[1].SendTo(P1NByte, 0, P1NByte.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);

            GameLoop();



        }

        public static byte[] SendToPlyrChoseMove = new byte[] {5};
        public static byte[] SendToPlyrCharInfo;
        //public static List<byte> SendToPlyrCharInfoList = new List<byte>();

        public async static void GameLoop() {
            while (PlayerOne.Health > 0 && PlayerTwo.Health > 0)
            {
                Console.Clear();
                Console.WriteLine(PlayerOne.ToString());
                Console.WriteLine(PlayerTwo.ToString());

                if (round == 1) { 
                    PlayerOne.Special();
                    PlyrRound = PlayerOne;
                    DisplayPlyr(PlayerOne, plyrOneName);
                } 
                else if (round == 2) {
                    PlayerTwo.Special();
                    PlyrRound = PlayerTwo;
                    DisplayPlyr(PlayerTwo, plyrTwoName);
                }
                byte PlyrRoundType = 0;
                Console.WriteLine("\n What Do you want to do?");
                if (PlyrRound is Warrior) {
                    PlyrRoundType = 1;
                    Console.WriteLine("1 : BaseAttack : 25 damage, if Bravery is active 15 supply damage");
                    Console.WriteLine("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
                }
                if (PlyrRound is Cleric) {
                    PlyrRoundType = 2;
                    Console.WriteLine("1 : BaseAttack : +15 hp");
                    Console.WriteLine("2 : AlternatifAttack : You will inflict a demage equal to the half of your mana, but -80 of your mana");
                }
                if (PlyrRound is Paladin) {
                    PlyrRoundType = 3;
                    Console.WriteLine("1 : BaseAttack : You will inflict damage equal to 25 + your buff, buff +3 (15 max)");
                    Console.WriteLine("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
                }

                SendToPlyrCharInfo = new byte[]{5, PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};
                if (round == 1) {
                    server.clientSockets[0].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
                } else if (round == 2) {
                    server.clientSockets[1].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);
                    
                }

                while (!PlyrHasChoseMove) {
                    
                }

                PlyrChoseMove();

                SendToPlyrCharInfo = new byte[]{6, PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};
                if (round == 1) {
                    server.clientSockets[0].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
                } else if (round == 2) {
                    server.clientSockets[1].SendTo(SendToPlyrCharInfo, 0, SendToPlyrCharInfo.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);
                    
                }

                if (round == 1) 
                { 
                    round = 2;
                }
                else if (round == 2) 
                {
                    round = 1;
                }

                Console.WriteLine(PlayerOne.Health+ " " + PlayerTwo.Health);
            }
            byte[] sendWinner = new byte[1];
            if (PlayerOne.Health == 0 && PlayerTwo.Health > 0) {
                Console.WriteLine(plyrTwoName + " WIN!");
                sendWinner[0] = 8;
            } else if(PlayerOne.Health > 0 && PlayerTwo.Health == 0) {
                Console.WriteLine(plyrOneName + " WIN!");
                sendWinner[0] = 7;
            } else if (PlayerOne.Health == 0 && PlayerTwo.Health == 0) {
                Console.WriteLine("DRAW!");
                sendWinner[0] = 9;
            }
            server.clientSockets[0].SendTo(sendWinner, 0, sendWinner.Length, SocketFlags.None, server.clientSockets[0].LocalEndPoint);
            server.clientSockets[1].SendTo(sendWinner, 0, sendWinner.Length, SocketFlags.None, server.clientSockets[1].LocalEndPoint);

            Console.WriteLine("Fini!");
            server.CloseAllSockets();

        }

        public static int PlyrMoveInput = -1;
        public static int CibleMovePlyr = -1;
        public static bool PlyrHasChoseMove = false;

        public static void PlyrChoseMove() {
            Console.WriteLine("Your target?");
            Console.WriteLine("1 : PlayerOne");
            Console.WriteLine("2 : PlayerTwo");

           

            if (round == 1) 
            {

                if (PlyrMoveInput == 1) {
                    if (CibleMovePlyr == 1) {
                        PlayerOne.CibledSpecial(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerOne.CibledSpecial(PlayerTwo);
                    }
                }
                else if(PlyrMoveInput == 2) {
                    if (CibleMovePlyr == 1) {
                        PlayerOne.AlternatifAtk(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerOne.AlternatifAtk(PlayerTwo);
                    }
                }

            } else if (round == 2) 
            {
                if (PlyrMoveInput == 1) {
                    if (CibleMovePlyr == 1) {
                        PlayerTwo.CibledSpecial(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerTwo.CibledSpecial(PlayerTwo);
                    }
                }
                else if(PlyrMoveInput == 2) {
                    if (CibleMovePlyr == 1) {
                        PlayerTwo.AlternatifAtk(PlayerOne);
                    }
                    else if(CibleMovePlyr == 2) {
                        PlayerTwo.AlternatifAtk(PlayerTwo);
                    }
                }
            }

            PlyrHasChoseMove = false;
            return;
        }

        public static int choiseOneOrTwo() {
            string PlyrInput = "";
            PlyrInput = Console.ReadLine();
            PlyrInput = PlyrInput != null ? PlyrInput : PlyrInput = "";
            if (PlyrInput == "1") {
                return 1;
            }
            else if(PlyrInput == "2") {
                return 2;
            } else {
                Console.WriteLine("Not valid input, try again");
                return choiseOneOrTwo();
            }
        }

        public static void PlyrChoice(byte PlyrChosen, byte plyrID) {
            string PlyrName = plyrID == 1 ? plyrOneName : plyrTwoName;
            if (PlyrChosen == 1) {
                if (plyrID == 1) {
                    PlayerOne = new Warrior(PlyrName, 200);
                    PlyrOneTypeCharacter = 1;
                }
                if (plyrID == 2) {
                    PlayerTwo = new Warrior(PlyrName, 200);
                    PlyrTwoTypeCharacter = 1;
                }
            }
            else if(PlyrChosen == 2) {
                if (plyrID == 1) {
                    PlayerOne = new Cleric(PlyrName, 200);
                    PlyrOneTypeCharacter = 2;
                }
                if (plyrID == 2) {
                    PlayerTwo = new Cleric(PlyrName, 200);
                    PlyrTwoTypeCharacter = 2;
                }
            }
            else if(PlyrChosen == 3) {                
                if (plyrID == 1) {
                    PlayerOne = new Paladin(PlyrName, 200);
                    PlyrOneTypeCharacter = 3;
                }
                if (plyrID == 2) {
                    PlayerTwo = new Paladin(PlyrName, 200);
                    PlyrTwoTypeCharacter = 3;
                }
            }

            Console.WriteLine(PlyrName + "  " + (PlayerOne != null ? PlayerOne.ToString() : "") + "  " + (PlayerTwo != null ? PlayerTwo.ToString() : ""));

        }

        public static void DisplayPlyr(Character plyrToDisplay, string PlyrName) {
            if (plyrToDisplay is Warrior) {
                Console.WriteLine(PlyrName + '\n');
                PrintWarrior();
            } else if (plyrToDisplay is Cleric) {
                Console.WriteLine(PlyrName + '\n');
                PrintCleric();
            } else if (plyrToDisplay is Paladin) {
                Console.WriteLine(PlyrName + '\n');
                PrintPaladin();
            }
        }

        public static void PrintWarrior() {
            for (int i = 0; i < shieldASCII.Length; i++)
            {
                Console.WriteLine(shieldASCII[i]);
            }
        }
        public static void PrintCleric() {
            for (int i = 0; i < mageASCII.Length; i++)
            {
                Console.WriteLine(mageASCII[i]);
            }
        }
        public static void PrintPaladin() {
            for (int i = 0; i < paladinASCII.Length; i++)
            {
                Console.WriteLine(paladinASCII[i]);
            }
        }
    }
    
}
*/