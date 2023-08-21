using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FHOSceneManager : MonoBehaviour
{
    public Text timeText;
    public GameObject Explain_Panel, Clear_Panel, Fail_Panel;
    public Button[] answer;
    private float time = 60;  
    private int score = 0;
    private int success = 3;  
    private bool start = false;
    private bool check = true;
    private bool temp = false;

    [SerializeField] Image blackImage;

    [Header("Setting")]
    [SerializeField] GameObject setting_Panel;
    [HideInInspector] public bool isSetting = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FirstFOCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(start)
        {
            time -= Time.deltaTime;
            timeText.text = string.Format("{0:N2}",time);
        }
        if(score == success && check)
        {
            start = false;
            Clear_Panel.SetActive(true);
            StartCoroutine(fadeIn(Clear_Panel));
            check = false;
        }
        else if(score < success && time <= 0 && check)
        {
            start = false;
            time = 0;
            timeText.text = string.Format("{0:N2}", time);

            Fail_Panel.SetActive(true);
            StartCoroutine(fadeIn(Fail_Panel));
            check = false;
        }
    }

    #region Button
    public void Btn_GameStart()
    {
        start = true;
        StartCoroutine(fadeOut(Explain_Panel));
        Explain_Panel.SetActive(false);

    }
    
    public void Btn_Clear()
    {
        PlayerPrefs.SetString("ButterflyGame", "음~. 아름다운 내게 딱 어울리는 맛이야. 달지만 깊이 있는 이 맛은 고풍스럽기도 하지만, 화려하기도 하지.");
        PlayerPrefs.SetInt("Save", 3);

        StartCoroutine(LastFICoroutine());
    }

    public void Btn_Fail()
    {
        PlayerPrefs.SetString("ButterflyGame", "하암~. 아직 멀었어?");
        PlayerPrefs.SetInt("Save", 3);

        StartCoroutine(LastFICoroutine());
    }

    public void Btn_Setting()
    {
        isSetting = !isSetting;

        if (time < 60)
            start = !isSetting;

        setting_Panel.SetActive(isSetting);
    }

    public void Btn_Quit()
    {
        Application.Quit();
    }

    //더미 데이터
    public void Btn_Retry()
    {
        time = 60;
        StartCoroutine(fadeOut(Fail_Panel));
        Fail_Panel.SetActive(false);
        start = true;
        check = true;
        score = 0;
        for(int i = 0; i < answer.Length; i++)
        {
            answer[i].interactable = true;
            answer[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1,0);
        }
    }
    #endregion

    public void score_add()
    {
        score++;
    }

    IEnumerator fadeIn(GameObject panel)
    {
        float count = 0;
        while(count < 1.0f)
        {
            count += 0.01f;
            yield return new WaitForSeconds(0.01f);
            panel.GetComponent<Image>().color = new Color(1,1,1,count);
        }
    }

    IEnumerator fadeOut(GameObject panel)
    {
        float count = 1;
        while(count > 0.0f)
        {
            count -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            panel.GetComponent<Image>().color = new Color(1,1,1,count);
        }
    }

    IEnumerator FirstFOCoroutine()
    {
        float count = 1;
        while (count > 0.0f)
        {
            count -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            blackImage.color = new Color(0, 0, 0, count);
        }
    }

    IEnumerator LastFICoroutine()
    {
        float count = 0;
        while (count < 1.0f)
        {
            count += 0.01f;
            yield return new WaitForSeconds(0.01f);
            blackImage.color = new Color(0, 0, 0, count);
        }

        SceneManager.LoadScene(3);
    }

}
