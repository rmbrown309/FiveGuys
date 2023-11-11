using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] SavedSettings savedSettings;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;

    float xRot;
    float mouseY;
    float mouseX;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // get input
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(2))
        {
            mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * savedSettings.Sensitivity * 1000;
            mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * savedSettings.Sensitivity * 1000;
        }
        else
        {
            mouseY = (Input.GetAxis("Mouse Y") * Time.deltaTime * savedSettings.Sensitivity * 1000) / 4;
            mouseX = (Input.GetAxis("Mouse X") * Time.deltaTime * savedSettings.Sensitivity * 1000) / 3;
        }

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

    public IEnumerator ShakeCam(float duration, float strength)
    {
        Vector3 origPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 2) * strength;
            float y = Random.Range(-1, 2) * strength;

            transform.localPosition = new Vector3(x, y + origPos.y, origPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = origPos;
    }
}
