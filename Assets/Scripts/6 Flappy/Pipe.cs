using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yeol
{

    public class Pipe : MonoBehaviour
    {
        //pipe speed, FlappyBirdManager에서 호출 시 값 넘겨줌
        float speed;
        public bool isGameover = false;

        //FlappyBirdManager에서 호출, 속도 값을 받아 MmoveCoroutine 시작
        public void StartMove(float speed)
        {
            this.speed = speed;
            StartCoroutine(MoveCoroutine());
        }

        //장애물 이동 코루틴
        IEnumerator MoveCoroutine()
        {
            while (isActiveAndEnabled && transform.position.x > -11.5f)
            {
                transform.Translate(Vector2.left * speed * 0.03f);
                yield return new WaitForSeconds(0.03f);

                if (isGameover)
                    yield break;
            }

            gameObject.SetActive(false);
        }
    }
}
