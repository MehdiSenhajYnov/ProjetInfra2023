using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameNetClient
{
    public class Game : Singleton<Game>
    {
        public Character PlayerOne;
        public Character PlayerTwo; 
        public Character PlyrRound = null;

        public int round = 0;
        public int PlyrId = 0;
        //public string[] shieldASCII = File.ReadAllLines("./shieldimg.txt");
        //public string[] mageASCII = File.ReadAllLines("./mageimg.txt");
        //public string[] paladinASCII = File.ReadAllLines("./paladinimg.txt");
        public string plyrOneName = "";
        public string plyrTwoName = "";

        //public NetworkManager client;

        public Image charImage;
        public List<Sprite> charImages = new List<Sprite>();


        public Button MoveOneBtn;
        public Button MoveTwoBtn;
        [SerializeField] PlyrMoves plyrMoves;

        [SerializeField] RectTransform HealthOne;
        [SerializeField] RectTransform HealthTwo;

        [SerializeField] List<Sprite> charSprites = new List<Sprite>();
        [SerializeField] Image PlayerOneSprite;
        [SerializeField] Image PlayerTwoSprite;

        public int MyPlayerClass;

        public Image GetPlayerOneSprite()
        {
            return PlayerOneSprite;
        }

        public Image GetPlayerTwoSprite()
        {
            return PlayerTwoSprite;
        }

        private void Start()
        {
            plyrMoves.ChangeAllBtnState(false);
            // XMax 165 Y = 10
            HealthOne.sizeDelta = new Vector2(165, 10);
            HealthTwo.sizeDelta = new Vector2(165, 10);
            //GameBegin();
        }

        public void GameBegin () {
            
            //client.ConnectToServer();
        }

        private void OnDisable()
        {
            //client.Exit();
        }

        public void GameOver (string GameState) {

        }

        public int plyrToSet;

        public bool CheckName(string playernameinput) 
        {
            if (string.IsNullOrEmpty(playernameinput) || string.IsNullOrWhiteSpace(playernameinput)) return false;

            if (plyrToSet == 1)
            {
                plyrOneName = playernameinput;
                PlyrId = 1;

            }
            else if (plyrToSet == 2)
            {
                plyrTwoName = playernameinput;
                PlyrId = 2;
            }

            //NetworkManager.Instance.sendUserName();
            return true;
        }

        public void PlyrGetAtkInput(byte TypeOfPlyrInput) {

            //Utilities.Debugger(PlayerOne.ToString());
            //Utilities.Debugger(PlayerTwo.ToString());
        

            if (PlyrId == 1) {
                DisplayPlyr(TypeOfPlyrInput, plyrOneName);
                DisplayAtkChoiseOfCharacter(TypeOfPlyrInput);
            } else if (PlyrId == 2) {
                DisplayPlyr(TypeOfPlyrInput, plyrTwoName);
                DisplayAtkChoiseOfCharacter(TypeOfPlyrInput);
            }
            
            //return choiceOneOrTwo();
        }



        public void CibleOfAtkInput() {
            /*
            Utilities.Debugger("Your target?");
            Utilities.Debugger("1 : PlayerOne");
            Utilities.Debugger("2 : PlayerTwo");
            */
        }

        public void choiceOneOrTwo() {

            MoveOneBtn.gameObject.SetActive(true);
            MoveTwoBtn.gameObject.SetActive(true);

            /*string PlyrInput = "";

            PlyrInput = Console.ReadLine();
            PlyrInput = PlyrInput != null ? PlyrInput : PlyrInput = "";
            if (PlyrInput == "1") {
                return 1;
            }
            else if(PlyrInput == "2") {
                return 2;
            } else {
                Utilities.Debugger("Not valid input, try again");
                return choiceOneOrTwo();
            }*/

        }


        public void ChoosedOne()
        {

        }

        public void ChoosedTwo()
        {

        }


        public byte PlyrChoice(string PlyrName) {
            /*for (int i = 0; i < shieldASCII.Length && i < mageASCII.Length && i < paladinASCII.Length; i++)
            {
                Utilities.Debugger(shieldASCII[i] + mageASCII[i] + paladinASCII[i]);
            }*/
            /*/
            Utilities.Debugger(PlyrName + " CHOOSE YOUR CHARACTER");
            Utilities.Debugger("1 : Warrior");
            Utilities.Debugger("2 : Cleric");
            Utilities.Debugger("3 : Paladin");
            */
            string plyrInput = "";
            //plyrInput = Console.ReadLine();
            plyrInput = plyrInput != null ? plyrInput : plyrInput = "";
            if (plyrInput == "1") {
                //Utilities.Debugger("Your choice : Warrior\n"); 
                return 1;
            }
            else if(plyrInput == "2") {
                //Utilities.Debugger("Your choice : Cleric\n"); 
                return 2;
            }
            else if(plyrInput == "3") {
                //Utilities.Debugger("Your Choice : Paladin\n"); 
                return 3;
            }
            else {
                //Utilities.Debugger("Not valid input\n");
                return PlyrChoice(PlyrName);    
            }

        }

        public void DisplayPlyr(byte plyrToDisplay, string PlyrName) {
            if (plyrToDisplay == 1) {
                //Utilities.Debugger(PlyrName + '\n');
                PrintWarrior();
            } else if (plyrToDisplay == 2) {
                //Utilities.Debugger(PlyrName + '\n');
                PrintCleric();
            } else if (plyrToDisplay == 3) {
                //Utilities.Debugger(PlyrName + '\n');
                PrintPaladin();
            }
        }


        public void UpdatePlyrsFromServer(byte[] byteInfo) {
            // PTD = Player to display

            if (byteInfo[0] == 1) {
                PlayerOne = new Warrior(plyrOneName, (int)byteInfo[1]);
                SetPlyrImage(0, PlayerOneSprite, PlayerTwoSprite);
            }
            if (byteInfo[0] == 2) {
                PlayerOne = new Cleric(plyrOneName, (int)byteInfo[1]);
                SetPlyrImage(1, PlayerOneSprite, PlayerTwoSprite);
            }
            if (byteInfo[0] == 3) {
                PlayerOne = new Paladin(plyrOneName, (int)byteInfo[1]);
                SetPlyrImage(2, PlayerOneSprite, PlayerTwoSprite);
            }
            PlayerOne.TakeDamage((int)(byteInfo[1] - byteInfo[2]));
            PlayerOne.SetUniqueValue(byteInfo[3]);

            if (byteInfo[4] == 1) {
                PlayerTwo = new Warrior(plyrTwoName, (int)byteInfo[5]);
                SetPlyrImage(0, PlayerTwoSprite, PlayerOneSprite);
            }
            if (byteInfo[4] == 2) {
                PlayerTwo = new Cleric(plyrTwoName, (int)byteInfo[5]);
                SetPlyrImage(1, PlayerTwoSprite, PlayerOneSprite);
            }
            if (byteInfo[4] == 3) {
                PlayerTwo = new Paladin(plyrTwoName, (int)byteInfo[5]);
                SetPlyrImage(2, PlayerTwoSprite, PlayerOneSprite);
            }
            PlayerTwo.TakeDamage((int)(byteInfo[5] - byteInfo[6]));
            PlayerTwo.SetUniqueValue(byteInfo[7]);



            // Mon joueur est toujours celui en bas � gauche
            // Health : MaxHealth =  x : 165
            if (PlyrId == 1)
            {
                HealthOne.sizeDelta = new Vector2(PlayerOne.Health * 165 / PlayerOne.MaxHealth, 10);
                HealthTwo.sizeDelta = new Vector2(PlayerTwo.Health * 165 / PlayerTwo.MaxHealth, 10);
            } else
            {
                HealthTwo.sizeDelta = new Vector2(PlayerOne.Health * 165 / PlayerOne.MaxHealth, 10);
                HealthOne.sizeDelta = new Vector2(PlayerTwo.Health * 165 / PlayerTwo.MaxHealth, 10);
            }

            PlyrMoves.Instance.SetAtkDescription();

        }

        public void SetPlyrImage(int ImageToSet, Image PlyrOneImage, Image PlyrTwoImage)
        {
            Debug.Log("I Have To Set int " + ImageToSet + " my plyr id is " + PlyrId);
            if (PlyrId == 1)
            {
                PlyrOneImage.sprite = charSprites[ImageToSet];
            }
            else
            {
                PlyrTwoImage.sprite = charSprites[ImageToSet];
            }
        }
        public void DisplayAtkChoiseOfCharacter(byte characterToUse) {
            /*
            Utilities.Debugger("\n What Do you want to do?");
            if (characterToUse == 1) {
                Utilities.Debugger("1 : BaseAttack : 25 damage, if Bravery is active 15 supply damage");
                Utilities.Debugger("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
            }
            if (characterToUse == 2) {
                Utilities.Debugger("1 : BaseAttack : +15 hp");
                Utilities.Debugger("2 : AlternatifAttack : You will inflict a demage equal to the half of your mana, but -80 of your mana");
            }
            if (characterToUse == 3) {
                Utilities.Debugger("1 : BaseAttack : You will inflict damage equal to 25 + your buff, buff +3 (15 max)");
                Utilities.Debugger("2 : AlternatifAttack : 50 damge, but you take a backlash of 10 hp");
            }
            */
        }


        public void PrintWarrior() {
            charImage.sprite = charImages[0];
        }
        public void PrintCleric() {
            charImage.sprite = charImages[1];
        }
        public void PrintPaladin() {
            charImage.sprite = charImages[2];
        }
    }
    
}