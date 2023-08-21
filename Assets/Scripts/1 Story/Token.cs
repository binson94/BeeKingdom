using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Token : MonoBehaviour
{
    [SerializeField] Text logText;
    [HideInInspector] public string nameText;
    [HideInInspector] public string scriptText;
    public int scriptHeight;

    public void Set(string n, string s)
    {
        nameText = n;
        scriptText = s;
        logText.text = string.Concat(n, "\n", s);

        scriptHeight = (s.Length / 50) + 1;
    }
}

