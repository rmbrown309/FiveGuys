using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class HudAnimate : MonoBehaviour
{
    private Animator mAnimator;
    [SerializeField] GameObject mainWeapon;
    [SerializeField] GameObject jumpPower;
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
            if (GameManager.instance.jumpPower)
            {
                mAnimator.SetBool("isJumpOn", true);
            }
            else
            {
                mAnimator.SetBool("isJumpOn", false);
            }
            if (GameManager.instance.speedPower)
            {
                mAnimator.SetBool("isSpeedOn", true);
            }
            else
            {
               mAnimator.SetBool("isSpeedOn", false);
            }
            if (GameManager.instance.invulnerablePower)
            {
                mAnimator.SetBool("isInvulPowerOn", true);
            }
            else
            {
                mAnimator.SetBool("isInvulPowerOn", false);
            }
            if (GameManager.instance.fireSpeedPower)
            {
                mAnimator.SetBool("isFireRatePowerOn", true);
            }
            else
            {
                mAnimator.SetBool("isFireRatePowerOn", false);
            }
        }
    }
    public void TurnOffMainWeapon()
    {
        mAnimator.SetBool("isBaseGunOff",true);
    }
    public void TurnOnMainWeapon()
    {
        mAnimator.SetBool("isBaseGunOff", false);
    }
    public void TurnOffSubWeapon()
    {
        mAnimator.SetBool("isSubGunOn", false);

    }
    public void TurnOnSubWeapon()
    {
        mAnimator.SetBool("isSubGunOn", true);
    }
    public void TurnOffSnipeWeapon()
    {
        mAnimator.SetBool("isSniperOn", false);

    }
    public void TurnOnSnipeWeapon()
    {
        mAnimator.SetBool("isSniperOn", true);

    }
    public void TurnOnShotGunWeapon()
    {
        mAnimator.SetBool("isShotGunOn", true);

    }
    public void TurnOffShotGunWeapon()
    {
        mAnimator.SetBool("isShotGunOn", false);
    }
    public void TurnOnPowerWeapons()
    {
        mAnimator.SetBool("isPowerGunOn", true);
    }
    public void TurnOffPowerWeapons()
    {
        mAnimator.SetBool("isPowerGunOn", false);
    }
}
