using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class Log : MonoBehaviour
{
    public class Script
    {
        public int cutIdx;  //0 ~ 21 : 캐릭터 이미지, 22 : 선택지, 23 : bgm 변경, 24 : 효과음, 25 : 씬 변경, 26 : 데이터 저장, 27 : 지점 점프
        public string str;
        public string str2;     //선택지에서만 사용

        public Script(int a, string b, string c = "")
        {
            cutIdx = a;
            str = b;
            str2 = c;
        }
    }

    List<Script> scripts = new List<Script>();
    public GameObject logPanel;
    private bool panelcontrol = false;
    public GameObject LogPrefs1, LogPrefs2;
    public GameObject a,b;
    public ScrollRect scroll;
    // Start is called before the first frame update
    void Start()
    {
        //LogPrefs = Resources.Load<GameObject>("LogText");
        logPanel.SetActive(panelcontrol);
        log();
    }

    // Update is called once per frame
    void Update()
    {
        if(scroll.verticalNormalizedPosition <= 0.1)
        {
            a.SetActive(true);
            b.SetActive(false);
        }
        else if(scroll.verticalNormalizedPosition >= 0.9)
        {
            a.SetActive(false);
            b.SetActive(true);
        }
        else
        {
            a.SetActive(true);
            b.SetActive(true);
        }
        
    }

    void ScriptLoad()
    {
        TextAsset txtAsset;
        string loadStr;
        JsonData json;
        txtAsset = Resources.Load<TextAsset>(string.Concat("Jsons/", SceneManager.GetActiveScene().name));
        loadStr = txtAsset.text;
        json = JsonMapper.ToObject(loadStr);

        int max = int.Parse(json[0]["cut"].ToString());
        for (int i = 1; i <= max; i++)
            scripts.Add(new Script(int.Parse(json[i]["cut"].ToString()), json[i]["script1"].ToString(), json[i]["script2"].ToString()));
    }

    Script GetNextToken()
    {
        if (scripts.Count > 0)
        {
            Script s = scripts[0];
            scripts.RemoveAt(0);

            return s;
        }
        else
            return null;
    }
    public void PanelContol()
    {
        panelcontrol = !panelcontrol;
        logPanel.SetActive(panelcontrol);
    }
    string namecheck(int a)
    {
        string name;
        if(a <= 7)
        {
            name = "<size=65><color=yellow>꾸르벌</color></size>";
        }
        else if(a == 8)
        {
            name = "<size=65><color=#ABF200>일벌</color></size>";
        }
        else if(a >= 9 && a <= 13)
        {
            name = "<size=65><color=#FF5E00>나비</color></size>";
        }
        else if(a >= 14 && a <= 17)
        {
            name = "<size=65><color=#5D5D5D>너구리</color></size>";
        }
        else if(a >= 18 && a <= 21)
        {
            name = "<size=65><color=#993800>벌매</color></size>";
        }
        else if(a == 32)
        {
            name = "<size=65><color=#2F9D27>양봉업자</color></size>";
        }
        else if(a == 31)
        {
            name = "<size=65><color=black>나레이션</color></size>";
        }
        else
            name = "";

        return name;
    }
    void log()
    {
        string scenename = SceneManager.GetActiveScene().name;
        ScriptLoad();
        
        for(int j = 0; j < scripts.Count; j++)
        {
            Script s = GetNextToken();
                if(s.cutIdx <= 21 || s.cutIdx == 31 || s.cutIdx == 32)
                {
                    if(s.cutIdx >= 8 && s.cutIdx <= 21)
                    {
                        GameObject text;
                        text = (GameObject)Instantiate(LogPrefs1);
                        RectTransform logpos = text.GetComponent<RectTransform>();
                        text.transform.position = gameObject.transform.position;
                        string name = namecheck(s.cutIdx);
                        text.GetComponent<Text>().text += name + "\n" +s.str +"\n\n";
                        logpos.SetParent(GameObject.Find("Canvas").transform.Find("LogPanel").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content"));
                        logpos.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,50+150*j,150);
                    }
                    else
                    {
                        GameObject text;
                        text = (GameObject)Instantiate(LogPrefs2);
                        RectTransform logpos = text.GetComponent<RectTransform>();
                        text.transform.position = gameObject.transform.position;
                        string name = namecheck(s.cutIdx);
                        text.GetComponent<Text>().text += name + "\n" +s.str +"\n\n";
                        logpos.SetParent(GameObject.Find("Canvas").transform.Find("LogPanel").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content"));
                        logpos.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,50+150*j,150);
                    }
                }
        }
    
    }
}
