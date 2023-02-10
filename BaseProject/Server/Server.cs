using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace GameNetServer
{

    public class Server
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
            Console.WriteLine("Server setup complete");
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
}

public class Player {
    public string PlayerIP = "";
    public int ID;
}