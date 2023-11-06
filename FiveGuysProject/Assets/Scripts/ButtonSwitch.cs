using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField] bool onTrigger;
    [Range(0,1)] [SerializeField] float ButtonUpHeight;
    [SerializeField] bool SwitchState;
    [Range(0, 1)][SerializeField] float buttonDownHeight;
    [Header("-----Button Stats------")]
    [SerializeField] int collectibleCost;
    [SerializeField] GameObject[] collectibles;
    [SerializeField] bool collectibleSpawning;
    [SerializeField] bool gameEnding;

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

            if ((collectibleCost == 0 || GameManager.instance.playerScript.GetCollectables() >= collectibleCost))
            {
                SwitchState = !SwitchState;
                Debug.Log("Switch In");
                gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, buttonDownHeight), gameObject.transform.rotation);

                GameManager.instance.playerScript.SetCollectables(-collectibleCost);
                if (gameEnding)
                    GameManager.instance.maxWaves = GameManager.instance.waves;
                if (collectibleSpawning && collectibles != null)
                {
                    foreach (var item in collectibles)
                        item.SetActive(true);
                }
                GameManager.instance.collectable.SetActive(true);
            }
        }
    }
    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.pickupText.text = "[E]";
            GameManager.instance.pickupLabel.SetActive(true);
            onTrigger = true;
            Debug.Log("Switch In");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger = false;
            GameManager.instance.pickupLabel.SetActive(false);
            Debug.Log("Switch In");
        }
    }
    public bool GetSwitchState()
    {
        return SwitchState;
    }
}
