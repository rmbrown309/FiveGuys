using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    [Header("-----Components------")]
    [SerializeField] Transform[] posToSpawn;
    [SerializeField] GameObject[] enemy; // enemy type spawned by this spawner
    [Header("-----Spawner Stats------")]
    [Range(1, 20)][SerializeField] int waveStart; // On what wave this spawner is active
    [SerializeField] int numOfEnemies; // number of enemies spawned during the wave
    [SerializeField] float spawnRate; // seconds between each emnemy spawn
    [SerializeField] float spawnRange;
    [SerializeField] bool rats;
    [SerializeField] bool continuousSpawning;
    [SerializeField] bool finalBossRoom;
  
    bool spawnStopped = false;
    bool isWaveActive = true;

 
    // Update is called once per frame
    void Update()
    {
        // Spawns enemies only on the correct wave number
        if (isWaveActive && GameManager.instance.waves == waveStart)
        {
            StartCoroutine(TotalEnemy(numOfEnemies));
        }

        if(!isWaveActive && finalBossRoom && GameManager.instance.enemiesRemain < (numOfEnemies - 4))
        {
            StartCoroutine(TotalEnemy(1));
        }

        // allows next wave to begin
        if (spawnStopped && GameManager.instance.enemiesRemain == 0)
        {
            if(continuousSpawning)
            {
                waveStart++;
                numOfEnemies++;
                isWaveActive = true;
            }

            if (GameManager.instance.waves != GameManager.instance.maxWaves)
                GameManager.instance.IncreaseWaveCount(waveStart + 1);

            spawnStopped = false; // stops waves from being reset to a previous value
        }
    }

    IEnumerator TotalEnemy(int amount)
    {

        // prevents spawner from starting again during the wave
        isWaveActive = false;

        // increments remaining enemies by the amount of enemies about to spawn
        if(!rats)
            GameManager.instance.UpdateWinCondition(amount);

        Vector3 spawnpoint = posToSpawn[0].position;

        // spawns the specified enemies at the specified rate
        for (int i = 0; i < amount; i++)
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

            yield return new WaitForSeconds(spawnRate);
            Instantiate(enemy[Random.Range(0, enemy.Length)], spawnpoint, Quaternion.identity);
        }

        spawnStopped = true;
    }
}
