using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool SharedInstance;

    public List<GameObject> pooledEnemies;
    public GameObject[] eneimesToPool;
    public int enemiesAmountToPool;

    public List<GameObject> pooledBlood;
    public GameObject bloodToPool;
    public int bloodAmountToPool;

    public List<GameObject> pooledSparkle;
    public GameObject sparkleToPool;
    public int sparkleAmountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledEnemies = new List<GameObject>();
        for (int i = 0; i < enemiesAmountToPool; i++)
        {
            int randomIndex = Random.Range(0, eneimesToPool.Length);
            GameObject tmpEnemy = Instantiate(eneimesToPool[randomIndex]);
            tmpEnemy.SetActive(false);
            pooledEnemies.Add(tmpEnemy);
        }

        pooledBlood = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < bloodAmountToPool; i++)
        {
            tmp = Instantiate(bloodToPool);
            tmp.SetActive(false);
            pooledBlood.Add(tmp);
        }

        pooledSparkle = new List<GameObject>();
        GameObject tmpSplash;
        for (int i = 0; i < sparkleAmountToPool; i++)
        {
            tmpSplash = Instantiate(sparkleToPool);
            tmpSplash.SetActive(false);
            pooledSparkle.Add(tmpSplash);
        }
    }

    public GameObject GetPooledEnemy()
    {
        for (int i = 0; i < pooledEnemies.Count; i++)
        {
            if (!pooledEnemies[i].activeInHierarchy)
            {
                return pooledEnemies[i];
            }
        }
        return null;
    }

    public GameObject GetPooledBlood()
    {
        for (int i = 0; i < bloodAmountToPool; i++)
        {
            if (!pooledBlood[i].activeInHierarchy)
            {
                return pooledBlood[i];
            }
        }
        return null;
    }

    public GameObject GetPooledSparkle()
    {
        for (int i = 0; i < sparkleAmountToPool; i++)
        {
            if (!pooledSparkle[i].activeInHierarchy)
            {
                return pooledSparkle[i];
            }
        }
        return null;
    }
}
