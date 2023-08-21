using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

namespace Yeol
{
    public class EndingManager : MonoBehaviour
    {
        //23 : bgm 변경, 24 : 효과음 25 : 씬 변경
        //31 : 나레이션
        List<Script> scripts = new List<Script>();

        [Header("Story")]
        [SerializeField] GameObject nextBtn;
        [SerializeField] Image FirstBlackImage;
        [SerializeField] Text charText;
        [SerializeField] Image text_Panel;

        [Header("BG")]
        [SerializeField] Image bgImage;
        [SerializeField] Sprite[] bgSprites;
        bool isFade = false;

        [Header("Setting")]
        [SerializeField] GameObject setting_Panel;
        bool isSetting = false;

        [Header("Sound")]
        [SerializeField] AudioSource BGM;
        [SerializeField] AudioSource SFX;
        [SerializeField] AudioClip[] BGMList;
        [SerializeField] AudioClip[] SFXList;

        void Start()
        {
            ScriptLoad();

            isFade = true;
            StartFade();
        }

        #region Data
        void ScriptLoad()
        {
            TextAsset txtAsset;
            string loadStr;
            JsonData json;

            string loadJsonName;
            if (PlayerPrefs.GetString("ButterflyGame") == "음~. 아름다운 내게 딱 어울리는 맛이야. 달지만 깊이 있는 이 맛은 고풍스럽기도 하지만, 화려하기도 하지.")
            {
                if (PlayerPrefs.GetString("Bazzard") == "1")
                {
                    loadJsonName = "HappyEnding";
                    bgImage.sprite = bgSprites[0];
                }
                else
                {
                    loadJsonName = "Normal1Ending";
                    bgImage.sprite = bgSprites[1];
                }
            }
            else
            {
                if (PlayerPrefs.GetString("Bazzard") == "1")
                {
                    loadJsonName = "Normal2Ending";
                    bgImage.sprite = bgSprites[2];
                }
                else
                {
                    loadJsonName = "BadEnding";
                    bgImage.sprite = bgSprites[3];
                }
            }

            txtAsset = Resources.Load<TextAsset>(string.Concat("Jsons/", loadJsonName));
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

            
            //bgm 변경
            if (s.cutIdx == 23)
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
                FadeEnd();
            }
            //이미지 페이드 인
            else if (s.cutIdx == 29)
            {
                StartCoroutine(ImageFICoroutine(text_Panel));

                NextAction();
            }
            //이미지 페이드 아웃
            else if (s.cutIdx == 30)
            {
                StartCoroutine(ImageFOCoroutine(text_Panel));

                NextAction();
            }
            //나레이션
            else if (s.cutIdx == 31)
            {
                StartCoroutine(TextFOCoroutine(s.str));
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
        
        #region EndScene
        void FadeEnd()
        {
            StartCoroutine(LastFadeIn());
        }

        IEnumerator LastFadeIn()
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

            PlayerPrefs.DeleteKey("Save");
            SceneManager.LoadScene(0);
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


