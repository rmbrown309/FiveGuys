using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsEnd : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetButton("Shoot")) {
            SceneManager.LoadScene(0);
        }
    }
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CreditHead"))
        {
            
            SceneManager.LoadScene(0);
        }
    }
    
}
