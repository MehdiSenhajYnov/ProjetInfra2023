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
/*
namespace GameNetClient
{
    
    public class NetworkManager : Singleton<NetworkManager>
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

 

        // Start is called before the first frame update
        void Start()
        {
            int counter = 1;
            foreach (ActionCodes oneAction in Enum.GetValues(typeof(ActionCodes)))
            {
                Actions.Add(oneAction, (byte)counter);
                counter++;
            }
            Actions[ActionCodes.GetOtherName] = 48;

            showScreen(0);


            ConnectBtn.onClick.AddListener(() => StartCoroutine(ConnectToServer()));
            LoginBtn.onClick.AddListener(SendLoginPacket);



            Utilities.DesactivateDebugger();
            //StartCoroutine(ReceiveResponse());

        }



        public IEnumerator ConnectToServer()
        {
            Debug.Log("I will try to connect");
            string ip = "127.0.0.1";
            var address = IPAddress.Parse(ip);
            Debug.Log("Connection attempt to " + ip + ":" + Port + "...");

            yield return clientSocket.ConnectAsync(address, Port);
            setBeginToReceiveEvent();
            if (clientSocket.Connected)
            {
                showScreen(1);
                Utilities.Debugger("Connected!");
            }
        }

        void setBeginToReceiveEvent()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(new byte[1024], 0, 1024);

            args.Completed += (object sender, SocketAsyncEventArgs e) =>
            {
                onReceivedData(e);
            };

            bool willRaiseEvent = clientSocket.ReceiveAsync(args);
            if (!willRaiseEvent)
            {
                // La méthode ReceiveAsync a retourné false, ce qui signifie que la réception a été complétée de manière synchrone
                onReceivedData(args);
                //args.Completed(clientSocket, args);
            }
        }

        void onReceivedData(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // Les données ont été reçues avec succès
                byte[] receivedData = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, receivedData, 0, e.BytesTransferred);
                StartCoroutine(ReceiveResponse(receivedData));
            }
            else
            {
                // Une erreur s'est produite lors de la réception
                Console.WriteLine("Erreur de réception : {0}", e.SocketError);
            }
        }

        public void onConnected()
        {

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
        }


        public IEnumerator ReceiveResponse(byte[] data)
        {

            if (clientSocket == null || !clientSocket.Connected) {
                yield break;
            }
            
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
                yield break;

            }

            if (ActionsQueue.Count == 0)
            {
                foreach (var oneByte in data)
                {
                    ActionsQueue.Add(oneByte);
                    yield return null;
                }
                yield return StartCoroutine(PlayForVariable());
            }
            else
            {
                foreach (var oneByte in data)
                {
                    ActionsQueue.Add(oneByte);
                    yield return null;
                }
            }

        }

        public IEnumerator PlayForVariable()
        {
            while (true)
            {
                if (ActionsQueue.Count == 0) yield break;
                DoActionFromByte(ActionsQueue[0]);
                ActionsQueue.RemoveAt(0);
                yield return null;
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
                game.plyrToSet = myID;
                if (myID == 1)
                {
                    Utilities.Debugger("WHAT IS YOUR NAME PLAYER ONE ?");
                } else if (myID == 2)
                {
                    Utilities.Debugger("WHAT IS YOUR NAME PLAYER TWO ?");
                }

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

        public void sendUserName()
        {
            byte[] PlyrNameByte = Encoding.ASCII.GetBytes(((byte)0).ToString() + myID.ToString() + (myID == 1 ? game.plyrOneName : game.plyrTwoName));
            clientSocket.Send(PlyrNameByte);
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
            if (clientSocket.Connected)
            {
                clientSocket.Disconnect(false);
            }
            clientSocket.Close();

        }
    }
}

*/


/*
 
        void recResp()
        {
            if (clientSocket == null || !clientSocket.Connected)
            {
                return;
            }
            var buffer = new byte[2048];
            int received = clientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0)
            {
                return;
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
                return;
            }

            if (ActionsQueue.Count == 0)
            {
                foreach (var oneByte in data)
                {
                    ActionsQueue.Add(oneByte);
                }
                StartCoroutine(PlayForVariable());
            }
            else
            {
                foreach (var oneByte in data)
                {
                    ActionsQueue.Add(oneByte);
                }
            }

        }

*/