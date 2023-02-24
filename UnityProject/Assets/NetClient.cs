using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameNetClient {
    public class NetClient : MonoBehaviour
    {
        public Game game;

        public int myID;
        public const int Port = 7777;
        public readonly Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public bool hasLoggedin;
        //public int dataSent;

        public static Dictionary<ActionCodes, byte> Actions = new Dictionary<ActionCodes, byte>();

        //public bool WaitingSomeData = false;

        public byte[] actionSent = new byte[1];

        public List<byte> ActionsQueue = new List<byte>();

        public Button ConnectBtn;

        private void Start()
        {
            int counter = 1;
            foreach (ActionCodes oneAction in Enum.GetValues(typeof(ActionCodes)))
            {
                Actions.Add(oneAction, (byte)counter);
                counter++;
            }
            Actions[ActionCodes.GetOtherName] = 48;

            ConnectBtn.onClick.AddListener(ConnectToServer);
        }

        public void ConnectToServer()
        {
            int attempts = 0;

            try
            {
                attempts++;
                string ip = "127.0.0.1";
                Debug.Log("Connection attempt to " + ip + ":"  + Port + "...");
                var address = IPAddress.Parse(ip);
                clientSocket.Connect(address, Port);
            }
            catch (SocketException e)
            {
                Debug.Log(e.Message);
                return;
            }

            SendLoginPacket();
            Utilities.Debugger("Connected!");
            
            
            return;
        }

        public void SendLoginPacket()
        {
            if (!hasLoggedin)
            {
                SendString("NewUserConnected");
                hasLoggedin = true;
            }

            //WaitingSomeData = true;
        }


        public void Exit()
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            Environment.Exit(0);
        }

        public void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            //Utilities.Debugger("Sent To Server : " + text);
            clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
            //dataSent++;
            //Utilities.Debugger(dataSent);
        }

        private void Update()
        {
            ReceiveResponse();
            PlayForVariable();
        }

        public void ReceiveResponse()
        {
            if (clientSocket == null || !clientSocket.Connected) return;
            var buffer = new byte[2048];
            int received = clientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0)
            {
                return;
            }

            var data = new byte[received];
            Array.Copy(buffer, data, received);
            
            // Nom du joueur adveresaire
            if (data[0] == 48 && data.Length > 1) {
                string text = Encoding.ASCII.GetString(data.Skip(1).Take(data.Length - 1).ToArray());
                if (game.plyrOneName == "") {
                    game.plyrOneName = text;
                    Utilities.Debugger("Your enemy is : " + game.plyrOneName);
                } else if (game.plyrTwoName == "") {
                    game.plyrTwoName = text;
                    Utilities.Debugger("Your enemy is : " + game.plyrTwoName);
                }
                return;
            }
            

            if (data[0] == 5) {
                game.UpdatePlyrsFromServer(data.Skip(1).ToArray());

                //int PlyrInput = game.PlyrGetAtkInput((byte)(myID == 1 ? data[1] : data[5]));
                //int CiblePlyr = game.CibleOfAtkInput();
                //SendByte(new byte[]{4, (byte)PlyrInput, (byte)CiblePlyr});

                return;
            }

            if (data[0] == 6) {
                game.UpdatePlyrsFromServer(data.Skip(1).ToArray());
                Utilities.Debugger(game.PlayerOne.ToString());
                Utilities.Debugger(game.PlayerTwo.ToString());
                Utilities.Debugger("Waiting for " + (myID == 1 ? game.plyrTwoName : game.plyrOneName) + " play ...");
                return;
            }

            // ActionsQueue
            if (ActionsQueue.Count == 0) {
                foreach (var oneByte in data) {
                    ActionsQueue.Add(oneByte);
                } 
            } else {
                foreach (var oneByte in data) {
                    ActionsQueue.Add(oneByte);
                }
            }
        }

        public void PlayForVariable() {

            if (ActionsQueue.Count == 0) return;
            DoActionFromByte(ActionsQueue[0]);
            ActionsQueue.RemoveAt(0);
        }
        
        public void DoActionFromByte(byte oneByte) {
            if ( oneByte == Actions[ActionCodes.SetIdToOne]) {
                myID = 1;
            }
            if ( oneByte == Actions[ActionCodes.SetIdToTwo]) {
                myID = 2;
            }
            if ( oneByte == Actions[ActionCodes.SetplyrName]) {
                game.SetName(myID);
                

                // Send Name to server
                byte[] PlyrNameByte = Encoding.ASCII.GetBytes(((byte)0).ToString() + myID.ToString() +(myID == 1 ? game.plyrOneName : game.plyrTwoName));
                clientSocket.Send(PlyrNameByte);
                //Utilities.Debugger("Sended Name To Server");
                //WaitingSomeData = false;
            }
            if ( oneByte == Actions[ActionCodes.PlyrChoice]) {
                byte PlyrChosen = game.PlyrChoice(myID == 1 ? game.plyrOneName : game.plyrTwoName);
                SendByte(new byte[]{PlyrChosen, (byte)myID});
            }
            if ( oneByte == Actions[ActionCodes.PlyrOneWinner]) {
                Utilities.Debugger(game.plyrOneName + " WIN!");
                Exit();
            }
            if ( oneByte == Actions[ActionCodes.PlyrTwoWinner]) {
                Utilities.Debugger(game.plyrTwoName + " WIN!");
                Exit();
            }
            if ( oneByte == Actions[ActionCodes.DrawMatch]) {
                Utilities.Debugger("DRAW!");
                Exit();
            }
        }

        private void SendByte(byte byby) {
            actionSent[0] = byby;
            clientSocket.Send(actionSent);
            //Utilities.Debugger($"SENDED : {byby}   " + clientSocket.RemoteEndPoint);
        }

        private void SendByte(byte[] byby) {
            clientSocket.Send(byby);
            //Utilities.Debugger($"SENDED : {byby}   " + clientSocket.RemoteEndPoint);
        }
    }

    public enum ActionCodes {
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
}