using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePickup : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float rotationSpeed;
    [SerializeField] float duration;
    [SerializeField] float heightUp;
    [SerializeField] int itemsCollected;
    float rand;
    Vector3 origPosition;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rand = Random.Range(0.0f, 1.0f);
        origPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

        float newY = Mathf.Sin((Time.time + rand) * duration) * heightUp + origPosition.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<PlayerController>().SetCollectables(itemsCollected);
            if (player.GetComponent<PlayerController>().GetCollectables() > 0)
            {
                GameManager.instance.enableCollectable(true);
            }
            GameManager.instance.AddCurrentCollectables();
            Destroy(gameObject);
        }
        if(GameManager.instance.playerScript.GetCollectables() == 5)
        {
            GameManager.instance.SetQuestText("- Refuel the plane!");
        }
    }
}
