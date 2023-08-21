using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Yeol
{
    //Flappy Bird 게임 총괄 Manager, Bee에 할당
    public class BazzardGameManager : MonoBehaviour
    {
        //현재 게임 진행 상태 Ready -> Ingame -> Win or Lose
        FlappyState state = FlappyState.Ready;
        public int playTime = 0;

        //벌 rigidbody
        Rigidbody2D beeRigid;

        //UI 창 on, off를 위해 inspecter 창에서 할당
        [SerializeField] GameObject[] UIs;  //0 : ready, 1 : lose, 2 : win

        [Header("Pipe")]
        [SerializeField] GameObject pipePrefab;

        //파이프 pool, 게임 시작 시 일정량 생성 후 돌려 씀
        Pipe[] pipeInstances = new Pipe[10];
        int nowPipe = 0;
        //파이프 사이 간격(시간 단위) 초기값 : ~~, 최솟값 : ~~
        float pipeDelay = 2f;
        //파이프 속도                초기값 : ~~, 최솟값 : ~~
        float pipeSpeed = 5f;

        [Header("Image")]
        [SerializeField] Image FirstBlackImage;
        [SerializeField] GameObject stalkerSprite;
        [SerializeField] GameObject backgroundSprite;

        [Header("Setting")]
        [SerializeField] AudioSource bgm;
        [SerializeField] GameObject pauseUI;
        [SerializeField] AudioSource SFX;
        bool isFade = false;
        bool isPause = false;

        private void Start()
        {
            StartCoroutine(FirstFadeOut());
        }
        
        // 터치 입력 받기 - 터치 유지 시 y축 방향 힘 받기
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                PauseBtn();

            if (state == FlappyState.Ingame)
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (Input.touchCount > 0)
                    {
                        beeRigid.AddForce(Vector2.up * 15);
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        beeRigid.AddForce(Vector2.up * 15);
                    }
                }
            else if (state == FlappyState.Ready)
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (Input.touchCount > 0)
                    {
                        Touch t = Input.GetTouch(0);
                        if (t.phase == TouchPhase.Began)
                        {
                            OnStartBtn();
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        OnStartBtn();
                    }
                }
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

            isFade = false;
        }
        
        //최초 시작 시 호출, Pipe instance 생성, Pipe 이동 코루틴 호출
        void OnStartBtn()
        {
            beeRigid = GetComponent<Rigidbody2D>();
            beeRigid.gravityScale = 1;

            //readyUI 비활성화
            UIs[0].SetActive(false);

            bgm.Play();
            //파이프 생성
            PipeInstantiate();
            state = FlappyState.Ingame;

            //파이프 이동
            StartCoroutine(PipeMove());
            //시간 카운트
            StartCoroutine(TimeCount());
            //추적자 이미지 이동
            StartCoroutine(StalkerMove());
            //배경 이동
            StartCoroutine(BGMoveCoroutine());
        }

        #region Pipe
        //Pipe instance 생성
        void PipeInstantiate()
        {
            if (pipeInstances[0] != null)
                return;

            for (int i = 0; i < 10; i++)
            {
                pipeInstances[i] = Instantiate(pipePrefab).GetComponent<Pipe>();
                pipeInstances[i].gameObject.SetActive(false);
            }
        }

        //Pipe 이동 코루틴
        IEnumerator PipeMove()
        {
            while((state == FlappyState.Ingame || state == FlappyState.Pause) && playTime < 30)
            {
                pipeInstances[nowPipe].transform.position = new Vector3(11, Random.Range(-2.5f, 2), 0);
                pipeInstances[nowPipe].gameObject.SetActive(true);
                pipeInstances[nowPipe].StartMove(pipeSpeed);

                nowPipe = (nowPipe + 1) % 10;

                yield return new WaitForSeconds(pipeDelay);
            }
        }
        #endregion

        #region Judge
        IEnumerator TimeCount()
        {
            while (playTime < 30f)
            {
                playTime += 1;
                yield return new WaitForSeconds(1f);

                if(playTime % 5 == 0)
                {
                    pipeSpeed += 1;
                    pipeDelay -= 0.2f;
                }

                if (state == FlappyState.Gameover)
                    yield break;
            }


            int count;
            do
            {
                count = 0;
                for (int i = 0; i < 10; i++)
                    if (pipeInstances[i].isActiveAndEnabled)
                        count++;
                yield return new WaitForSeconds(0.03f);

                if (state == FlappyState.Gameover)
                    yield break;
            } while (count != 0);

            state = FlappyState.ClearAnim;
            StartCoroutine(StalkerCatchMiss());
        }

        //충돌 처리, 장애물 충돌 시 게임 오버
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Pipe")
            {
                GameOver();
            }
        }
        #endregion

        public void PauseBtn()
        {
            if (state == FlappyState.Gameover)
                return;

            isPause = !isPause;
            pauseUI.SetActive(isPause);

            if(isPause)
            {
                bgm.Pause();
                Time.timeScale = 0;
                state = FlappyState.Pause;
            }
            else
            {
                bgm.Play();
                Time.timeScale = 1;
                state = FlappyState.Ingame;
            }

        }

        IEnumerator BGMoveCoroutine()
        {
            while (!(state == FlappyState.Gameover || state == FlappyState.Clear))
            {
                backgroundSprite.transform.Translate(Vector2.left * 0.02f);
                yield return new WaitForSeconds(0.01f);
            }
        }

        #region Bazzard
        //OnStartBtn 함수에서 호출, 추적자 이미지 이동 시작
        IEnumerator StalkerMove()
        {
            while ((state == FlappyState.Ingame || state == FlappyState.Pause) && playTime < 30)
            {
                stalkerSprite.transform.Translate(Vector3.left * 0.01f);
                yield return new WaitForSeconds(0.03f);
            }
        }

        //GameOver 함수에서 호출, 추적자 추적 성공
        IEnumerator StalkerCatch()
        {
            while (stalkerSprite.transform.position.x < -7.1f)
            {
                stalkerSprite.transform.Translate(Vector3.right * 0.1f);

                yield return null;
            }
        }

        //TimeCount 코루틴 끝에 호출, 추적자 추적 실패
        IEnumerator StalkerCatchMiss()
        {
            beeRigid.gravityScale = 0;
            beeRigid.velocity = new Vector2(0, 0);

            while (stalkerSprite.transform.position.x < -8f)
            {
                stalkerSprite.transform.Translate(Vector3.right * 0.06f);
                yield return null;
            }

            beeRigid.gravityScale = 0;
            beeRigid.velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(0.1f);

            while (stalkerSprite.transform.position.x > -20f)
            {
                stalkerSprite.transform.Translate(Vector3.left * 0.07f);
                yield return null;
            }

            GameClear();
        }
        #endregion

        #region GameEnd
        void GameOver()
        {
            bgm.Stop();

            SFX.Play();

            state = FlappyState.Gameover;

            beeRigid.gravityScale = 0;
            beeRigid.velocity = Vector2.zero;

            for (int i = 0; i < 10; i++)
                pipeInstances[i].isGameover = true;

            UIs[1].SetActive(true);
            Debug.Log("Game over");

            StartCoroutine(StalkerCatch());
        }

        void GameClear()
        {
            state = FlappyState.Clear;

            UIs[2].SetActive(true);
        }

        public void RetryBtn()
        {
            SceneManager.LoadScene(8);
        }

        public void NextStageBtn()
        {
            if (isFade)
                return;
            isFade = true;

            PlayerPrefs.SetInt("Save", 9);
            StartCoroutine(LastFadeIn());
        }

        public void QuitBtn()
        {
            Application.Quit();
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

             SceneManager.LoadScene(9);
        }
        #endregion
    }
}
