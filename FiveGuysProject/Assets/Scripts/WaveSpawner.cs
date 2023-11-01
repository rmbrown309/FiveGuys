using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] bool rats;
    [SerializeField] bool continuousSpawning;
  
    bool spawnStopped = false;
    bool isWaveActive = true;

 
    // Update is called once per frame
    void Update()
    {
        // Spawns enemies only on the correct wave number
        if (isWaveActive && GameManager.instance.waves == waveStart)
        {
            StartCoroutine(TotalEnemy());
        }

        // allows next wave to begin
        if (spawnStopped && GameManager.instance.enemiesRemain == 0)
        {
            if(GameManager.instance.waves != GameManager.instance.maxWaves)
                GameManager.instance.IncreaseWaveCount(waveStart + 1);

            if(continuousSpawning)
            {
                waveStart++;
                isWaveActive = true;
            }

            spawnStopped = false; // stops waves from being reset to a previous value
        }
    }

    IEnumerator TotalEnemy()
    {
        Transform spawnpoint = posToSpawn[0];

        // prevents spawner from starting again during the wave
        isWaveActive = false;

        // increments remaining enemies by the amount of enemies about to spawn
        if(!rats)
            GameManager.instance.UpdateWinCondition(numOfEnemies);

        // spawns the specified enemies at the specified rate
        for (int i = 0; i < numOfEnemies; i++)
        {
            if (posToSpawn.Length > 1)
            {
                Transform furthestSpawn = posToSpawn[0];
                for(int j = 1; j < posToSpawn.Length; j++)
                {
                    // prioritize the spawnpoint furthest from the player in order to keep enemies from spawning on top of them
                    if ((posToSpawn[j].position - GameManager.instance.player.transform.position).magnitude > (furthestSpawn.position - GameManager.instance.player.transform.position).magnitude)
                        furthestSpawn = posToSpawn[j];
                }

                spawnpoint = furthestSpawn;
            }

            yield return new WaitForSeconds(spawnRate);
            Instantiate(enemy[Random.Range(0, enemy.Length)], spawnpoint.position, Quaternion.identity);
        }

        spawnStopped = true;
    }
}
