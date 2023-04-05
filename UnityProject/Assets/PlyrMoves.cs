using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Riptide;
using Riptide.Utils;
using GameNetClient;
using TMPro;
public class PlyrMoves : Singleton<PlyrMoves>
{
    [SerializeField] Button AttackOne;
    [SerializeField] Button AttackTwo;
    [SerializeField] Button CibleOne;
    [SerializeField] Button CibleTwo;
    //List<Button> allBtns;
    // Start is called before the first frame update
    void Start()
    {
        //allBtns = new List<Button>() { AttackOne, AttackTwo, CibleOne, CibleTwo };

        AttackOne.onClick.AddListener(() => attackClicked(1));
        AttackTwo.onClick.AddListener(() => attackClicked(2));

        CibleOne.onClick.AddListener(() => CibleClicked(1));
        CibleTwo.onClick.AddListener(() => CibleClicked(2));

    }

    public void ChangeAllBtnState(bool newState)
    {
        ChangeMoveState(newState);
        ChangeCibleState(newState);
    }

    public void ChangeMoveState(bool newState)
    {
        AttackOne.interactable = newState;
        AttackTwo.interactable = newState;
    }

    public void ChangeCibleState(bool newState)
    {
        CibleOne.interactable = newState;
        CibleTwo.interactable = newState;
    }

    [MessageHandler((ushort)whatmessage.AskPlayerMove)]
    public static void ActiveCanPlay(Message message)
    {
        byte[] data = message.GetBytes();
        Game.Instance.UpdatePlyrsFromServer(data);
        Debug.Log("AskPlayerMove");
        Utilities.Debugger("AskPlayerMove");
        Instance.ChangeMoveState(true);
        Instance.ChangeCibleState(false);

    }
    int PlyrInput;
    void attackClicked(int atk)
    {
        ChangeMoveState(false);
        ChangeCibleState(true);
        PlyrInput = atk;
    }

    int CiblePlyr;
    void CibleClicked(int Cible)
    {
        ChangeAllBtnState(false);
        int myId = Game.Instance.PlyrId;
        if (myId == 1)
        {
            CiblePlyr = Cible;
        } else
        {
            if (Cible == 1) CiblePlyr = 2;
            if (Cible == 2) CiblePlyr = 1;
        }

        var attackInfo = new byte[] {(byte)PlyrInput, (byte)CiblePlyr };

        Debug.Log("Send Attack " + PlyrInput + " On Cible " + CiblePlyr);
        Utilities.Debugger("Send Attack " + PlyrInput + " On Cible " + CiblePlyr);

        NetPlayer.SendBytes(attackInfo, whatmessage.PlyrMove);
    }

    public void SetCibleNames(string myName, string otherName)
    {
        CibleOne.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = myName + " (YOU)";
        CibleTwo.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = otherName;

    }

    public void SetAtkDescription()
    {
        int myclass = Game.Instance.MyPlayerClass;
        if (myclass == 1)
        {
            AttacksDescriptionOne.text = AttacksDescription[0];
            AttacksDescriptionTwo.text = AttacksDescription[1];
        }
        else if (myclass == 2)
        {
            AttacksDescriptionOne.text = AttacksDescription[2];
            AttacksDescriptionTwo.text = AttacksDescription[3];
        }
        else if (myclass == 3)
        {
            AttacksDescriptionOne.text = AttacksDescription[4];
            AttacksDescriptionTwo.text = AttacksDescription[5];
        }
    }

    [SerializeField] List<string> AttacksDescription = new List<string>();
    [SerializeField] TMP_Text AttacksDescriptionOne;
    [SerializeField] TMP_Text AttacksDescriptionTwo;

    // Update is called once per frame
    void Update()
    {
        
    }
}
