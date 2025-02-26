using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager sharedInstance;

    private CageBehavior realCage;
    private List<CageBehavior> fakeCages;
    public CageBehavior[] fakeCagesArray;
    public GameObject[] princes;

    public float missionDuration;
    public float startMissionDuration;
    float secondTimer = 0f;
    public Image timer;
    public bool isGamePaused;
    public float timeRatio;
    public float loseCondition;

    private void Awake()
    {

        sharedInstance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        startMissionDuration = missionDuration;
        if (timer != null)
        {
            timer.fillAmount = 1f;
        }

        timeRatio = PlayerPrefs.GetFloat("timeRatio", missionDuration / startMissionDuration);
        loseCondition = PlayerPrefs.GetFloat("loseCondition", 0);

        InitializeCages();
        UpdatePrinceAnimations();
    }

    private void InitializeCages()
    {
        // Find real cage
        realCage = GameObject.FindGameObjectWithTag("RealCage")?.GetComponent<CageBehavior>();

        // Find all fake cages
        //fakeCages = GameObject.FindGameObjectsWithTag("FakeCage")
        //    .Select(c => c.GetComponent<CageBehavior>())
        //    .ToList();
        fakeCages = fakeCagesArray.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        isGamePaused = FindObjectOfType<ScenesController>().isPaused;

        secondTimer += Time.deltaTime;
        if (secondTimer >= 1f)
        {
            missionDuration--;
            secondTimer -= 1f;

            if (timer != null)
            {
                timer.fillAmount = missionDuration / startMissionDuration;
            }
            PlayerPrefs.SetFloat("timeRatio", missionDuration / startMissionDuration);
        }

        UpdatePrinceAnimations();

        if (missionDuration <= 0)
        {
            PlayerPrefs.SetFloat("loseCondition", 3);
            PlayerPrefs.Save();
            TriggerGameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindObjectOfType<ScenesController>().isPaused)
            {
                if (FindObjectOfType<ScenesController>().isTutorial)
                {
                    FindObjectOfType<ScenesController>().CloseTutorial();
                } else
                {
                    FindObjectOfType<ScenesController>().Resume();
                    Cursor.lockState = CursorLockMode.Locked;
                } 
            }
            else
            {
                FindObjectOfType<ScenesController>().Pause();
                
            }
        }
    }

    private void UpdatePrinceAnimations()
    {
        string animationTrigger;

        if (timeRatio > 0.66f)
        {
            animationTrigger = "Teen";
        }
        else if (timeRatio > 0.33f)
        {
            animationTrigger = "Adult";
        }
        else if (timeRatio > 0)
        {
            animationTrigger = "Old";
        }
        else
        {
            return; // No animation if ratio <= 0
        }

        foreach (var prince in princes)
        {
            Animator princeAnimator = prince.GetComponent<Animator>();
            if (princeAnimator != null)
            {
                princeAnimator.ResetTrigger("Teen");
                princeAnimator.ResetTrigger("Adult");
                princeAnimator.ResetTrigger("Old");

                princeAnimator.SetTrigger(animationTrigger);
            }
        }
    }

    public void OnCageDestroyed(CageBehavior destroyedCage)
    {
        if (destroyedCage.CompareTag("RealCage"))
        {
            TriggerPrinceRealDeathAnim(princes[1]);
            PlayerPrefs.SetFloat("loseCondition", 2);
            PlayerPrefs.Save();
            Invoke("TriggerGameOver", 2f);
        }
        else
        {
            if (destroyedCage == fakeCagesArray[0]) // First fake cage destroyed → First prince
            {
                TriggerPrinceFakeDeathAnim(princes[0]);
            }
            else if (destroyedCage == fakeCagesArray[1]) // Second fake cage destroyed → Last prince
            {
                TriggerPrinceFakeDeathAnim(princes[2]);
            }
            // If all fake cages are destroyed and real cage is still alive → FinishGame
            if (fakeCages.All(c => c.health <= 0) && realCage != null && realCage.health > 0)
            {
                FinishGame();
            }
        }
    }

    private void TriggerPrinceFakeDeathAnim(GameObject prince)
    {
        Animator princeAnimator = prince.GetComponent<Animator>();
        if (princeAnimator != null)
        {
            princeAnimator.SetTrigger("FakeDeath");
        }
    }

    private void TriggerPrinceRealDeathAnim(GameObject prince)
    {
        Animator princeAnimator = prince.GetComponent<Animator>();
        if (princeAnimator != null)
        {
            princeAnimator.SetTrigger("RealDeth");
        }
    }


    void FinishGame()
    {
        PlayerPrefs.SetFloat("timeRatio", missionDuration / startMissionDuration);
        PlayerPrefs.Save();
        FindObjectOfType<ScenesController>().GameWon();
    }

    public void TriggerGameOver()
    {
        FindObjectOfType<ScenesController>().GameOver();
    }
}
