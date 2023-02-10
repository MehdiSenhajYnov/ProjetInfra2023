using System;
namespace CSharpDiscovery.Examen
{


    public class Game 
    {
        public static Character PlayerOne;
        public static Character PlayerTwo; 
        public static Character PlyrRound = null;

        public static int round = 0;
        public static string[] shieldASCII = File.ReadAllLines("shieldimg.txt");
        public static string[] mageASCII = File.ReadAllLines("mageimg.txt");
        public static string[] paladinASCII = File.ReadAllLines("paladinimg.txt");
        public static string plyrOneName = "";
        public static string plyrTwoName = "";

        public static void GameBegin () {
            
            Console.Clear();


            Console.WriteLine("THE GAME BEGIN");
            Console.WriteLine("WHAT IS YOUR NAME PLAYER ONE?");
            plyrOneName = GetName();
            Console.WriteLine("WHAT IS YOUR NAME PLAYER TWO?");
            plyrTwoName = GetName();

            for (int i = 0; i < shieldASCII.Length && i < mageASCII.Length && i < paladinASCII.Length; i++)
            {
                Console.WriteLine(shieldASCII[i] + mageASCII[i] + paladinASCII[i]);
            }
            
            PlayerOne = PlyrChoise(plyrOneName);
            PlayerTwo = PlyrChoise(plyrTwoName);


            GameLoop();
        }

        public static string GetName() 
        {
            string plyrName = Console.ReadLine();
            if (plyrName != null && plyrName != "")
            {
                return plyrName;
            } else return GetName();
        }

        public static void GameLoop() {
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
        }

        public static void PlyrChoseMove() {
            int PlyrInput = choiseOneOrTwo();
            Console.WriteLine("Your target?");
            Console.WriteLine("1 : PlayerOne");
            Console.WriteLine("2 : PlayerTwo");
            int CiblePlyr = choiseOneOrTwo();
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

        public static Character PlyrChoise(string PlyrName) {
            Console.WriteLine(PlyrName + " CHOOSE YOUR CHARACTER");
            Console.WriteLine("1 : Warrior");
            Console.WriteLine("2 : Cleric");
            Console.WriteLine("3 : Paladin");
            string plyrInput = "";
            plyrInput = Console.ReadLine();
            plyrInput = plyrInput != null ? plyrInput : plyrInput = "";
            if (plyrInput == "1") {
                Console.WriteLine("Your Choise : Warrior\n"); 
                return new Warrior(PlyrName, 200);
            }
            else if(plyrInput == "2") {
                Console.WriteLine("Your Choise : Cleric\n"); 
                return new Cleric(PlyrName, 200);
            }
            else if(plyrInput == "3") {
                Console.WriteLine("Your Choise : Paladin\n"); 
                return new Paladin(PlyrName, 200);
            }
            else {
                Console.WriteLine("Not valid input\n");
                return PlyrChoise(PlyrName);    
            }

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