using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameNetClient;
using Riptide;
using Riptide.Utils;
using System;
using TMPro;
public class SavesSystem : Singleton<SavesSystem>
{

    [SerializeField] Game game;

    [SerializeField] Button continueButton;

    public string EnemyName;
    [SerializeField] TMP_Text enemynametext;
    // Start is called before the first frame update

    public string PlayerUniqueId;
    
    void Start()
    {
        // ENABLE IF SAVE SYSTEM WORK continueButton.onClick.AddListener(askLoad);
    }

    void askLoad()
    {
        // ENABLE IF SAVE SYSTEM WORK NetPlayer.MMSendString(PlayerUniqueId, MMCodes.ASK_LOAD_SAVE);
    }

    [MessageHandler((ushort)MMCodes.GET_ENEMY_NAME, 1)]
    public static void GetServerToConnect(Message message)
    {
        string enemyname = message.GetString();
        if (enemyname != "NOSAVEFOUND")
        {
            Instance.continueButton.interactable = true;
        }
        Instance.enemynametext.text = enemyname;
    }


    private void OnEnable()
    {
        /* ENABLE IF SAVE SYSTEM WORK 
        if (PlayerPrefs.HasKey("PlayerUniqueId"))
        {
            PlayerUniqueId = PlayerPrefs.GetString("PlayerUniqueId");
        }
        else
        {
            PlayerUniqueId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PlayerUniqueId", PlayerUniqueId);
        }
        */
        PlayerUniqueId = Guid.NewGuid().ToString();
        continueButton.interactable = false;
        // ENABLE IF SAVE SYSTEM WORK NetPlayer.MMSendString(PlayerUniqueId,MMCodes.ASK_ENEMY_NAME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
