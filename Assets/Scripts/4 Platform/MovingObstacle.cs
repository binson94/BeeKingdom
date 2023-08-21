using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yeol {

    public class MovingObstacle : MonoBehaviour
    {
        [SerializeField] float[] moveRange;
        Vector2 moveDir = Vector2.right;
        [SerializeField] float speed = 3f;
        Vector3 startPos;

        Animator anim;
        SpriteRenderer sprite;

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
            if (anim)
                anim.SetBool("isIngame", false);
            startPos = transform.position;
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        //게임 시작 시, 사망 후 재시작 시 호출
        public void IngameStart()
        {
            if (anim)
                anim.SetBool("isIngame", true);

            transform.position = startPos;
            sprite.flipX = false;
            moveDir = Vector2.right;
            StartCoroutine(MoveCoroutine());
        }

        IEnumerator MoveCoroutine()
        {
            while (JoystickManager.instance.state == PlatformState.Ingame || JoystickManager.instance.state == PlatformState.Pause)
            {
                transform.Translate(moveDir * speed * Time.deltaTime);

                if (transform.localPosition.x < moveRange[0])
                {
                    moveDir = Vector2.right;
                    sprite.flipX = false;
                }
                else if (transform.localPosition.x > moveRange[1])
                {
                    moveDir = Vector2.left;
                    sprite.flipX = true;
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            
            if (anim)
                anim.SetBool("isIngame", false);
        }
    }
}
