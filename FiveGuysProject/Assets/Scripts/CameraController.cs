using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] SavedSettings savedSettings;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    float xRot;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // get input
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * savedSettings.Sensitivity * 1000;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * savedSettings.Sensitivity * 1000;

        if (invertY)
            xRot += mouseY;
        else
            xRot -= mouseY;

        // clamp the rotation on the X-axis
        xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);

        // rotate the camera on the X-axis
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        // rotate the player on the Y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }

}
