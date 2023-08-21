using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

namespace Yeol
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

    public class CutSceneManager : MonoBehaviour
    {
        //캐릭터 스프라이트
        [SerializeField] Sprite[] character_sprites;
        [SerializeField] Sprite[] characterNameSprites;
        //꾸르벌(0기본,1고민,2기쁨,3당황,4부끄,5슬픔,6지침7화남)
        //벌 상대(8)
        //나비(9기본,10귀찮,11기쁨,12부끄,13의심)
        //너구리(14기본,15귀찮,16발끈,17졸림)
        //벌매(18기본,19거만,20화남,21흥미)

        //22 : 선택지, 23 : bgm 변경, 24 : 효과음 25 : 씬 변경, 26 : 데이터 저장
        //27 : 지점 점프(PlayerPrefs 기반), 28 : 지점 점프(텍스트 기반)
        //29 : 이미지 페이드 인(str=="0~7", 왼쪽, "8~21" 오른쪽, "22" 텍스트 판넬), 30 : 이미지 페이드 아웃
        //31 : 나레이션, 32 : 양봉업자
        List<Script> scripts = new List<Script>();

        [SerializeField] GameObject nextBtn;
        [SerializeField] Image FirstBlackImage;

        [Header("Character")]
        [SerializeField] Image leftCharImage;
        [SerializeField] Image leftCharName;
        [SerializeField] Image rightCharImage;
        [SerializeField] Image rightCharName;

        [SerializeField] Text charText;
        [SerializeField] Image text_Panel;
        bool isFade = false;

        [Header("Select")]
        [SerializeField] GameObject select_Panel;
        [SerializeField] Text selectTxt1;
        [SerializeField] Text selectTxt2;

        [Header("Setting")]
        [SerializeField] GameObject setting_Panel;
        bool isSetting = false;

        [Header("Sound")]
        [SerializeField] AudioSource BGM;
        [SerializeField] AudioSource SFX;
        [SerializeField] AudioClip[] BGMList;
        [SerializeField] AudioClip[] SFXList;

        [Header("Log")]
        [SerializeField] LogManager logMgr;

        void Start()
        {
            ScriptLoad();

            isFade = true;

            if (SceneManager.GetActiveScene().name != "1 FirstStoryScene")
                StartFade();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SettingBtn();
        }

        #region Data
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

        void Happy_Ending()
        {
            TextAsset txtAsset;
            string loadStr;
            JsonData json;
            txtAsset = Resources.Load<TextAsset>(string.Concat("Jsons/", "HappyEnding"));
            loadStr = txtAsset.text;
            json = JsonMapper.ToObject(loadStr);

            int max = int.Parse(json[0]["cut"].ToString());
            for (int i = 1; i <= max; i++)
                scripts.Add(new Script(int.Parse(json[i]["cut"].ToString()), json[i]["script1"].ToString(), json[i]["script2"].ToString()));
        }

        void Normal_Ending1()
        {
            TextAsset txtAsset;
            string loadStr;
            JsonData json;
            txtAsset = Resources.Load<TextAsset>(string.Concat("Jsons/", "Normal1Ending"));
            loadStr = txtAsset.text;
            json = JsonMapper.ToObject(loadStr);

            int max = int.Parse(json[0]["cut"].ToString());
            for (int i = 1; i <= max; i++)
                scripts.Add(new Script(int.Parse(json[i]["cut"].ToString()), json[i]["script1"].ToString(), json[i]["script2"].ToString()));
        }

        void Normal_Ending2()
        {
            TextAsset txtAsset;
            string loadStr;
            JsonData json;
            txtAsset = Resources.Load<TextAsset>(string.Concat("Jsons/", "Normal2Ending"));
            loadStr = txtAsset.text;
            json = JsonMapper.ToObject(loadStr);

            int max = int.Parse(json[0]["cut"].ToString());
            for (int i = 1; i <= max; i++)
                scripts.Add(new Script(int.Parse(json[i]["cut"].ToString()), json[i]["script1"].ToString(), json[i]["script2"].ToString()));
        }

        void Bad_Ending()
        {
            TextAsset txtAsset;
            string loadStr;
            JsonData json;
            txtAsset = Resources.Load<TextAsset>(string.Concat("Jsons/", "BadEnding"));
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
        #endregion

        //시작 함수
        public void StartFade()
        {
            StartCoroutine(FirstFadeOut());
        }

        //최초 페이드 아웃 - 검은색 배경 제거
        IEnumerator FirstFadeOut()
        {
            isFade = true;
            Color c = FirstBlackImage.color;
            float r = c.r, g = c.g, b = c.b;
            float count = 1;
            while (count > 0.0f)
            {
                count -= 0.02f;
                yield return new WaitForSeconds(0.005f);
                FirstBlackImage.color = new Color(r, g, b, count);
            }

            NextAction();
        }
        
        public void Btn_Next()
        {
            nextBtn.SetActive(false);
            NextAction();
        }

        void NextAction()
        {
            Script s = GetNextToken();

            //꾸르벌 대사
            if (s.cutIdx < 8)
            {
                leftCharImage.sprite = character_sprites[s.cutIdx];
                StartCoroutine(ImageFICoroutine(leftCharName));
                StartCoroutine(ImageFOCoroutine(rightCharName));
                StartCoroutine(TextFOCoroutine(s.str));
                logMgr.AddLog(s);
            }
            //상대방 대사
            else if (s.cutIdx < 22)
            {
                rightCharImage.sprite = character_sprites[s.cutIdx];
                if (s.cutIdx < 9)
                    rightCharName.sprite = characterNameSprites[0];
                else if (s.cutIdx < 14)
                    rightCharName.sprite = characterNameSprites[1];
                else if (s.cutIdx < 18)
                    rightCharName.sprite = characterNameSprites[2];
                else
                    rightCharName.sprite = characterNameSprites[3];

                StartCoroutine(ImageFICoroutine(rightCharName));
                StartCoroutine(ImageFOCoroutine(leftCharName));
                StartCoroutine(TextFOCoroutine(s.str));
                logMgr.AddLog(s);

            }
            //선택지인 경우
            else if (s.cutIdx == 22)
            {
                ShowSelectUI(s);
            }
            //bgm 변경
            else if (s.cutIdx == 23)
            {
                BGM.clip = BGMList[int.Parse(s.str)];
                BGM.Play();
                NextAction();
            }
            //효과음 재생
            else if (s.cutIdx == 24)
            {
                SFX.clip = SFXList[int.Parse(s.str)];
                SFX.Play();
                NextAction();
            }
            //씬 전환
            else if (s.cutIdx == 25)
            {
                FadeEnd(s.str);
            }
            //값 저장
            else if (s.cutIdx == 26)
            {
                PlayerPrefs.SetString(s.str, s.str2);
                NextAction();
            }
            //구간 점프(PlayerPrefs 기반)
            else if (s.cutIdx == 27)
            {
                if (PlayerPrefs.GetString(s.str) == "음~. 아름다운 내게 딱 어울리는 맛이야. 달지만 깊이 있는 이 맛은 고풍스럽기도 하지만, 화려하기도 하지." || PlayerPrefs.GetString(s.str) == "하암~. 아직 멀었어?")
                {
                    rightCharImage.sprite = character_sprites[11];
                    StartCoroutine(ImageFICoroutine(rightCharImage));
                }

                while (!((scripts[0].cutIdx < 22 || scripts[0].cutIdx == 31 || scripts[0].cutIdx == 32) && scripts[0].str == PlayerPrefs.GetString(s.str)))
                {
                    GetNextToken();
                }

                NextAction();
            }
            //구간 점프(string 기반)
            else if (s.cutIdx == 28)
            {
                while (!((scripts[0].cutIdx < 22 || scripts[0].cutIdx == 31 || scripts[0].cutIdx == 32) && scripts[0].str == s.str))
                    GetNextToken();

                NextAction();
            }
            //이미지 페이드 인
            else if (s.cutIdx == 29)
            {
                int num = int.Parse(s.str);
                if (num < 8)
                {
                    leftCharImage.sprite = character_sprites[num];
                    StartCoroutine(ImageFICoroutine(leftCharImage));
                }
                else if (num < 22)
                {
                    rightCharImage.sprite = character_sprites[num];
                    StartCoroutine(ImageFICoroutine(rightCharImage));
                }
                else
                    StartCoroutine(ImageFICoroutine(text_Panel));

                NextAction();
            }
            //이미지 페이드 아웃
            else if (s.cutIdx == 30)
            {
                int num = int.Parse(s.str);
                if (num < 8)
                    StartCoroutine(ImageFOCoroutine(leftCharImage));
                else if (num < 22)
                    StartCoroutine(ImageFOCoroutine(rightCharImage));
                else
                    StartCoroutine(ImageFOCoroutine(text_Panel));

                NextAction();
            }
            //나레이션
            else if(s.cutIdx == 31)
            {
                StartCoroutine(ImageFOCoroutine(rightCharName));
                StartCoroutine(ImageFOCoroutine(leftCharName));
                StartCoroutine(TextFOCoroutine(s.str));
                logMgr.AddLog(s);
            }
            //양봉업자
            else if(s.cutIdx==32)
            {
                rightCharName.sprite = characterNameSprites[4];

                StartCoroutine(ImageFICoroutine(rightCharName));
                StartCoroutine(ImageFOCoroutine(leftCharName));
                StartCoroutine(TextFOCoroutine(s.str));
                logMgr.AddLog(s);
            }
        }
        
        //투명해짐
        #region FadeOut
        IEnumerator ImageFOCoroutine(Image image)
        {
            Color c = image.color;
            float r = c.r, g = c.g, b = c.b;
            float count = 1;

            if (c.a > 0)
                while (count > 0.0f)
                {
                    count -= 0.02f;
                    yield return new WaitForSeconds(0.005f);
                    image.color = new Color(r, g, b, count);
                }
        }

        IEnumerator TextFOCoroutine(string txt)
        {
            float count = 1;
            if (charText.color.a > 0)
                while (count > 0.0f)
                {
                    count -= 0.02f;
                    yield return new WaitForSeconds(0.005f);
                    charText.color = new Color(0, 0, 0, count);
                }
            
            StartCoroutine(TextFICoroutine(txt));
        }
        #endregion

        //불투명해짐
        #region FadeIn
        IEnumerator ImageFICoroutine(Image image)
        {
            Color c = image.color;
            float r = c.r, g = c.g, b = c.b;

            float count = 0;
            if (c.a < 1)
                while (count < 1.0f)
                {
                    count += 0.02f;
                    yield return new WaitForSeconds(0.005f);
                    image.color = new Color(r, g, b, count);
                }
        }

        IEnumerator TextFICoroutine(string txt)
        {
            charText.text = txt;

            float count = 0;
            while (count < 1)
            {
                count += 0.02f;
                yield return new WaitForSeconds(0.005f);
                charText.color = new Color(0, 0, 0, count);
            }

            isFade = false;
            if (!isSetting)
                nextBtn.SetActive(true);
        }
        #endregion

        #region Select
        //선택지 보이기
        void ShowSelectUI(Script s)
        {
            selectTxt1.text = s.str;
            selectTxt2.text = s.str2;

            select_Panel.SetActive(true);
        }

        //선택지 선택 시 호출 -> 선택 내용 대화 나올 때까지 스킵
        public void Btn_Select(int num)
        {
            select_Panel.SetActive(false);
            switch (num)
            {
                case 0:
                    while (!(scripts[0].cutIdx < 22 && scripts[0].str == selectTxt1.text))
                        GetNextToken();

                    NextAction();
                    break;
                case 1:
                    while (!(scripts[0].cutIdx < 22 && scripts[0].str == selectTxt2.text))
                        GetNextToken();

                    NextAction();
                    break;
            }

        }
        #endregion

        #region EndScene
        void FadeEnd(string scene)
        {
            StartCoroutine(LastFadeIn(scene));
        }

        IEnumerator LastFadeIn(string scene)
        {
            Color c = FirstBlackImage.color;
            float r = c.r, g = c.g, b = c.b;

            float count = 0;
            while (count < 1.0f)
            {
                count += 0.02f;
                yield return new WaitForSeconds(0.01f);
                FirstBlackImage.color = new Color(r, g, b, count);
            }

            PlayerPrefs.SetInt("Save", scene[0] - '0');

            SceneManager.LoadScene(scene);
        }
        #endregion

        #region Setting
        //설정 버튼, 돌아가기 버튼, esc 키(모바일 뒤로가기 키) 누르면 호출
        public void SettingBtn()
        {
            isSetting = !isSetting;
            setting_Panel.SetActive(isSetting);

            if (!isFade)
                nextBtn.SetActive(!isSetting);
        }

        //설정 판넬의 게임 종료 버튼 누르면 호출 -> 게임 종료
        public void ExitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}


