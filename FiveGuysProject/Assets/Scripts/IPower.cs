using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPower
{
    void JumpPower(int jumps);
    void SpeedBoost(float speed);
    void Invulnerability();
    void ShootRate(float shoot);
    void EnemyHealthDown(int damage);
}


