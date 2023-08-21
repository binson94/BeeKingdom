using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class LogManager : MonoBehaviour
{
    int logLastPos = 50;
    List<Token> logs = new List<Token>();

    [Header("Prefab")]
    [SerializeField] GameObject rightLogPref;
    [SerializeField] GameObject leftLogPref;

    [Header("Log Parent")]
    [SerializeField] RectTransform content;
    [SerializeField] ScrollRect scroll;

    [Header("Arrow")]
    [SerializeField] GameObject upArrow;
    [SerializeField] GameObject downArrow;


    [Header("Panel")]
    [SerializeField] GameObject logPanel;
    bool isLogOn = false;
    // Start is called before the first frame update
    void Start()
    {
        logPanel.SetActive(isLogOn);
    }

    // Update is called once per frame
    void Update()
    {
        if (scroll.verticalNormalizedPosition <= 0.1)
        {
            upArrow.SetActive(true);
            downArrow.SetActive(false);
        }
        else if (scroll.verticalNormalizedPosition >= 0.9)
        {
            upArrow.SetActive(false);
            downArrow.SetActive(true);
        }
        else
        {
            upArrow.SetActive(true);
            downArrow.SetActive(true);
        }

    }

    public void Btn_LogOn()
    {
        isLogOn = !isLogOn;
        logPanel.SetActive(isLogOn);
    }

    string namecheck(int a)
    {
        string name;
        if (a <= 7)
        {
            name = "<size=65><color=yellow>꾸르벌</color></size>";
        }
        else if (a == 8)
        {
            name = "<size=65><color=#ABF200>일벌</color></size>";
        }
        else if (a >= 9 && a <= 13)
        {
            name = "<size=65><color=#FF5E00>나비</color></size>";
        }
        else if (a >= 14 && a <= 17)
        {
            name = "<size=65><color=#5D5D5D>너구리</color></size>";
        }
        else if (a >= 18 && a <= 21)
        {
            name = "<size=65><color=#993800>벌매</color></size>";
        }
        else if (a == 32)
        {
            name = "<size=65><color=#2F9D27>양봉업자</color></size>";
        }
        else if (a == 31)
        {
            name = "<size=65><color=black>나레이션</color></size>";
        }
        else
            name = "";

        return name;
    }

    public void AddLog(Yeol.Script s)
    {
        Token log;
        RectTransform rect;

        if (s.cutIdx < 8 || s.cutIdx == 31)
            log = Instantiate(leftLogPref).GetComponent<Token>();
        else if (s.cutIdx < 22 || s.cutIdx == 32)
            log = Instantiate(rightLogPref).GetComponent<Token>();
        else
            return;

        rect = log.GetComponent<RectTransform>();

        log.Set(namecheck(s.cutIdx), s.str);

        rect.SetParent(content);
        rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, logLastPos, 150 * log.scriptHeight);

        logLastPos += log.scriptHeight * 150;
        logs.Add(log);

        scroll.normalizedPosition = Vector2.zero;
    }
}
