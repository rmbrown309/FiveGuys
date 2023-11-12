using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitch : MonoBehaviour
{
    [SerializeField] bool onTrigger;
    [Range(0, 1)][SerializeField] float ButtonUpHeight;
    [SerializeField] bool SwitchState;
    [Range(0, 1)][SerializeField] float buttonDownHeight;
    [Header("-----Button Stats------")]
    [SerializeField] int collectibleCost;
    [SerializeField] GameObject[] collectibles;
    [SerializeField] bool collectibleSpawning;
    [SerializeField] bool gameEnding;
    [SerializeField] Material matt;
    [SerializeField] GameObject buttonAudio;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.SetQuestText("- Find the control station");
        GameManager.instance.SetQuestState(true);
        // Debug.Log("Switch In");
        matt.SetColor("_Color", Color.red);
        matt.SetColor("_EmissionColor", Color.red);
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

            if (gameEnding && GameManager.instance.playerScript.GetCollectables() < collectibleCost)
            {
                buttonAudio.GetComponent<AudioSource>().PlayOneShot(buttonAudio.GetComponent<AudioSource>().clip);
                GameManager.instance.SetFindButton();
            }
            if ((collectibleCost == 0 || GameManager.instance.playerScript.GetCollectables() >= collectibleCost))
            {
                SwitchState = !SwitchState;
                Debug.Log("SetColor");
                matt.SetColor("_Color", Color.green);
                matt.SetColor("_EmissionColor", Color.green);


                gameObject.transform.SetLocalPositionAndRotation(new Vector3(0, buttonDownHeight), gameObject.transform.rotation);

                GameManager.instance.playerScript.SetCollectables(-collectibleCost);
                if (gameEnding)
                {
                    GameManager.instance.maxWaves = GameManager.instance.waves;
                    GameManager.instance.SetQuestState(true);
                }
                if (collectibleSpawning && collectibles != null)
                {
                    buttonAudio.GetComponent<AudioSource>().PlayOneShot(buttonAudio.GetComponent<AudioSource>().clip);
                    GameManager.instance.SetQuestText("- Find the gas cans");
                    GameManager.instance.SetFindButton("Find the gas cans, and refuel the plane!");
                    //GameManager.instance.SetPowerText("Find the gas cans");

                    foreach (var item in collectibles)
                        item.SetActive(true);
                }
                GameManager.instance.collectable.SetActive(true);
                GameManager.instance.collectablesActive = true;
            }
        }
    }
    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.pickupText.text = "[E]";
            GameManager.instance.pickupLabelContainer.SetActive(true);
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
            GameManager.instance.pickupLabelContainer.SetActive(false);
            GameManager.instance.pickupLabel.SetActive(false);
            Debug.Log("Switch In");
        }
    }
    public bool GetSwitchState()
    {
        return SwitchState;
    }
}
