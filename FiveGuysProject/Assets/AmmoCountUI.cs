using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AmmoCountUI : MonoBehaviour
{
    [SerializeField] Image _bar;
    [SerializeField] RectTransform button;

    [SerializeField] float ammoVal = 0;
    [SerializeField] float ammoCurr;
    [SerializeField] float ammoMax;
    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        ammoCurr = GameManager.instance.GetAmmo();
        ammoMax = GameManager.instance.GetAmmoMax();
        ammoVal = ammoCurr /ammoMax;
        AmmoChange(ammoVal);
    }
    void AmmoChange(float ammoValue)
    {
        float amount = (ammoVal / .5f) * 180f / 360;
        _bar.fillAmount = amount;
    }
}
