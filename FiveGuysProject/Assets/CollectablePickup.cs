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
    Vector3 origPosition;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        origPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

        float newY = Mathf.Sin((Time.time + 2) * duration) * heightUp + origPosition.y;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        player.GetComponent<PlayerController>().SetCollectables(itemsCollected);
        if (player.GetComponent<PlayerController>().GetCollectables() > 0)
        {
            GameManager.instance.enableCollectable(true);
        }
        Destroy(gameObject);
    }
}
