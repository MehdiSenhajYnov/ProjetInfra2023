using System;
namespace GameNetClient
{


    public class Game 
    {
        public static Character PlayerOne;
        public static Character PlayerTwo; 
        public static Character PlyrRound = null;

        public static int round = 0;
        public static int PlyrId = 0;
        public static string[] shieldASCII = File.ReadAllLines("Game/shieldimg.txt");
        public static string[] mageASCII = File.ReadAllLines("Game/mageimg.txt");
        public static string[] paladinASCII = File.ReadAllLines("Game/paladinimg.txt");
        public static string plyrOneName = "";
        public static string plyrTwoName = "";
        

        public static void GameBegin () {
            
            Console.Clear();
            var client = new NetClient();
            client.ConnectToServer();


            
            

            
            /*PlayerOne = Plyrchoice(plyrOneName);
            PlayerTwo = Plyrchoice(plyrTwoName);*/


            //ConsoleGameLoop();
            while (true)
            {
                
            }
            client.Exit();
        }


        public static void GameOver (string GameState) {

        }


        public static void SetName(int plyrToSet) 
        {
            Console.WriteLine("THE GAME BEGINS");

            if (plyrToSet == 1) {
                Console.WriteLine("WHAT IS YOUR NAME PLAYER ONE ?");
                plyrOneName = GetName();
                PlyrId = 1;
            } else if (plyrToSet == 2) {
                Console.WriteLine("WHAT IS YOUR NAME PLAYER TWO ?");
                plyrTwoName = GetName();
                PlyrId = 2;
            }
        }

        public static string GetName() 
        {
            string plyrName = Console.ReadLine();
            if (plyrName != null && plyrName != "")
            {
                return plyrName;
            } else return GetName();
        }

        /*public static void GameLoop() {
            while (PlayerOne.Health > 0 && PlayerTwo.Health > 0)
            {
                Console.Clear();
                Console.WriteLine(PlayerOne.ToString());
                Console.WriteLine(PlayerTwo.ToString());

                if (round == 0) { 
                    PlayerOne.Special();
                    PlyrRound = PlayerOne;
                    DisplayPlyr(PlayerOne, plyrOneName);
                } 
                else if (round == 1) {
                    PlayerTwo.Special();
                    PlyrRound = PlayerTwo;
                    DisplayPlyr(PlayerTwo, plyrTwoName);
                }
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
                
                PlyrChoseMove();

                if (round == 0) 
                { 
                    round = 1;
                }
                else if (round == 1) 
                {
                    round = 0;
                }
            }
            if (PlayerOne.Health == 0 && PlayerTwo.Health > 0) {
                Console.WriteLine(plyrTwoName + " WIN!");
            } else if(PlayerOne.Health > 0 && PlayerTwo.Health == 0) {
                Console.WriteLine(plyrOneName + " WIN!");
            } else if (PlayerOne.Health == 0 && PlayerTwo.Health == 0) {
                Console.WriteLine("DRAW!");
            }
        }*/

        /*public static void PlyrChoseMove() {
            int PlyrInput = choiceOneOrTwo();
            Console.WriteLine("Your target?");
            Console.WriteLine("1 : PlayerOne");
            Console.WriteLine("2 : PlayerTwo");
            int CiblePlyr = choiceOneOrTwo();
            if (round == 0) 
            {
                if (PlyrInput == 1) {
                    if (CiblePlyr == 1) {
                        PlayerOne.CibledSpecial(PlayerOne);
                    }
                    else if(CiblePlyr == 2) {
                        PlayerOne.CibledSpecial(PlayerTwo);
                    }
                }
                else if(PlyrInput == 2) {
                    if (CiblePlyr == 1) {
                        PlayerOne.AlternatifAtk(PlayerOne);
                    }
                    else if(CiblePlyr == 2) {
                        PlayerOne.AlternatifAtk(PlayerTwo);
                    }
                }

            } else if (round == 1) 
            {
                if (PlyrInput == 1) {
                    if (CiblePlyr == 1) {
                        PlayerTwo.CibledSpecial(PlayerOne);
                    }
                    else if(CiblePlyr == 2) {
                        PlayerTwo.CibledSpecial(PlayerTwo);
                    }
                }
                else if(PlyrInput == 2) {
                    if (CiblePlyr == 1) {
                        PlayerTwo.AlternatifAtk(PlayerOne);
                    }
                    else if(CiblePlyr == 2) {
                        PlayerTwo.AlternatifAtk(PlayerTwo);
                    }
                }
            }
        }*/

        public static int PlyrGetAtkInput(byte TypeOfPlyrInput) {

            Console.Clear();
            Console.WriteLine(PlayerOne.ToString());
            Console.WriteLine(PlayerTwo.ToString());
        

            if (PlyrId == 1) {
                DisplayPlyr(TypeOfPlyrInput, plyrOneName);
                DisplayAtkChoiseOfCharacter(TypeOfPlyrInput);
            } else if (PlyrId == 2) {
                DisplayPlyr(TypeOfPlyrInput, plyrTwoName);
                DisplayAtkChoiseOfCharacter(TypeOfPlyrInput);
            }
            
            return choiceOneOrTwo();
        }



        public static int CibleOfAtkInput() {
            Console.WriteLine("Your target?");
            Console.WriteLine("1 : PlayerOne");
            Console.WriteLine("2 : PlayerTwo");
            return choiceOneOrTwo();
        }

        public static int choiceOneOrTwo() {
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
                return choiceOneOrTwo();
            }
        }

        public static byte PlyrChoice(string PlyrName) {
            for (int i = 0; i < shieldASCII.Length && i < mageASCII.Length && i < paladinASCII.Length; i++)
            {
                Console.WriteLine(shieldASCII[i] + mageASCII[i] + paladinASCII[i]);
            }
            Console.WriteLine(PlyrName + " CHOOSE YOUR CHARACTER");
            Console.WriteLine("1 : Warrior");
            Console.WriteLine("2 : Cleric");
            Console.WriteLine("3 : Paladin");
            string plyrInput = "";
            plyrInput = Console.ReadLine();
            plyrInput = plyrInput != null ? plyrInput : plyrInput = "";
            if (plyrInput == "1") {
                Console.WriteLine("Your choice : Warrior\n"); 
                return 1;
            }
            else if(plyrInput == "2") {
                Console.WriteLine("Your choice : Cleric\n"); 
                return 2;
            }
            else if(plyrInput == "3") {
                Console.WriteLine("Your Choice : Paladin\n"); 
                return 3;
            }
            else {
                Console.WriteLine("Not valid input\n");
                return PlyrChoice(PlyrName);    
            }

        }

        public static void DisplayPlyr(byte plyrToDisplay, string PlyrName) {
            if (plyrToDisplay == 1) {
                Console.WriteLine(PlyrName + '\n');
                PrintWarrior();
            } else if (plyrToDisplay == 2) {
                Console.WriteLine(PlyrName + '\n');
                PrintCleric();
            } else if (plyrToDisplay == 3) {
                Console.WriteLine(PlyrName + '\n');
                PrintPaladin();
            }
        }


        public static void UpdatePlyrsFromServer(byte[] byteInfo) {
            // PTD = Player to display
            
            if (byteInfo[0] == 1) {
                PlayerOne = new Warrior(plyrOneName, (int)byteInfo[1]);
            }
            if (byteInfo[0] == 2) {
                PlayerOne = new Cleric(plyrOneName, (int)byteInfo[1]);
            }
            if (byteInfo[0] == 3) {
                PlayerOne = new Paladin(plyrOneName, (int)byteInfo[1]);
            }
            PlayerOne.TakeDamage((int)(byteInfo[1] - byteInfo[2]));
            PlayerOne.SetUniqueValue(byteInfo[3]);

            if (byteInfo[4] == 1) {
                PlayerTwo = new Warrior(plyrTwoName, (int)byteInfo[5]);
            }
            if (byteInfo[4] == 2) {
                PlayerTwo = new Cleric(plyrTwoName, (int)byteInfo[5]);
            }
            if (byteInfo[4] == 3) {
                PlayerTwo = new Paladin(plyrTwoName, (int)byteInfo[5]);
            }
            PlayerTwo.TakeDamage((int)(byteInfo[5] - byteInfo[6]));
            PlayerTwo.SetUniqueValue(byteInfo[7]);
        }


        public static void DisplayAtkChoiseOfCharacter(byte characterToUse) {
            Console.WriteLine("\n What Do you want to do?");
            if (characterToUse == 1) {
                Console.WriteLine("1 : BaseAttack : 25 damage, if Bravery is active 15 supply damage");
                Console.WriteLine("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
            }
            if (characterToUse == 2) {
                Console.WriteLine("1 : BaseAttack : +15 hp");
                Console.WriteLine("2 : AlternatifAttack : You will inflict a demage equal to the half of your mana, but -80 of your mana");
            }
            if (characterToUse == 3) {
                Console.WriteLine("1 : BaseAttack : You will inflict damage equal to 25 + your buff, buff +3 (15 max)");
                Console.WriteLine("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
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