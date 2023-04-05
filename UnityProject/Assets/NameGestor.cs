using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Riptide;
using Riptide.Utils;
using TMPro;
using GameNetClient;
public class NameGestor : MonoBehaviour
{
    public Game game;
    public TMP_InputField usernameinput;
    public Button submitButton;
    // Start is called before the first frame update
    void Start()
    {
        usernameinput.onSubmit.AddListener(usernamesubmitted);
        submitButton.onClick.AddListener(() => usernamesubmitted(usernameinput.text));

        WarriorBtn.onClick.AddListener(() => ClassChoise(1));
        ClericBtn.onClick.AddListener(() => ClassChoise(2));
        PaladinBtn.onClick.AddListener(() => ClassChoise(3));
    }

    void usernamesubmitted(string username)
    {
        if (!game.CheckName(username)) return;
        
        usernameinput.interactable = false;
        submitButton.interactable = false;

        NetPlayer.SendString(username, whatmessage.clientname);

        Utilities.Debugger("Wait for player");
    }


    [SerializeField] Button WarriorBtn;
    [SerializeField] Button ClericBtn;
    [SerializeField] Button PaladinBtn;


    public void ClassChoise(byte choise)
    {
        Game.Instance.MyPlayerClass = choise;
        NetPlayer.SendBytes(new byte[] { choise, (byte)game.PlyrId }, whatmessage.PlyrChoice);
        setInterctablesBtns(false);
    }

    public void setInterctablesBtns(bool newstate)
    {
        WarriorBtn.interactable = newstate;
        ClericBtn.interactable = newstate;
        PaladinBtn.interactable = newstate;
    }

    [MessageHandler((ushort)whatmessage.OtherClientName)]
    public static void GetOtherPlayerUsername(Message message)
    {
        Utilities.Instance.showScreen(ScreenGame.Game);
        string text = message.GetString();
        int charType = message.GetInt() - 1;
        if (Game.Instance.plyrOneName == "")
        {
            Game.Instance.plyrOneName = text;
            Debug.Log("Your enemy is : " + Game.Instance.plyrOneName);
            Utilities.Debugger("Your enemy is : " + Game.Instance.plyrOneName);
            Game.Instance.SetPlyrImage(charType, Game.Instance.GetPlayerOneSprite(), Game.Instance.GetPlayerTwoSprite());
            Game.Instance.SetPlyrImage(Game.Instance.MyPlayerClass - 1, Game.Instance.GetPlayerTwoSprite(), Game.Instance.GetPlayerOneSprite());

            PlyrMoves.Instance.SetCibleNames(Game.Instance.plyrTwoName, Game.Instance.plyrOneName);
        }
        else if (Game.Instance.plyrTwoName == "")
        {
            Game.Instance.plyrTwoName = text;
            Debug.Log("Your enemy is : " + Game.Instance.plyrTwoName);
            Utilities.Debugger("Your enemy is : " + Game.Instance.plyrTwoName);
            Game.Instance.SetPlyrImage(charType, Game.Instance.GetPlayerTwoSprite(), Game.Instance.GetPlayerOneSprite());
            Game.Instance.SetPlyrImage(Game.Instance.MyPlayerClass - 1, Game.Instance.GetPlayerOneSprite(), Game.Instance.GetPlayerTwoSprite());

            PlyrMoves.Instance.SetCibleNames(Game.Instance.plyrOneName, Game.Instance.plyrTwoName);
        }


        PlyrMoves.Instance.SetAtkDescription();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
