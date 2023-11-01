using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float rotationXSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        transform.Rotate(Vector3.left * (rotationXSpeed * Time.deltaTime));
    }
}
