using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject enemy;
    [SerializeField] int numOfEnemies;
    [SerializeField] float spawnRate;
    [SerializeField] Transform posToSpawn;

    int origNumOfEnemies;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.noEnemies == true && GameManager.instance.waves != 5)
        {
            origNumOfEnemies = numOfEnemies;
            StartCoroutine(TotalEnemy());
            GameManager.instance.waves++;
            GameManager.instance.noEnemies = false;
        }

    }

    IEnumerator TotalEnemy()
    {
        while (numOfEnemies > 0 )
        {

            Instantiate(enemy, posToSpawn.position, Quaternion.identity);
            numOfEnemies--;
            yield return new WaitForSeconds(spawnRate);

        }
        numOfEnemies = origNumOfEnemies;
    }
}
