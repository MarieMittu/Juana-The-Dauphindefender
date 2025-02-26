using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseResult : MonoBehaviour
{
    public GameObject juanaDead, dauphinKilled, dauphinDead;

    // Start is called before the first frame update
    void Start()
    {
        float loseCondition = PlayerPrefs.GetFloat("loseCondition", 0);

        if (loseCondition == 1f)
        {
            juanaDead.SetActive(true);
            dauphinKilled.SetActive(false);
            dauphinDead.SetActive(false);

        }
        else if (loseCondition == 2f)
        {
            juanaDead.SetActive(false);
            dauphinKilled.SetActive(true);
            dauphinDead.SetActive(false);
        }
        else if (loseCondition == 3f)
        {
            juanaDead.SetActive(false);
            dauphinKilled.SetActive(false);
            dauphinDead.SetActive(true);
        }
    }
}
