using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuanaMovement : MonoBehaviour
{
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;

    private JuanaBehavior juanaBehavior;
    private Rigidbody2D m_Rigidbody2D;
    public Animator animator;
    public bool m_FacingRight = false;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;
    public float runSpeed = 100f;
    [HideInInspector] public float horizontalMove = 0f;

    public float minX;
    public float maxX;

    private bool isAttacking = false;
    private Vector3 initialPosition;

    private bool openDoor = false;
    private int currentDoorIndex = -1;  
    private Transform[] currentDoorArray;
    private bool doorWalkCooldown = false;  
    private float cooldownDuration = 0.2f;
    [SerializeField] private Sprite closedDoor; 
    [SerializeField] private Sprite openedDoor;

    public Transform[] leftDoors;   
    public Transform[] rightDoors;

    private AudioSource stepAudioSource;
    public AudioClip footstepsClip;


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        juanaBehavior = GetComponent<JuanaBehavior>();
        animator = GetComponent<Animator>();
        stepAudioSource = gameObject.AddComponent<AudioSource>();
        stepAudioSource.clip = footstepsClip;  
        stepAudioSource.playOnAwake = false;
        stepAudioSource.loop = true;
        stepAudioSource.volume = 0.23f;

    }

    public void Move(float move)
    {
        float currentX = transform.position.x;

        if ((move < 0 && currentX <= minX) || (move > 0 && currentX >= maxX))
        {
            move = 0f; 
        }

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !m_FacingRight)
        {

            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && m_FacingRight)
        {

            // ... flip the player.
            Flip();
        }

    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    void FixedUpdate()
    {
        // Move our character
        if (!isAttacking) Move(horizontalMove * Time.fixedDeltaTime);

    }

    // Update is called once per frame
    void Update()
    {

        
            float speed = Mathf.Abs(horizontalMove);
            animator.SetFloat("Speed", speed);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("JuanaRun"))
            {
                animator.SetBool("ContinueRun", false); // Reset after transition
            }
        


        if (!isAttacking)
        {
            if (!juanaBehavior.isBlocking && !juanaBehavior.isDead) 
            {
                if (Input.GetKey(KeyCode.A)) horizontalMove = -runSpeed;
                else if (Input.GetKey(KeyCode.D)) horizontalMove = runSpeed;
                else horizontalMove = 0f;
            }
            else
            {
                horizontalMove = 0f;
            }

            HandleAnimation();
        }
        else
        {
            horizontalMove = 0f; 
        }

        HandleAttackMovement();


        if (openDoor && currentDoorIndex != -1 && !doorWalkCooldown)
        {
            

            if (Input.GetKey(KeyCode.W))
            {
                int nextIndex = (currentDoorIndex + 1) % currentDoorArray.Length;
                WalkToDoor(nextIndex);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                int prevIndex = (currentDoorIndex - 1 + currentDoorArray.Length) % currentDoorArray.Length;
                WalkToDoor(prevIndex);
            }
        }
    }

    private void HandleAnimation()
    {
        float speed = Mathf.Abs(horizontalMove);

        animator.SetFloat("Speed", speed);

        if (speed > 0.01f && animator.GetCurrentAnimatorStateInfo(0).IsName("JuanaIdle"))
        {
            animator.Play("JuanaIdleToRun");
            if (!stepAudioSource.isPlaying)
                stepAudioSource.Play();
        }
        else if (speed < 0.01f && animator.GetCurrentAnimatorStateInfo(0).IsName("JuanaRun"))
        {
            animator.Play("JuanaRunToIdle");
            if (stepAudioSource.isPlaying)
                stepAudioSource.Stop();
        }
    }

    private void HandleAttackMovement()
    {
        if (Input.GetMouseButtonDown(0)) // Attack Start
        {
            isAttacking = true;
            juanaBehavior.isHittingEnemy = true;
            initialPosition = transform.position;
            juanaBehavior.animator.SetTrigger("Attack");
            
        }
        else if (Input.GetMouseButtonUp(0)) // Attack End
        {
            isAttacking = false;
            juanaBehavior.isHittingEnemy = false;

        }

        if (!isAttacking) // Allow movement when not attacking
        {
            if (Input.GetKey(KeyCode.A)) horizontalMove = -runSpeed;
            else if (Input.GetKey(KeyCode.D)) horizontalMove = runSpeed;
            else horizontalMove = 0f;
        }
        else
        {
            horizontalMove = 0f; // Stop moving while attacking
        }
    }

  

    private void WalkToDoor(int index)
    {
        if (currentDoorIndex >= 0 && currentDoorArray != null)
        {
            currentDoorArray[currentDoorIndex].GetChild(0).gameObject.SetActive(false);
            currentDoorArray[currentDoorIndex].GetComponent<AudioSource>().Play();
            StartCoroutine(ChangeDoorSprite(currentDoorArray[currentDoorIndex].GetComponent<SpriteRenderer>()));

        }

        transform.position = currentDoorArray[index].position;
        currentDoorIndex = index;
        currentDoorArray[index].GetChild(0).gameObject.SetActive(false);
        StartCoroutine(ChangeDoorSprite(currentDoorArray[index].GetComponent<SpriteRenderer>()));

        StartCoroutine(DoorWalkCooldown());
    }

    private IEnumerator DoorWalkCooldown()
    {
        doorWalkCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        doorWalkCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!doorWalkCooldown && collision.gameObject.CompareTag("Door"))
        {
            openDoor = true;
            SetCurrentDoor(collision.transform);
            collision.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!doorWalkCooldown && collision.gameObject.CompareTag("Door"))
        {
            openDoor = false;
            currentDoorIndex = -1;
            currentDoorArray = null;
            collision.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void SetCurrentDoor(Transform door)
    {
        if (door.position.x < 0)
            currentDoorArray = leftDoors;
        else
            currentDoorArray = rightDoors;

        for (int i = 0; i < currentDoorArray.Length; i++)
        {
            if (currentDoorArray[i] == door)
            {
                currentDoorIndex = i;
                break;
            }
        }
    }

    private IEnumerator ChangeDoorSprite(SpriteRenderer doorSpriteRenderer)
    {
        if (doorSpriteRenderer != null)
        {
            doorSpriteRenderer.sprite = openedDoor;
            doorSpriteRenderer.transform.localScale = new Vector3(0.63f, 0.68f, 1f);
            yield return new WaitForSeconds(1.0f); 
            doorSpriteRenderer.sprite = closedDoor;
            doorSpriteRenderer.transform.localScale = new Vector3(0.63f, 0.68f, 1f);
        }
    }
}
