using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Riptide;
using Riptide.Utils;

public class WinnerScreen : MonoBehaviour
{
    [SerializeField]Button backToHome;
    // Start is called before the first frame update
    void Start()
    {
        backToHome.onClick.AddListener(() => Utilities.Instance.showScreen(ScreenGame.Home));
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    [MessageHandler((ushort)whatmessage.Winner)]
    public static void GetWinner(Message message)
    {
        Utilities.Instance.showScreen(ScreenGame.Winner);
        var winnerstr = message.GetString();
        Debug.Log("WINNER ! : " + winnerstr);
        Utilities.Debugger(winnerstr);
    }
}
