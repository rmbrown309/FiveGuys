using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudAnimate : MonoBehaviour
{
    private Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        if(mAnimator != null)
        {
            mAnimator.SetTrigger("isStart");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            if (Input.GetButtonDown("Tab"))
            {
                mAnimator.SetBool("isInformed", true);
            }
            if (Input.GetButtonUp("Tab"))
            {
                mAnimator.SetBool("isInformed", false);
            }
        }
    }
}
