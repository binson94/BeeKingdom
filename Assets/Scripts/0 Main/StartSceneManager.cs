using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    private bool setting = false;
    [SerializeField] GameObject settingPanel;
    [SerializeField] GameObject creditPanel;

    void Start()
    {
        Screen.SetResolution(1920,1080,true);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Setting();
    }

    public void GameStart()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Save", 1);      //각 씬 숫자 저장, 불러오기 시 씬 넘버로 불러옴, 매 씬이 끝날 때마다 업데이트

        SceneManager.LoadScene("1 FirstStoryScene");
    }

    public void ContinueBtn()
    {
        if (PlayerPrefs.HasKey("Save"))
            SceneManager.LoadScene(PlayerPrefs.GetInt("Save"));
        else
            GameStart();
    }

    //설정 버튼, 돌아가기 버튼, esc 키(모바일 뒤로가기 키) 누르면 호출
    public void Setting()
    {
        setting = !setting;
        settingPanel.SetActive(setting);
    }

    public void CreditBtn()
    {
        setting = false;
        creditPanel.SetActive(true);
        settingPanel.SetActive(false);
    }

    public void CloseCreditBtn()
    {
        creditPanel.SetActive(false);
    }

    //설정 판넬의 게임 종료 버튼 누르면 호출 -> 게임 종료
    public void ExitGame()
    {
        Application.Quit();
    }
}
