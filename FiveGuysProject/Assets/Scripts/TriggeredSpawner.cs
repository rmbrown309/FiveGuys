using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredSpawner : MonoBehaviour
{
    [Header("-----Components------")]
    [SerializeField] GameObject switchObject;
    [SerializeField] Transform[] posToSpawn;
    [SerializeField] GameObject[] enemy; // enemy type spawned by this spawner
    [Header("-----Spawner Stats------")]
    [SerializeField] int numOfEnemies; // number of enemies spawned during the wave
    [SerializeField] float spawnRate; // seconds between each emnemy spawn
    [SerializeField] float spawnRange;
    [SerializeField] bool rats;

    bool isWaveActive = true;
    bool isTriggered = false;

    // Update is called once per frame
    void Update()
    {
        
        if(switchObject.GetComponentInChildren<ButtonSwitch>() != null)
        {
            if (switchObject.GetComponentInChildren<ButtonSwitch>().GetSwitchState())
            {
                gameObject.GetComponent<SphereCollider>().enabled = true;
            }
        }
        // Spawns enemies only on the correct wave number
        if (isTriggered && isWaveActive)
        {
            StartCoroutine(TotalEnemy());
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }

    IEnumerator TotalEnemy()
    {
        // prevents spawner from starting again during the wave
        isWaveActive = false;

        Vector3 spawnpoint = posToSpawn[0].position;

        // increments remaining enemies by the amount of enemies about to spawn
        if (!rats)
            GameManager.instance.UpdateWinCondition(numOfEnemies);

        // spawns the specified enemies at the specified rate
        for (int i = 0; i < numOfEnemies; i++)
        {
            if (posToSpawn.Length > 1)
            {
                Transform furthestSpawn = posToSpawn[0];
                for (int j = 1; j < posToSpawn.Length; j++)
                {
                    // prioritize the spawnpoint furthest from the player in order to keep enemies from spawning on top of them
                    if ((posToSpawn[j].position - GameManager.instance.player.transform.position).magnitude > (furthestSpawn.position - GameManager.instance.player.transform.position).magnitude)
                        furthestSpawn = posToSpawn[j];
                }

                // randomly choose a spawnpoint around the spawner transform in a circle of radius spawnRange
                spawnpoint = furthestSpawn.position + Random.insideUnitSphere * spawnRange;
                spawnpoint.y = furthestSpawn.position.y;
            }
            else
            {
                spawnpoint += Random.insideUnitSphere * spawnRange;
                spawnpoint.y = posToSpawn[0].position.y;
            }

            Instantiate(enemy[Random.Range(0, enemy.Length)], spawnpoint, Quaternion.identity);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            isTriggered = true;
        }
    }
}
