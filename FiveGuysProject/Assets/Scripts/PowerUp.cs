using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerUp : MonoBehaviour
{

    [SerializeField] float destroyTimer;
    [SerializeField] int typePower;
    [SerializeField] int newJumpMax;
    [SerializeField] int newSpeedMax;
    [SerializeField] int newEnemyHealthDown;
    [SerializeField] float newShootRate;
    [SerializeField] float newRegen;
    [SerializeField] bool randType;
    [SerializeField] float rotationSpeed;
    [SerializeField] float duration;
    [SerializeField] float heightUp;
    Vector3 origPosition;
    // Start is called before the first frame update
    void Start()
    {
        origPosition = transform.position;
        Destroy(gameObject, destroyTimer);
    }
    private void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

        float newY = Mathf.Sin(Time.time * duration) * heightUp + origPosition.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }


    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.transform != this.transform)
        {
            return;
        }
        IPower powerUp = other.GetComponent<IPower>();
        if (powerUp != null)
        {
            if(randType)
            {
                typePower = Random.Range(1, 6);
                Debug.Log(typePower);
            }
            switch (typePower)
            {
                case 1:
                    Debug.Log("jump");
                    powerUp.JumpPower(newJumpMax);
                    break;
                case 2:
                    Debug.Log("speed");

                    powerUp.SpeedBoost(newSpeedMax);
                    break;
                case 3:
                    Debug.Log("health");

                    powerUp.Invulnerability();
                    break;
                case 4:
                    Debug.Log("shoot");

                    powerUp.ShootRate(newShootRate);
                    break;
                case 5:
                    Debug.Log("dmg");

                    powerUp.EnemyHealthDown(newEnemyHealthDown);
                    break;
                case 6:
                    Debug.Log("dmg");

                    powerUp.EnemyHealthDown(newEnemyHealthDown);
                    break;
            }
        }
        Destroy(gameObject);
    }
}
