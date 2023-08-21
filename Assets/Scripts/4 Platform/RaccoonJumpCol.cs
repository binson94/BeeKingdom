using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yeol
{
    public class RaccoonJumpCol : MonoBehaviour
    {
        Raccoon raccoon;
        // Start is called before the first frame update
        void Start()
        {
            raccoon = transform.parent.GetComponent<Raccoon>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Platform")
                raccoon.isJump = false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Platform")
                raccoon.isJump = true;
        }
    }
}
