using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPower
{
    void JumpPower(int jumps);
    void SpeedBoost(float speed);

    void HealthRegen(float regen);
    void ShootRate(float shoot);
    void DamageUp(int damage);
}


