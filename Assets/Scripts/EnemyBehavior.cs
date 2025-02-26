using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public static EnemyBehavior instance;

    public float health;
    public float speed;
    private Rigidbody2D myBody;
    private bool canTakeDamage = true;
    private float damageCooldown = 0.2f;
    [HideInInspector]
    public bool isFighting = false;
    [HideInInspector]
    public bool isDestroyingCage = false;
    private bool isSpawnedFromLeft;
    private Transform playerTransform;
    public Animator animator;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (health > 0)
        {
            myBody.velocity = new Vector2(speed, 0);

        }

    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RealCage") || collision.gameObject.CompareTag("FakeCage") && !isFighting)
        {
            speed = 0;
            StartCoroutine(DestroyCycle());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RealCage") || collision.gameObject.CompareTag("FakeCage"))
        {
            speed = (isSpawnedFromLeft) ? 2 : -2;
            isDestroyingCage = false;
            StopCoroutine(DestroyCycle());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandlePlayerCollision(collision);
    }

    private void HandlePlayerCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FlipToFacePlayer();

            if (!isFighting)
            {
                StartCoroutine(FightCycle());
            }

            JuanaBehavior player = collision.gameObject.GetComponent<JuanaBehavior>();

            if (player != null && player.isHittingEnemy && canTakeDamage)
            {
                Debug.Log("Enemy taking damage!");
                TakeDamage();
            }

        }
    }

    private void FlipToFacePlayer()
    {
        if (playerTransform != null)
        {
            float playerDirection = playerTransform.position.x - transform.position.x;

            if (isSpawnedFromLeft && playerDirection < 0)
            {
                // Flip if player is on the left and enemy spawned from the left
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (!isSpawnedFromLeft && playerDirection > 0)
            {
                // Flip if player is on the right and enemy spawned from the right
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void TakeDamage()
    {
        Debug.Log("is dmaaaaged");
        animator.SetTrigger("Hit");
        ShowBlood();
        health--;

        if (health <= 0)
        {
            animator.SetTrigger("Die");
            Invoke("RemoveBody", 2f);
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
        if (speed != 0)
        {
            animator.SetTrigger("Walk");
        } else
        {
            animator.SetTrigger("Fight");
        }
        
    }

    private IEnumerator FightCycle()
    {
        Debug.Log("HITHITHIT");
        animator.SetTrigger("Fight");
        isFighting = true;
        isDestroyingCage = false;
        yield return new WaitForSeconds(0.1f); 

        canTakeDamage = true;  
        Debug.Log("Enemy can now take damage during fight!");

        yield return new WaitForSeconds(3.5f); 

        isFighting = false;
        Debug.Log("idle");
    }

    private IEnumerator DestroyCycle()
    {
        Debug.Log("Starting cage destruction cycle...");
        isDestroyingCage = true;
        animator.SetTrigger("Fight");
        while (isDestroyingCage)
        {
            Debug.Log("HIT CAGE!");
            // ADD: Call cage damage method here if needed
            yield return new WaitForSeconds(2.5f); // Interval between hits
        }

        Debug.Log("Stopped cage destruction cycle.");
    }



    private void ShowBlood()
    {
        GameObject newBlood = ObjectPool.SharedInstance.GetPooledBlood();

        if (newBlood != null)
        {
            newBlood.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            newBlood.transform.rotation = Quaternion.identity;
            newBlood.SetActive(true);
        }
    }

    private void RemoveBody()
    {
        gameObject.SetActive(false);
    }

    public void SetSpawnSide(bool fromLeft)
    {
        isSpawnedFromLeft = fromLeft;
    }
}
