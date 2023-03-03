using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System;
using System.Text;
using TMPro;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameNetClient
{
    public class NetworkManager : MonoBehaviour
    {
        public Game game;

        public readonly Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public bool hasLoggedin;
        public const int Port = 7777;
        public int myID;
        public static Dictionary<ActionCodes, byte> Actions = new Dictionary<ActionCodes, byte>();
        public byte[] actionSent = new byte[1];
        public List<byte> ActionsQueue = new List<byte>();

        public List<GameObject> screensParent = new List<GameObject>();

        public Button ConnectBtn;

        // Login Screen
        public Button LoginBtn;
        public TMP_InputField userNameInput;

        CancellationTokenSource tokenSource;

        // Start is called before the first frame update
        void Start()
        {
            tokenSource = new CancellationTokenSource();
            int counter = 1;
            foreach (ActionCodes oneAction in Enum.GetValues(typeof(ActionCodes)))
            {
                Actions.Add(oneAction, (byte)counter);
                counter++;
            }
            Actions[ActionCodes.GetOtherName] = 48;

            showScreen(0);


            ConnectBtn.onClick.AddListener(ConnectToServer);
            LoginBtn.onClick.AddListener(SendLoginPacket);



            Utilities.DesactivateDebugger();
            Task.Run(() =>
            {
                ReceiveResponse();
            }, tokenSource.Token);
        }



        public async void ConnectToServer()
        {
            Debug.Log("I will try to connect");
            string ip = "127.0.0.1";
            var address = IPAddress.Parse(ip);
            Debug.Log("Connection attempt to " + ip + ":" + Port + "...");

            await Task.Run(() =>
            {
                try
                {
                    clientSocket.Connect(address, Port);
                    showScreen(1);
                    Utilities.Debugger("Connected!");
                    Debug.Log("Connected");
                }
                catch (SocketException e)
                {
                    Debug.Log("Connexion Echoué!");
                    Debug.Log(e.Message);
                    return;
                }
            }, tokenSource.Token);

            
        }

        public void SendLoginPacket()
        {
            if (userNameInput.text == "")
            {
                Utilities.Debugger("Choisis un nom valide !");
                return;
            }
            if (!hasLoggedin)
            {
                SendString("NewUserConnected");
                hasLoggedin = true;
            }

        }

        void Update()
        {
            //ReceiveResponse();
        }

        public void ReceiveResponse()
        {
            while(true)
            {
                if (clientSocket == null || !clientSocket.Connected) continue;
                var buffer = new byte[2048];
                int received = clientSocket.Receive(buffer, SocketFlags.None);
                if (received == 0)
                {
                    continue;
                }

                var data = new byte[received];
                Array.Copy(buffer, data, received);

            
                // Nom du joueur adveresaire
                if (data[0] == 48 && data.Length > 1)
                {
                    string text = Encoding.ASCII.GetString(data.Skip(1).Take(data.Length - 1).ToArray());
                    if (game.plyrOneName == "")
                    {
                        game.plyrOneName = text;
                        Utilities.Debugger("Your enemy is : " + game.plyrOneName);
                    }
                    else if (game.plyrTwoName == "")
                    {
                        game.plyrTwoName = text;
                        Utilities.Debugger("Your enemy is : " + game.plyrTwoName);
                    }
                    continue;
                }

                /*
                if (data[0] == 5)
                {
                    Game.UpdatePlyrsFromServer(data.Skip(1).ToArray());
                    int PlyrInput = Game.PlyrGetAtkInput((byte)(myID == 1 ? data[1] : data[5]));
                    int CiblePlyr = Game.CibleOfAtkInput();
                    SendByte(new byte[] { 4, (byte)PlyrInput, (byte)CiblePlyr });

                    return;
                }

                if (data[0] == 6)
                {
                    Game.UpdatePlyrsFromServer(data.Skip(1).ToArray());
                    Console.Write("\n");
                    Console.WriteLine(Game.PlayerOne.ToString());
                    Console.WriteLine(Game.PlayerTwo.ToString());
                    Console.WriteLine("Waiting for " + (myID == 1 ? Game.plyrTwoName : Game.plyrOneName) + " play ...");
                    return;
                }
                 */

                // ActionsQueue
                if (ActionsQueue.Count == 0)
                {
                    foreach (var oneByte in data)
                    {
                        ActionsQueue.Add(oneByte);
                    }
                    PlayForVariable();
                }
                else
                {
                    foreach (var oneByte in data)
                    {
                        ActionsQueue.Add(oneByte);
                    }
                }

            }
        }

        public void PlayForVariable()
        {
            while (true)
            {
                if (ActionsQueue.Count == 0) return;
                DoActionFromByte(ActionsQueue[0]);
                ActionsQueue.RemoveAt(0);
            }
        }

        public void DoActionFromByte(byte oneByte)
        {
            if (oneByte == Actions[ActionCodes.SetIdToOne])
            {
                myID = 1;
            }
            if (oneByte == Actions[ActionCodes.SetIdToTwo])
            {
                myID = 2;
            }
            if (oneByte == Actions[ActionCodes.SetplyrName])
            {
                game.SetName(myID);


                // Send Name to server
                byte[] PlyrNameByte = Encoding.ASCII.GetBytes(((byte)0).ToString() + myID.ToString() + (myID == 1 ? game.plyrOneName : game.plyrTwoName));
                clientSocket.Send(PlyrNameByte);
                //Console.WriteLine("Sended Name To Server");
            }
            if (oneByte == Actions[ActionCodes.PlyrChoice])
            {
                byte PlyrChosen = game.PlyrChoice(myID == 1 ? game.plyrOneName : game.plyrTwoName);
                SendByte(new byte[] { PlyrChosen, (byte)myID });
            }
            if (oneByte == Actions[ActionCodes.PlyrOneWinner])
            {
                Debug.Log(game.plyrOneName + " WIN!");
                Exit();
            }
            if (oneByte == Actions[ActionCodes.PlyrTwoWinner])
            {
                Debug.Log(game.plyrTwoName + " WIN!");
                Exit();
            }
            if (oneByte == Actions[ActionCodes.DrawMatch])
            {
                Debug.Log("DRAW!");
                Exit();
            }
        }


        public void Exit()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private void SendByte(byte[] byby)
        {
            clientSocket.Send(byby);
        }

        public void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            //Utilities.Debugger("Sent To Server : " + text);
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            //dataSent++;
            //Utilities.Debugger(dataSent);
        }


        public enum ActionCodes
        {
            SetIdToOne = 1,
            SetIdToTwo = 2,
            SetplyrName = 3,
            PlyrChoice = 4,
            PlyrMove = 5,
            PostAttaqueInfo = 6,
            PlyrOneWinner = 7,
            PlyrTwoWinner = 8,
            DrawMatch = 9,
            GetOtherName = 48,
        }

        public void showScreen(int indexOfScreen)
        {
            for(int i = 0; i < screensParent.Count; i++)
            {
                if (i == indexOfScreen)
                {
                    screensParent[i].SetActive(true);
                } else
                {
                    screensParent[i].SetActive(false);
                }
            }
        }

        private void OnDisable()
        {
            tokenSource.Cancel();
            if (clientSocket.Connected)
            {
                clientSocket.Disconnect(false);
            }
            clientSocket.Close();

        }
    }
}