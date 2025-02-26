using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] speechSprites;

    [Header("Random Interval Settings")]
    public float minInterval = 5f; 
    public float maxInterval = 10f; 

    [Header("Random Display Duration")]
    public float minDuration = 4f; 
    public float maxDuration = 7f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sprite = null; 
        StartCoroutine(SpeechCycle());
    }

    private IEnumerator SpeechCycle()
    {
        while (true) 
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime); 

            spriteRenderer.sprite = GetRandomSprite();

            float displayTime = Random.Range(minDuration, maxDuration);
            yield return new WaitForSeconds(displayTime); 

            spriteRenderer.sprite = null; 
        }
    }

    private Sprite GetRandomSprite()
    {
        if (speechSprites.Length == 0) return null; 
        int randomIndex = Random.Range(0, speechSprites.Length);
        return speechSprites[randomIndex];
    }
}
