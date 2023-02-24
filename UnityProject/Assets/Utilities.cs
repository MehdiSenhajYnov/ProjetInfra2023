using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Utilities : MonoBehaviour
{
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
}
