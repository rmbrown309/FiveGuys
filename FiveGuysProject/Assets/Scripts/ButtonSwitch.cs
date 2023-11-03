using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField] bool onTrigger;
    [Range(0,1)] [SerializeField] float ButtonUpHeight;
    [SerializeField] bool SwitchState;
    [Range(0, 1)][SerializeField] float buttonDownHeight;
    // Start is called before the first frame update
    void Start()
    {
       // Debug.Log("Switch In");
    }
    private void Update()
    {
        //buttonSet = false;
        //if (Input.GetButtonUp("Interact") && SwitchState == true && onTrigger )
        //{
        //    SwitchState = !SwitchState;
        //    Debug.Log("Switch off");
        //    gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, ButtonUpHeight), gameObject.transform.rotation);
        //
        //}
       if (Input.GetButtonUp("Interact") && SwitchState == false && onTrigger)
       {
           SwitchState = !SwitchState;
           Debug.Log("Switch In");
           gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, buttonDownHeight), gameObject.transform.rotation) ;
       }
       
    }
    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger = true;
            Debug.Log("Switch In");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger = false;
            Debug.Log("Switch In");
        }
    }
    public bool GetSwitchState()
    {
        return SwitchState;
    }
}
