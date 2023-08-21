using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yeol
{
    public class Raccoon : MonoBehaviour
    {
        public bool isMove = false;
        public Vector3 moveVec;
        float speed = 6f;
        public bool isJump = false;
        Rigidbody2D raccoonRigid;
        Collider2D raccoonCol;
        SpriteRenderer raccoonSprite;

        // Start is called before the first frame update
        void Start()
        {
            raccoonSprite = GetComponentInChildren<SpriteRenderer>();
            raccoonRigid = GetComponent<Rigidbody2D>();
            raccoonCol = GetComponent<Collider2D>();
        }

        //최초 시작 시, 사망 후 재시작 시 호출 - 중력값 및 점프 초기화
        public void IngameStart()
        {
            raccoonCol.isTrigger = false;
            raccoonRigid.gravityScale = 1;
            isJump = false;
            StartCoroutine(MoveCoroutine());
        }

        IEnumerator MoveCoroutine()
        {
            float minx = -8.1f + 20 * JoystickManager.instance.nowStage;
            float maxx = 8.1f + 20 * JoystickManager.instance.nowStage;
            while (JoystickManager.instance.state == PlatformState.Ingame || JoystickManager.instance.state == PlatformState.Pause)
            {
                if (isMove)
                    if (!((transform.position.x < minx && moveVec.x < 0) || (transform.position.x > maxx && moveVec.x > 0)))
                    {
                        if (moveVec.x < 0)
                            raccoonSprite.flipX = true;
                        else if (moveVec.x > 0)
                            raccoonSprite.flipX = false;

                        transform.Translate(moveVec * speed * Time.deltaTime);
                    }
                yield return new WaitForSeconds(Time.deltaTime);

                raccoonCol.isTrigger = isJump;
            }
        }

        public void Jump()
        {
            if (!isJump && Mathf.Abs(raccoonRigid.velocity.y) < 0.2f)
            {
                isJump = true;
                raccoonRigid.AddForce(Vector2.up * 350);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (JoystickManager.instance.state == PlatformState.Ingame)
                if (other.gameObject.tag == "Obstacle")
                {
                    raccoonRigid.gravityScale = 0;
                    raccoonRigid.velocity = Vector2.zero;
                    JoystickManager.instance.Gameover();
                }
                else if (other.gameObject.tag == "Flag" && JoystickManager.instance.nowCherry >= JoystickManager.instance.requireCherry)
                {
                    raccoonRigid.gravityScale = 0;
                    raccoonRigid.velocity = Vector2.zero;
                    JoystickManager.instance.StageClear();
                }
                else if (other.gameObject.tag == "Cherry")
                {
                    other.gameObject.SetActive(false);
                    JoystickManager.instance.nowCherry++;
                }
        }
    }
}
