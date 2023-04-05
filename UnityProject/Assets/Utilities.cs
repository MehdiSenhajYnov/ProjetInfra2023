using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Utilities : Singleton<Utilities>
{
    private void Start()
    {
        showScreen(ScreenGame.Home);
    }
    private static TMP_Text DebugText;
    public static TMP_Text _debugText
    {
        get
        {
            if (DebugText == null)
            {
                DebugText = GameObject.FindGameObjectWithTag("Debugger").GetComponent<TMP_Text>();
            }
            return DebugText;
        }
        set
        {
            DebugText = value;
        }
    }

    public static void Debugger(string textToDebug)
    {
        _debugText.gameObject.SetActive(true);
        _debugText.text = textToDebug;
    }
    public static void DesactivateDebugger()
    {
        _debugText.gameObject.SetActive(false);
    }

    public List<GameObject> screensParent = new List<GameObject>();

    /*
    public void showScreen(int indexOfScreen)
    {
        for (int i = 0; i < screensParent.Count; i++)
        {
            if (i == indexOfScreen)
            {
                screensParent[i].SetActive(true);
            }
            else
            {
                screensParent[i].SetActive(false);
            }
        }
    }
    */
    
    public void showScreen(ScreenGame indexOfScreen)
    {
        for (int i = 0; i < screensParent.Count; i++)
        {
            if (i == (int)indexOfScreen)
            {
                screensParent[i].SetActive(true);
            }
            else
            {
                screensParent[i].SetActive(false);
            }
        }
    }

}
public enum ScreenGame
{
    Home,
    Menu,
    Login,
    Game,
    Winner,
}
