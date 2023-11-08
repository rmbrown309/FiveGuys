using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] Transform startMarker;
    [SerializeField] Transform endMarker;
    [SerializeField] float speed;
    [SerializeField] bool isOn;
    [SerializeField] bool isUp;
    float startTime;
    float destinationLength;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        destinationLength = Vector3.Distance(startMarker.position, endMarker.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Animate");
        if (isOn)
        {
            isUp = true;
            float distanceCovered = (Time.time - startTime) * speed;
            float completedDistance = distanceCovered / destinationLength;
            transform.position = Vector3.Lerp(startMarker.position, endMarker.position, completedDistance);
        }else if (!isOn && isUp)
        {
            if(gameObject.transform.position == startMarker.position)
            {
                isUp = false;

            }
            float distanceCovered = (Time.time - startTime) * speed;
          float completedDistance = distanceCovered / destinationLength;
          transform.position = Vector3.Lerp(endMarker.position, startMarker.position, completedDistance);
        }
    }
}
