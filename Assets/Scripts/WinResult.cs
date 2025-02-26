using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinResult : MonoBehaviour
{

    public GameObject young, adult, old;
    public Text title;

    // Start is called before the first frame update
    void Start()
    {
        float timeRatio = PlayerPrefs.GetFloat("timeRatio", 0);

        if (timeRatio > 0.66f)
        {
            title.text = "Long lives the Dauphin!";
            young.SetActive(true);
            adult.SetActive(false);
            old.SetActive(false);

        }
        else if (timeRatio > 0.33f)
        {
            title.text = "Pretty long lives the Dauphin!";
            young.SetActive(false);
            adult.SetActive(true);
            old.SetActive(false);
        }
        else
        {
            title.text = "Not so long lives the Dauphin...";
            young.SetActive(false);
            adult.SetActive(false);
            old.SetActive(true);
        }
    }

}
