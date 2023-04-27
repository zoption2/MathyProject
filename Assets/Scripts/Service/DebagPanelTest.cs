using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebagPanelTest : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public static DebagPanelTest Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Log(string massage)
    {
        string temp = text.text;
        temp += "\n" + massage;
        text.text = temp;
    }
}
