using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Yeol
{
    public class IntroManager : MonoBehaviour
    {
        List<Script> scripts = new List<Script>();

        [SerializeField] Sprite[] backgroundSprites;

        [SerializeField] Image blackImage;

        int backIdx = 0;
        [SerializeField] Image backgroundimage;
        [SerializeField] Text text;
        [SerializeField] Image text_Panel;
        bool isFade = false;

        [SerializeField] GameObject nextBtn;
        [SerializeField] GameObject logBtn;

        void Start()
        {
            ScriptInitialization();

            backgroundimage.sprite = backgroundSprites[0];
            text.text = "우리 꿀벌 왕국은 좋은 자리를 잡아 번영해나가고 있었어.";

            isFade = true;
            StartCoroutine(FirstFadeOut());
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

        void ScriptInitialization()
        {
            scripts.Add(new Script(0, "나는 언제나처럼 왕국을 위해 꿀을 찾아 나섰지."));
            scripts.Add(new Script(1, "꿀을 찾고 왕국으로 돌아가던 와중에 다친 우리 왕국의 꿀벌을 만났어."));
            scripts.Add(new Script(0, "다친 꿀벌은 우리 왕국이 말벌에게 습격당했다고 말했어."));
            scripts.Add(new Script(1, "왕국으로 돌아온 나는 충격적인 광경을 접했어,"));
            scripts.Add(new Script(0, "벌집은 파괴되어 있었고, 동료들은 여왕님이 말벌에게 납치당하셨다며 울고 있었지."));
            scripts.Add(new Script(0, "왕국을 위해서 하루빨리 벌집을 재건하고 여왕님을 되찾아야 해!"));
        }
        
        IEnumerator FirstFadeOut()
        {
            isFade = true;
            Color c = blackImage.color;
            float r = c.r, g = c.g, b = c.b;
            float count = 1;
            while (count > 0.0f)
            {
                count -= 0.02f;
                yield return new WaitForSeconds(0.01f);
                blackImage.color = new Color(r, g, b, count);
            }

            isFade = false;
            nextBtn.SetActive(true);
        }

        public void Btn_Next()
        {
            nextBtn.SetActive(false);
            FadeOut();
        }

        //투명해짐
        void FadeOut()
        {
            isFade = true;
            if (scripts.Count == 0)
                FadeEnd();
            else
            {
                Script script = scripts[0];

                if (script.cutIdx == 1)
                {
                    backIdx++;
                    StartCoroutine(FICoroutine());
                }
                else
                {
                    StartCoroutine(TextFOCoroutine());
                }
            }
        }

        IEnumerator FICoroutine()
        {
            Color c = blackImage.color;
            float r = c.r, g = c.g, b = c.b;

            float count = 0;
            while (count < 1.0f)
            {
                count += 0.02f;
                yield return new WaitForSeconds(0.01f);
                blackImage.color = new Color(r, g, b, count);
            }

            ImageUpdate();
        }

        void ImageUpdate()
        {
            Script script = GetNextToken();

            backgroundimage.sprite = backgroundSprites[backIdx];
            text.text = script.str;

            StartCoroutine(FOCoroutine());
        }

        IEnumerator FOCoroutine()
        {
            Color c = blackImage.color;
            float r = c.r, g = c.g, b = c.b;
            float count = 1;
            while (count > 0.0f)
            {
                count -= 0.02f;
                yield return new WaitForSeconds(0.01f);
                blackImage.color = new Color(r, g, b, count);
            }

            isFade = false;
            nextBtn.SetActive(true);
        }
        

        IEnumerator TextFOCoroutine()
        {
            float count = 1;
            while (count > 0.0f)
            {
                count -= 0.02f;
                yield return new WaitForSeconds(0.01f);
                text.color = new Color(0, 0, 0, count);
            }

            TextUpdate();
        }

        void TextUpdate()
        {
            Script script = GetNextToken();

            text.text = script.str;

            StartCoroutine(TextFICoroutine());
        }

        IEnumerator TextFICoroutine()
        {
            float count = 0;
            while (count < 1)
            {
                count += 0.02f;
                yield return new WaitForSeconds(0.01f);
                text.color = new Color(0, 0, 0, count);
            }

            isFade = false;
            nextBtn.SetActive(true);
        }

        void FadeEnd()
        {
            StartCoroutine(LastFadeIn());
        }

        IEnumerator LastFadeIn()
        {
            Color c = blackImage.color;
            float r = c.r, g = c.g, b = c.b;

            float count = 0;
            while (count < 1.0f)
            {
                count += 0.02f;
                yield return new WaitForSeconds(0.01f);
                blackImage.color = new Color(r, g, b, count);
            }

            backgroundimage.sprite = backgroundSprites[3];
            text_Panel.color = new Color(1, 1, 1, 0);
            text.color = new Color(0, 0, 0, 0);
            text.text = "현재 일할 수 있는 일벌은 얼마나 돼?";
            logBtn.SetActive(true);
            GetComponent<CutSceneManager>().StartFade();
        }
    }
}


