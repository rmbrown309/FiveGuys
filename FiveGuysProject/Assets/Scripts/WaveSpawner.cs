using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("-----Components------")]
    [SerializeField] Transform posToSpawn;
    [SerializeField] GameObject enemy; // enemy type spawned by this spawner
    [Header("-----Spawner Stats------")]
    [Range(1, 5)][SerializeField] int waveNum; // On what wave this spawner is active
    [SerializeField] int numOfEnemies; // number of enemies spawned during the wave
    [SerializeField] float spawnRate; // seconds between each emnemy spawn
    [SerializeField] bool rats;
  
    bool spawnStopped = false;
    bool isWaveActive = true;

 
    // Update is called once per frame
    void Update()
    {
        // Spawns enemies only on the correct wave number
        if (isWaveActive && GameManager.instance.waves == waveNum)
        {
            StartCoroutine(TotalEnemy());
        }

        // allows next wave to begin
        if (spawnStopped && GameManager.instance.enemiesRemain == 0)
        {
            GameManager.instance.IncreaseWaveCount(waveNum + 1);
            spawnStopped = false; // stops waves from being reset to a previous value
        }
    }

    IEnumerator TotalEnemy()
    {
        // prevents spawner from starting again during the wave
        isWaveActive = false;

        // increments remaining enemies by the amount of enemies about to spawn
        if(!rats)
            GameManager.instance.UpdateWinCondition(numOfEnemies);

        // spawns the specified enemies at the specified rate
        for (int i = 0; i < numOfEnemies; i++)
        {
            Instantiate(enemy, posToSpawn.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnRate);
        }

        spawnStopped = true;
    }
}
