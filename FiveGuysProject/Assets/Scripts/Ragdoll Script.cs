using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollScript : MonoBehaviour
{
    [SerializeField] float despawnTime;
    [SerializeField] GameObject ragdoll;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
 

        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}
