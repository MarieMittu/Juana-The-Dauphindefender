using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageBehavior : MonoBehaviour
{

    public int health;
    private bool canTakeDamage = true;
    private float damageCooldown = 2.5f;
    private bool isEnemyDamaging = false;
    private Coroutine enemyDamageCoroutine;

    [Header("Sprites for Different Damage Levels")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] cageSprites;

    // Start is called before the first frame update
    void Start()
    {
        UpdateSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandlePlayerCollision(collision);
        HandleEnemyCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandlePlayerCollision(collision);
        HandleEnemyCollision(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && enemyDamageCoroutine != null)
        {
            StopCoroutine(enemyDamageCoroutine);  
            enemyDamageCoroutine = null;          
            isEnemyDamaging = false;
            Debug.Log("STOP DAMAGE");
        }
    }



    private void HandlePlayerCollision(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            JuanaBehavior player = collision.gameObject.GetComponent<JuanaBehavior>();

            if (player != null && player.isHittingCage && canTakeDamage)
            {
                TakeDamage();
            }
        }
    }

    private void HandleEnemyCollision(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.gameObject.GetComponent<EnemyBehavior>();

            if (enemy != null && enemy.isDestroyingCage && !isEnemyDamaging)
            {
                enemyDamageCoroutine = StartCoroutine(EnemyDamageCycle());
            }
        }
    }

    private IEnumerator EnemyDamageCycle()
    {
        isEnemyDamaging = true;

        while (true)  // Loop continues until stopped
        {
            if (canTakeDamage)
            {
                TakeDamage();
            }

            yield return new WaitForSeconds(damageCooldown); // Wait for cooldown before next hit
        }
    }

    private void TakeDamage()
    {
        if (health > 0) ShowSparkles();
        health--;
        UpdateSprite();

        if (health <= 0)
        {
            GameManager.sharedInstance.OnCageDestroyed(this);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    private void ShowSparkles()
    {
        GameObject newSparkle = ObjectPool.SharedInstance.GetPooledSparkle();

        if (newSparkle != null)
        {
            newSparkle.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            newSparkle.transform.rotation = transform.rotation;
            newSparkle.SetActive(true);
        }
    }

    private void UpdateSprite()
    {
        if (spriteRenderer != null && cageSprites.Length == 4)
        {
            spriteRenderer.transform.localScale = new Vector3(0.69f, 0.69f, 1f);
            if (health >= 5)
                spriteRenderer.sprite = cageSprites[0]; 
            else if (health >= 3)
                spriteRenderer.sprite = cageSprites[1]; 
            else if (health >= 1)
                spriteRenderer.sprite = cageSprites[2]; 
            else
                spriteRenderer.sprite = cageSprites[3]; 
        }
    }

}
