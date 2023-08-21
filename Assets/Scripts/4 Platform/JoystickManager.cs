using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Yeol
{
    public enum PlatformState { Ready, Ingame, Pause, Respawn, Clear, FinalClear};

    public class JoystickManager : MonoBehaviour
    {
        public static JoystickManager instance = null;

        [SerializeField] Raccoon raccoon;

        [Header("Joystick")]
        [SerializeField] Transform joystick;
        Vector3 stickFirstPos;
        Vector3 joystickVec;
        float radius;

        Vector3 raccoonFirstPos;
        Vector3 cameraFirstPos;

        [Header("Obstacle")]
        [SerializeField] List<MovingObstacle> movingObstacles;
        [SerializeField] int[] movingObstaclesCount;

        [Header("Cherry")]
        [SerializeField] List<GameObject> cherrys;
        [SerializeField] int[] cherrysCount;

        [Header("UI")]
        [SerializeField] GameObject[] Ui; //0 : ready, 1 : gameover, 2 : clear
        [SerializeField] Image FirstBlackImage;
        [SerializeField] GameObject settingPanel;
        bool isSetting = false;

        [HideInInspector] public int nowCherry = 0;
        [HideInInspector] public int requireCherry;

        [HideInInspector] public int nowStage = 0;

        public PlatformState state = PlatformState.Ready;

        private void Awake()
        {
            instance = this;
        }

        IEnumerator FirstFadeOut()
        {
            Color c = FirstBlackImage.color;
            float r = c.r, g = c.g, b = c.b;
            float count = 1;
            while (count > 0.0f)
            {
                count -= 0.02f;
                yield return new WaitForSeconds(0.01f);
                FirstBlackImage.color = new Color(r, g, b, count);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(FirstFadeOut());
            radius = 98;
            raccoonFirstPos = raccoon.transform.position;
            cameraFirstPos = Camera.main.transform.position;
            stickFirstPos = joystick.position;
            state = PlatformState.Ready;

            Ui[0].SetActive(true);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (state == PlatformState.Ingame || state == PlatformState.Pause)
                    Btn_Setting();
            }
        }

        //최초 게임 시작 및 사망 후 재시작 시 호출
        public void IngameStart()
        {
            if (state == PlatformState.Ingame)
                return;
            state = PlatformState.Ingame;

            Ui[0].SetActive(false);
            Ui[1].SetActive(false);

            nowCherry = 0;
            requireCherry = cherrysCount[nowStage + 1] - cherrysCount[nowStage];

            for (int i = cherrysCount[nowStage]; i < cherrysCount[nowStage + 1]; i++)
                cherrys[i].SetActive(true);

            for (int i = movingObstaclesCount[nowStage]; i < movingObstaclesCount[nowStage + 1]; i++)
                movingObstacles[i].IngameStart();

            raccoon.transform.position = raccoonFirstPos + Vector3.right * 20 * nowStage;
            raccoon.IngameStart();
        }


        public void Gameover()
        {
            state = PlatformState.Respawn;
            Ui[1].SetActive(true);
        }

        public void StageClear()
        {
            //1, 2 스테이지 클리어
            if (nowStage < 2)
            {
                state = PlatformState.Clear;
            }
            //최종 스테이지 클리어
            else
            {
                state = PlatformState.FinalClear;
            }
            Ui[2].SetActive(true);
        }

        //한 스테이지 클리어 후, 다음 스테이지 진행 시 호출
        public void NextStage()
        {
            if (nowStage < 2)
            {
                Ui[2].SetActive(false);
                nowStage++;
                StartCoroutine(NextCoroutine());
            }
            else if (nowStage == 2)
            {
                nowStage++;
                LoadNextScene();
            }
        }

        #region middleClear
        IEnumerator NextCoroutine()
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

            raccoon.transform.position = raccoonFirstPos + Vector3.right * 20 * nowStage;
            Camera.main.transform.position = cameraFirstPos + Vector3.right * 20 * nowStage;
            Ui[0].SetActive(true);

            count = 1;
            while (count > 0.0f)
            {
                count -= 0.02f;
                yield return new WaitForSeconds(0.01f);
                FirstBlackImage.color = new Color(r, g, b, count);
            }

            state = PlatformState.Ready;
        }
        #endregion

        #region finalClear
        void LoadNextScene()
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

            PlayerPrefs.SetInt("Save", 5);
            SceneManager.LoadScene(5);
        }
        #endregion

        #region joystick
        public void Drag(BaseEventData _eventData)
        {
            if (state == PlatformState.Ingame)
            {
                PointerEventData Data = _eventData as PointerEventData;
                Vector3 Pos = Data.position;

                joystickVec = (Pos - stickFirstPos).normalized;

                float dis = Vector3.Distance(Pos, stickFirstPos);

                if(joystickVec.x > 0)
                {
                    raccoon.isMove = true;
                    raccoon.moveVec = Vector2.right;
                }
                else if(joystickVec.x < 0)
                {
                    raccoon.isMove = true;
                    raccoon.moveVec = Vector2.left;
                }

                if (joystickVec.y > 0.5f)
                    raccoon.Jump();

                if (dis < radius)
                    joystick.position = stickFirstPos + joystickVec * dis;
                else
                    joystick.position = stickFirstPos + joystickVec * radius;
            }
            else
                EndDrag();
        }

        public void EndDrag()
        {
            raccoon.isMove = false;
            raccoon.moveVec = Vector2.zero;
            joystick.position = stickFirstPos;
            joystickVec = Vector3.zero;
        }
        #endregion

        #region setting
        public void Btn_Setting()
        {
            if (!(state == PlatformState.Ingame || state == PlatformState.Pause))
                return;

            isSetting = !isSetting;
            settingPanel.SetActive(isSetting);

            Time.timeScale = isSetting ? 0 : 1;
            if (isSetting)
            {
                state = PlatformState.Pause;
                EndDrag();
            }
            else
                state = PlatformState.Ingame;
        }

        public void Btn_Quit()
        {
            Application.Quit();
        }
        #endregion
    }
}
