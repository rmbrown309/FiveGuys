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
    [SerializeField] GameObject hudSound;
    [SerializeField] AudioClip[] pickedUpSound;
    float rand;
    Vector3 origPosition;
    // Start is called before the first frame update
    void Start()
    {
        rand = Random.Range(0.0f, 1.0f);
        origPosition = transform.position;
        if(randType)
            Destroy(gameObject, destroyTimer);
    }
    private void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        
        float newY = Mathf.Sin((Time.time + rand) * duration) * heightUp + origPosition.y;
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
                    //Debug.Log("jump");
                    
                    powerUp.JumpPower(newJumpMax);
                    Destroy(gameObject);
                    break;
                case 2:
                    //Debug.Log("speed");
                    powerUp.SpeedBoost(newSpeedMax);

                    Destroy(gameObject);
                    break;
                case 3:
                    //Debug.Log("health");
                    powerUp.Invulnerability();

                    Destroy(gameObject);
                    break;
                case 4:
                    //Debug.Log("shoot");
                    powerUp.ShootRate(newShootRate);

                    Destroy(gameObject);
                    break;
                case 5:
                    //Debug.Log("dmg");
                    //powerUp.EnemyHealthDown(newEnemyHealthDown);
                    //
                    //Destroy(gameObject);
                    //GameManager.instance.weaponPower = true;
                    powerUp.SuperWeapons();
                    Destroy(gameObject);
                    break;
                case 6:
                    powerUp.AmmoRefillPower();

                    StartCoroutine(Reactivate());
                    break;
            }
        }
    }

    IEnumerator Reactivate()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(15);
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;
    }

}
