using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Assertions;

public class JuanaBehavior : MonoBehaviour
{
    public GameObject[] shields;
    public int life;
    public Animator animator;
    private bool isAttackFinished = true;

    private bool canTakeDamageFromEnemy = true;
    private float enemyDamageCooldown = 2f;
    private bool isTakingDamage = false;

    [HideInInspector]
    public bool isHittingEnemy = false;
    [HideInInspector]
    public bool isHittingCage = false;
    [HideInInspector]
    public bool isBlocking = false;
    [HideInInspector]
    public bool isDead = false;

    private AudioSource extraAudioSource;
    [SerializeField]
    private AudioClip swoosh;
    [SerializeField]
    private AudioClip fall;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        extraAudioSource = gameObject.AddComponent<AudioSource>();
        extraAudioSource.volume = 0.2f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ControlLifeUI();
        if (!isDead)
        {


            if (Input.GetMouseButtonDown(0) && isAttackFinished)
            {
                //attack enemies
                isHittingEnemy = true;
                isBlocking = false;
                isHittingCage = false;
                isAttackFinished = false;
                animator.SetTrigger("Attack");
                extraAudioSource.PlayOneShot(swoosh);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isHittingEnemy = false;
            }
            //if (animator.GetCurrentAnimatorStateInfo(0).IsName("JuanaAttack"))
            //{
            //    isHittingEnemy = true; 
            //}
            //else
            //{
            //    isHittingEnemy = false; 
            //}
            if (!isAttackFinished && animator.GetCurrentAnimatorStateInfo(0).IsName("JuanaAttack"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
                {
                    isAttackFinished = true;

                    if (IsMoving())
                        animator.SetBool("ContinueRun", true);
                    else
                        animator.SetTrigger("Idle");
                }
            }




            if (Input.GetMouseButtonDown(1))
            {
                //set block
                isBlocking = true;
                isHittingEnemy = false;
                isHittingCage = false;
                animator.SetTrigger("Block");
            }
            else if (Input.GetMouseButtonUp(1))
            {
                isBlocking = false;
                animator.SetTrigger("Idle");
            }


            if (Input.GetKeyDown(KeyCode.Space) && isAttackFinished && !IsMoving())
            {
                //attack cage
                isHittingCage = true;
                isHittingEnemy = false;
                isBlocking = false;
                isAttackFinished = false;
                animator.SetTrigger("Attack");
                extraAudioSource.PlayOneShot(swoosh);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                isHittingCage = false;
            }
        }
    }

    private bool IsMoving()
    {
        JuanaMovement movement = GetComponent<JuanaMovement>();
        return Mathf.Abs(movement.horizontalMove) > 0.01f;
    }
    public void FinishAttack()
    {
        isAttackFinished = true;
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Idle");
    }


    public void ControlLifeUI()
    {
        if (life < 1)
        {
            isDead = true;
            Destroy(shields[0].gameObject);
            animator.Play("JuanaDie");
            PlayerPrefs.SetFloat("loseCondition", 1);
            PlayerPrefs.Save();
            extraAudioSource.PlayOneShot(fall);
            Invoke("LoseGame", 1f);

        }
        else if (life < 2)
        {
            Destroy(shields[1].gameObject);
        }
        else if (life < 3)
        {
            Destroy(shields[2].gameObject);
        }
    }

    private void LoseGame()
    {
        GameManager.sharedInstance.TriggerGameOver();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleEnemyCollision(collision);
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.gameObject.GetComponent<EnemyBehavior>();

            if (enemy.isFighting && !isBlocking && !isHittingEnemy && canTakeDamageFromEnemy && !isTakingDamage)
            {
                ShowBlood();
                life--;
                StartCoroutine(PlayDamagedAnimation());
                StartCoroutine(EnemyDamageCooldown());
            } else if (enemy.isFighting && (isBlocking || isHittingEnemy))
            {
                ShowSparkles();
            }
        }
    }

    private IEnumerator PlayDamagedAnimation()
    {
        isTakingDamage = true;

        animator.SetTrigger("Damaged"); // Trigger the damage animation

        // Wait until the animation reaches the end
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("JuanaDamaged") &&
                                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        // Check if the player is still moving
        if (IsMoving())
            animator.SetBool("ContinueRun", true);  // Transition to run
        else
            animator.SetTrigger("Idle");  // Otherwise, transition to idle

        isTakingDamage = false;
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

    private IEnumerator EnemyDamageCooldown()
    {
        canTakeDamageFromEnemy = false;
        yield return new WaitForSeconds(enemyDamageCooldown);
        canTakeDamageFromEnemy = true;
    }
}
