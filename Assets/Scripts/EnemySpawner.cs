using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public Transform[] leftPositions;  
    public Transform[] rightPositions;

    public int randomSide;
    public int enemySpeed;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }



    IEnumerator SpawnEnemies()
    {

        while (true)
        {
            float waitToSpawn = GetSpawnInterval();
            yield return new WaitForSeconds(waitToSpawn);

            randomSide = Random.Range(0, 2);

            GameObject selectedEnemy = ObjectPool.SharedInstance.GetPooledEnemy();
            EnemyBehavior enemyBehavior = selectedEnemy.GetComponent<EnemyBehavior>();

            //left side
            if (randomSide == 0)
            {
                int randomIndex = Random.Range(0, leftPositions.Length);
                selectedEnemy.transform.position = leftPositions[randomIndex].position;
                selectedEnemy.transform.localScale = new Vector3(-selectedEnemy.transform.localScale.x, selectedEnemy.transform.localScale.y, selectedEnemy.transform.localScale.z);
                enemyBehavior.speed = enemySpeed;
                enemyBehavior.SetSpawnSide(true);

            }
            else
            {
                //right side
                int randomIndex = Random.Range(0, rightPositions.Length);
                selectedEnemy.transform.position = rightPositions[randomIndex].position;
                enemyBehavior.speed = -enemySpeed;
                enemyBehavior.SetSpawnSide(false);

            }

            selectedEnemy.SetActive(true);
        }
    }

    private float GetSpawnInterval()
    {
        float ratio = GameManager.sharedInstance.missionDuration / GameManager.sharedInstance.startMissionDuration;

        if (ratio > 0.66f)
            return Random.Range(5f, 8f);
        else if (ratio > 0.33f)
            return Random.Range(4f, 7f);
        else
            return Random.Range(2f, 4f);
    }
}
