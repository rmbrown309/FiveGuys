using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.GraphicsBuffer;

public class TrackingProjectiles : MonoBehaviour, ISpray
{
    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem bugRelease;
    //[SerializeField] float smallBug;
    //[SerializeField] float bigBug;
    [SerializeField] SavedSettings settings;

    [Header("----- Bullet Stats -----")]
    public int damage;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    public float rotateSpeed = 200f;

    [Header("----- Bug Audio -----")]
    [SerializeField] AudioSource aud;
    [Range(0, 1)][SerializeField] float idleBuzzVol;
    [SerializeField] AudioClip[] idleBuzz;
    [Range(0, 1)][SerializeField] float idleBuzzPlayPercentage;
    [SerializeField] float idleBuzzCoolDown;

    bool hasExpanded = false;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
    //Bullet Tracker; Update is hopefully called once per frame
    private void Update()
    {
        Vector3 direction = GameManager.instance.player.transform.position - rb.position;
        direction.Normalize();
        Vector3 rotateAmount = Vector3.Cross(direction, transform.forward);
        rb.angularVelocity = -rotateAmount * rotateSpeed * Time.deltaTime;
        rb.velocity = speed * Time.deltaTime * (GameManager.instance.player.transform.position - transform.position);
        if (hasExpanded == false)
        {
            //StartCoroutine(Expand());
        }
        if (Random.value < idleBuzzPlayPercentage)
        {
            StartCoroutine(RandomIdleChat());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage damageable = other.GetComponent<IDamage>();
        damageable?.takeDamage(damage);
        Destroy(gameObject);
    }
    public void setDestroyTime(int time)
    {
        destroyTime = time;
    }
    IEnumerator RandomIdleChat()
    {
        if (Random.value < idleBuzzPlayPercentage)
        {
            float randPitch = Random.Range(0.95f, 1.05f);
            aud.pitch = randPitch;
            aud.PlayOneShot(idleBuzz[Random.Range(0, idleBuzz.Length)], settings.SoundEffectVoulume / 20);
        }
        yield return new WaitForSeconds(idleBuzzCoolDown);
    }
    void ISpray.kill()
    {
        Destroy(gameObject);
    }
    //Alternative expanding version
    //IEnumerator Expand()
    //{
    //    yield return ScaleBug(bigBug * Vector3.one);
    //    yield return ScaleBug(smallBug * Vector3.one);
    //}
    //private IEnumerator ScaleBug(Vector3 finalSize)
    //{
    //    bool expanded = false;
    //    Vector3 vel = Vector3.zero;
    //    while (!expanded)
    //    {
    //        transform.localScale = Vector3.SmoothDamp(transform.localScale, finalSize, ref vel, speed);
    //        float distance = Vector3.Distance(transform.localScale, finalSize);
    //        if (distance <= 0.1)
    //        {
    //            expanded = true;
    //        }
    //        yield return null;
    //    }
    //    hasExpanded = true;
    //}
}