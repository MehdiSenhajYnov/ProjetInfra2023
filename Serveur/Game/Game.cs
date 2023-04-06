using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Serveur;
using MatchMakingServer;
namespace GameNetServer
{


    public class Game 
    {
        public MatchMakingClient matchMakingClient;
        public Game(MatchMakingClient newMatchMakingClient)
        {
            matchMakingClient = newMatchMakingClient;
            SaveID = "";
        }
        public string SaveID;
        public Game(MatchMakingClient newMatchMakingClient, string newSaveID)
        {
            matchMakingClient = newMatchMakingClient;
            SaveID = newSaveID;

            // Load Save
        }
        public Character? PlayerOne;
        public Character? PlayerTwo; 
        public Character? PlyrRound = null;

        public byte PlyrOneTypeCharacter = 0;
        public byte PlyrTwoTypeCharacter = 0;

        public int round = 1;
        public string[] shieldASCII = File.ReadAllLines("Game/shieldimg.txt");
        public string[] mageASCII = File.ReadAllLines("Game/mageimg.txt");
        public string[] paladinASCII = File.ReadAllLines("Game/paladinimg.txt");
        public string plyrOneName = "";
        public string plyrTwoName = "";

        public GameServer ? server {get; set; }

        public async void LoadGame(byte[] playersData, int oldRound, string oldSaveID, string oldPlyrOneName, string oldPlyrTwoName) {
            Console.WriteLine("Loading Game ...");
            // Load Save
            // playersData is like new byte[]{PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};
            PlyrOneTypeCharacter = playersData[0];
            PlyrTwoTypeCharacter = playersData[4];
            if (PlyrOneTypeCharacter == 1) {
                PlayerOne = new Warrior();
                ((Warrior)PlayerOne).Bravery = playersData[3] == 1;
            } else if (PlyrOneTypeCharacter == 2) {
                PlayerOne = new Cleric();
                ((Cleric)PlayerOne).Mana = playersData[3];
            } else if (PlyrOneTypeCharacter == 3) {
                PlayerOne = new Paladin();
                ((Paladin)PlayerOne).Buff = playersData[3];
            }

            if (PlyrTwoTypeCharacter == 1) {
                PlayerTwo = new Warrior();
                ((Warrior)PlayerTwo).Bravery = playersData[7] == 1;
            } else if (PlyrTwoTypeCharacter == 2) {
                PlayerTwo = new Cleric();
                ((Cleric)PlayerTwo).Mana = playersData[7];
            } else if (PlyrTwoTypeCharacter == 3) {
                PlayerTwo = new Paladin();
                ((Paladin)PlayerTwo).Buff = playersData[7];
            }

            PlayerOne.MaxHealth = playersData[1];
            PlayerOne.Health = playersData[2];
            PlayerTwo.MaxHealth = playersData[5];
            PlayerTwo.Health = playersData[6];

            round = oldRound;
            GameLoaded = true;
            SaveID = oldSaveID;

            plyrOneName = oldPlyrOneName;
            plyrTwoName = oldPlyrTwoName;

            Console.WriteLine("Game Loaded");
        }

        public bool GameLoaded = false;

        public async Task GameBegin () {

            server = new GameServer(this);
            server.SetupServer();

            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("SERVER_OPEN Sended To Matchaking Server");
            Console.WriteLine("---------------------------------------------------------------------");
            MatchMakingClient.SendMessage(MMCodes.SERVER_OPEN);

            //WaitingWhile(!TwoPlayersConnected);            
            
            if (!GameLoaded) {
                while (!TwoPlayersConnected)
                {
                    Thread.Sleep(250);
                    if (GameLoaded) break;
                }
            }

            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("SERVER_CLOSED Sended To Matchaking Server");
            Console.WriteLine("---------------------------------------------------------------------");
            MatchMakingClient.SendMessage(MMCodes.SERVER_CLOSED);

            if (!GameLoaded) 
            {
                Console.WriteLine("Two players Connected GameBegin");
            
                //WaitingWhile(!PlayersChoseName);

                while (!PlayersChoseName)
                {
                    Thread.Sleep(250);
                    if (GameLoaded) break;
                }

                GameServer.SendStringInt(plyrTwoName, PlyrTwoTypeCharacter, GameServer.AllPlayers.Keys.ElementAt(0), whatmessage.OtherClientName);
                GameServer.SendStringInt(plyrOneName, PlyrOneTypeCharacter, GameServer.AllPlayers.Keys.ElementAt(1), whatmessage.OtherClientName);
            }

            await GameLoop();

        }

        bool TwoPlayersConnected => GameServer.AllPlayers.Count == 2;

        bool PlayersChoseName => plyrOneName != "" && plyrTwoName != "" && PlayerOne != null && PlayerTwo != null;

        public byte[] SendToPlyrChoseMove = new byte[] {5};
        public byte[]? SendToPlyrCharInfo;
        //public List<byte> SendToPlyrCharInfoList = new List<byte>();
        public bool inGame;
        public async Task GameLoop() {
            Console.WriteLine("Begin GameLoop");

            SendToPlyrCharInfo = new byte[]{PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};
            int LastElement;
            if (GameServer.AllPlayers.Keys.Count >= 2) {
                LastElement = round-1;
            } else {
                LastElement = 0;
            }

            //GameServer.SendBytes(SendToPlyrCharInfo, GameServer.AllPlayers.Keys.ElementAt(LastElement), whatmessage.UpdateAndWait);
            GameServer.SendBytesToAll(SendToPlyrCharInfo, whatmessage.UpdateAndWait);
            Console.WriteLine("Before While : " + PlayerOne?.Health + " " + PlayerTwo?.Health);
            inGame = true;
            while (inGame && PlayerOne?.Health > 0 && PlayerTwo?.Health > 0)
            {
                /*Console.WriteLine(PlayerOne.ToString());
                Console.WriteLine(PlayerTwo.ToString());*/

                Console.WriteLine("Play at round " + round);

                if (round == 1) { 
                    PlayerOne.Special();
                    PlyrRound = PlayerOne;
                    //DisplayPlyr(PlayerOne, plyrOneName);
                } 
                else if (round == 2) {
                    PlayerTwo.Special();
                    PlyrRound = PlayerTwo;
                    //DisplayPlyr(PlayerTwo, plyrTwoName);
                }
                /*
                Console.WriteLine("\n What Do you want to do?");
                if (PlyrRound is Warrior) {
                    Console.WriteLine("1 : BaseAttack : 25 damage, if Bravery is active 15 supply damage");
                    Console.WriteLine("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
                }
                if (PlyrRound is Cleric) {
                    Console.WriteLine("1 : BaseAttack : +15 hp");
                    Console.WriteLine("2 : AlternatifAttack : You will inflict a demage equal to the half of your mana, but -80 of your mana");
                }
                if (PlyrRound is Paladin) {
                    Console.WriteLine("1 : BaseAttack : You will inflict damage equal to 25 + your buff, buff +3 (15 max)");
                    Console.WriteLine("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
                }
                */
                SendToPlyrCharInfo = new byte[]{PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};

                try
                {
                    GameServer.SendBytes(SendToPlyrCharInfo, GameServer.AllPlayers.Keys.ElementAt(round-1), whatmessage.AskPlayerMove);
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Cannot send to player");
                }


                while (!PlyrHasChoseMove) {
                    if (!inGame) break;
                }
                if (!inGame) break;
                PlyrChoseMove();

                SendToPlyrCharInfo = new byte[]{PlyrOneTypeCharacter, (byte)PlayerOne.MaxHealth, (byte)PlayerOne.Health, (byte)PlayerOne.GetUniqueValue() , PlyrTwoTypeCharacter, (byte)PlayerTwo.MaxHealth, (byte)PlayerTwo.Health, (byte)PlayerTwo.GetUniqueValue()};
                //GameServer.SendBytes(SendToPlyrCharInfo, GameServer.AllPlayers.Keys.ElementAt(LastElement), whatmessage.UpdateAndWait);
                GameServer.SendBytesToAll(SendToPlyrCharInfo, whatmessage.UpdateAndWait);


                if (round == 1) 
                { 
                    round = 2;
                }
                else if (round == 2) 
                {
                    round = 1;
                }

                Console.WriteLine(PlayerOne.Health+ " " + PlayerTwo.Health);

                //ENABLE IF SAVE SYSTEM WORK SaveID = await DatabaseGestor.AddSave(plyrOneName, plyrTwoName, SendToPlyrCharInfo, SaveID, round);
                if (GameLoaded) {
                    Console.WriteLine("GameLoaded, Saved with parameters : " + plyrOneName + " " + plyrTwoName + " " + SendToPlyrCharInfo + " " + SaveID + " " + round);
                    
                    server?.CloseAllSockets();
                    return;
                    
                }

                string plyrOneWinMessage = "";
                string plyrTwoWinMessage = "";
                // WIN CONDITION
                if (PlayerOne?.Health == 0 && PlayerTwo?.Health > 0) {
                    Console.WriteLine(plyrTwoName + " WIN!");
                    //GameServer.SendStringToAll(plyrTwoName + " WIN!", whatmessage.Winner);
                    plyrOneWinMessage = "YOU LOSE!";
                    plyrTwoWinMessage = "YOU WIN!";
                    SendWinner(plyrOneWinMessage, plyrTwoWinMessage);
                    break;
                } else if(PlayerOne?.Health > 0 && PlayerTwo?.Health == 0) {
                    Console.WriteLine(plyrOneName + " WIN!");
                    //GameServer.SendStringToAll(plyrOneName + " WIN!", whatmessage.Winner);
                    plyrOneWinMessage = "YOU WIN!";
                    plyrTwoWinMessage = "YOU LOSE!";
                    SendWinner(plyrOneWinMessage, plyrTwoWinMessage);
                    break;
                } else if (PlayerOne?.Health == 0 && PlayerTwo?.Health == 0) {
                    Console.WriteLine("DRAW!");
                    GameServer.SendStringToAll("DRAW!", whatmessage.Winner);
                    break;
                }



            }

            Console.WriteLine("Fini!");
            server?.CloseAllSockets();


        }

        public static void SendWinner(string plyrOneWinMessage, string plyrTwoWinMessage) {
            if (plyrOneWinMessage != "" && GameServer.AllPlayers.ContainsValue(1))
            {
                GameServer.SendString(plyrOneWinMessage, GetKeyFromValue(GameServer.AllPlayers,1), whatmessage.Winner);
            }
            if (plyrTwoWinMessage != "" && GameServer.AllPlayers.ContainsValue(2))
            {
                GameServer.SendString(plyrTwoWinMessage, GetKeyFromValue(GameServer.AllPlayers,2), whatmessage.Winner);
            }

            GameServer.currentGame.inGame = false;

        }

        public int PlyrMoveInput = -1;
        public int CibleMovePlyr = -1;
        public bool PlyrHasChoseMove = false;

        public void PlyrChoseMove() {
            Console.WriteLine("Your target?");
            Console.WriteLine("1 : PlayerOne");
            Console.WriteLine("2 : PlayerTwo");

           

            if (round == 1) 
            {
                if (PlayerOne == null || PlayerTwo == null) return;
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
                if (PlayerOne == null || PlayerTwo == null) return;
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

        public int choiseOneOrTwo() {
            string? PlyrInput = "";
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

        public void PlyrChoice(byte PlyrChosen, byte plyrID) {
            Console.WriteLine("Player " + plyrID + " choose the Character " + PlyrChosen);
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

        public void DisplayPlyr(Character plyrToDisplay, string PlyrName) {
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

        public void PrintWarrior() {
            for (int i = 0; i < shieldASCII.Length; i++)
            {
                Console.WriteLine(shieldASCII[i]);
            }
        }
        public void PrintCleric() {
            for (int i = 0; i < mageASCII.Length; i++)
            {
                Console.WriteLine(mageASCII[i]);
            }
        }
        public void PrintPaladin() {
            for (int i = 0; i < paladinASCII.Length; i++)
            {
                Console.WriteLine(paladinASCII[i]);
            }
        }

        public static ushort GetKeyFromValue(SortedDictionary<ushort, int> dictionary, int value)
        {
            var result = dictionary.FirstOrDefault(x => x.Value == value);
            return result.Key;
        }
    }
    
}