using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{

    [SerializeField] GameObject quitButton;

    private Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();

        if (Application.platform == RuntimePlatform.WebGLPlayer)
            Destroy(quitButton);
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            if (Input.GetButtonUp("Shoot"))
            {
                mAnimator.SetTrigger("isClicked");
                mAnimator.SetBool("contText", false);
            }
        }
    }
}
