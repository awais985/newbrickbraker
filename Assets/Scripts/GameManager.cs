using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // =========================================================
    // GAME SETTINGS
    // =========================================================

    [Header("Lives")]
    [SerializeField] private int lives = 3;
    [SerializeField] private int maxLives = 3;

    [Header("References")]
    [SerializeField] private BallMovement ballMovement;
    [SerializeField] private PaddleController paddleController;
    [SerializeField] private LivesUI livesUI;


    // =========================================================
    // OPTIONAL EXTRA LIFE TEST SPAWNER
    // =========================================================

    [Header("Extra Life Test")]
    [SerializeField] private GameObject extraLife;

    private Coroutine extraLifeGenerate;


    // =========================================================
    // SINGLETON
    // =========================================================

    // Puri game mein GameManager ka global reference
    public static GameManager instance;


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // MultiBall system ke liye
    private int activeBalls = 1;

    // Pause state
    private bool isPaused;

    // Ek hi ball miss par multiple life loss rokne ke liye
    private bool isLosingLife;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Duplicate GameManager ko destroy karna
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Current GameManager ko main instance banana
        instance = this;
    }


    private void Start()
    {
        // Starting lives UI update
        if (livesUI != null)
        {
            livesUI.UpdateLives(lives);
        }

        /*
        // Sirf testing ke liye:
        // Har kuch seconds baad ExtraLife spawn karwana ho
        if (extraLifeGenerate == null)
        {
            extraLifeGenerate =
                StartCoroutine(ExtraLifeCreate());
        }
        */
    }


    // =========================================================
    // UPDATE / INPUT
    // =========================================================

    private void Update()
    {
        // Escape key se Pause / Resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    // =========================================================
    // MULTI BALL SYSTEM
    // =========================================================

    // Extra Ball spawn hone par count increase karna
    public void RegisterBall()
    {
        activeBalls++;
    }


    // Ball DeadZone mein jane par call hoga
    public void UnregisterBall(BallMovement lostBall)
    {
        activeBalls--;

        // -----------------------------------------------------
        // Agar aur balls abhi game mein hain
        // -----------------------------------------------------

        if (activeBalls > 0)
        {
            if (lostBall.IsExtraBall())
            {
                // Extra ball simply destroy
                Destroy(lostBall.gameObject);
            }
            else
            {
                // Original ball ko destroy nahi karna
                // sirf temporary hide karna
                lostBall.gameObject.SetActive(false);
            }

            return;
        }


        // -----------------------------------------------------
        // Last active ball bhi gir gayi
        // -----------------------------------------------------

        if (lostBall.IsExtraBall())
        {
            Destroy(lostBall.gameObject);
        }

        // Next round mein sirf original ball hogi
        activeBalls = 1;


        // Original Ball ko restore karna
        if (ballMovement != null)
        {
            ballMovement.gameObject.SetActive(true);

            LoseLife(ballMovement);
        }
    }


    // MultiBall sirf tab allowed hai
    // jab ek hi ball active ho
    public bool CanSpawnMultiBall()
    {
        return activeBalls == 1;
    }


    // =========================================================
    // OPTIONAL EXTRA LIFE TEST SPAWNER
    // =========================================================

    private IEnumerator ExtraLifeCreate()
    {
        while (true)
        {
            // 5 seconds wait
            yield return new WaitForSecondsRealtime(5f);

            if (extraLife == null)
            {
                continue;
            }

            // ExtraLife spawn
            GameObject extraLifeObject =
                Instantiate(
                    extraLife,
                    transform.position,
                    Quaternion.identity
                );

            // Testing size
            extraLifeObject.transform.localScale =
                Vector3.one * 0.2f;


            // 3 seconds visible
            yield return new WaitForSecondsRealtime(3f);


            // Agar abhi collect nahi hui
            if (extraLifeObject != null)
            {
                Destroy(extraLifeObject);
            }
        }
    }


    // =========================================================
    // LEVEL RESTART
    // =========================================================

    public void RestartLevel()
    {
        PlayButtonSound();

        // Pause state remove
        Time.timeScale = 1f;
        isPaused = false;


        if (LevelManager.instance != null)
        {
            LevelManager.instance.RestartCurrentLevel();
        }


        ResetLevelState();
    }


    // =========================================================
    // RESTART WHOLE GAME
    // =========================================================

    public void RestartGame()
    {
        PlayButtonSound();

        Time.timeScale = 1f;
        isPaused = false;


        if (LevelManager.instance != null)
        {
            LevelManager.instance.LoadLevel(1);
        }
    }


    // =========================================================
    // NEXT LEVEL
    // =========================================================

    public void LoadNextLevel()
    {
        PlayButtonSound();

        Time.timeScale = 1f;
        isPaused = false;


        if (LevelManager.instance == null)
        {
            return;
        }


        bool hasNextLevel =
            LevelManager.instance.LevelCompleted();


        // -----------------------------------------------------
        // Next level available
        // -----------------------------------------------------

        if (hasNextLevel)
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.HideLevelComplete();
            }

            ResetLevelState();
        }

        // -----------------------------------------------------
        // Saare levels complete
        // -----------------------------------------------------

        else
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.HideLevelComplete();
                UIManager.instance.ShowGameComplete();
            }
        }
    }


    // =========================================================
    // START GAME / LEVEL SELECT
    // =========================================================

    public void StartGame()
    {
        PlayButtonSound();

        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene("LevelSelect");
    }


    // =========================================================
    // RESET BALL + PADDLE
    // =========================================================

    private void ResetLevelState()
    {
        if (ballMovement != null)
        {
            ballMovement.ResetBall();
        }


        // IMPORTANT:
        // Pehle yahan == null tha, jo wrong tha
        if (paddleController != null)
        {
            paddleController.ResetPaddle();
        }
    }


    // =========================================================
    // MAIN MENU
    // =========================================================

    public void LoadMainMenu()
    {
        PlayButtonSound();

        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(0);
    }


    // =========================================================
    // ADD LIFE
    // =========================================================

    public void AddLife()
    {
        // Maximum lives se zyada nahi jane dena
        if (lives >= maxLives)
        {
            lives = maxLives;
            return;
        }


        // Ek life add
        lives++;


        // Yahan LoseLife sound nahi chalana
        // Future mein PlayExtraLife() sound bana sakte ho

        /*
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayExtraLife();
        }
        */


        // Hearts UI update
        if (livesUI != null)
        {
            livesUI.UpdateLives(lives);
        }
    }


    // =========================================================
    // LOSE LIFE
    // =========================================================

    public void LoseLife(BallMovement lostBall)
    {
        // Same miss ko multiple baar process hone se rokna
        if (isLosingLife)
        {
            return;
        }

        isLosingLife = true;


        // Ek life minus
        lives--;


        // Lose-life sound sirf ek baar
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayLoseLife();
        }


        //Debug.Log("Lives Remaining: " + lives);


        // Hearts UI update
        if (livesUI != null)
        {
            livesUI.UpdateLives(lives);
        }


        // -----------------------------------------------------
        // GAME OVER
        // -----------------------------------------------------

        if (lives <= 0)
        {
            lives = 0;


            if (AudioClipManager.instance != null)
            {
                AudioClipManager.instance.PlayGameOver();
            }


            if (UIManager.instance != null)
            {
                UIManager.instance.ShowGameOver();
            }

            return;
        }


        // -----------------------------------------------------
        // LIFE STILL REMAINS
        // -----------------------------------------------------

        if (lostBall != null)
        {
            lostBall.ResetBall();
        }


        // Thori protection delay
        StartCoroutine(
            AllowLifeLossAfterDelay()
        );
    }


    // =========================================================
    // LIFE LOSS COOLDOWN
    // =========================================================

    private IEnumerator AllowLifeLossAfterDelay()
    {
        // TimeScale ko ignore karega
        yield return new WaitForSecondsRealtime(0.5f);

        isLosingLife = false;
    }


    // =========================================================
    // PAUSE
    // =========================================================

    public void PauseGame()
    {
        PlayButtonSound();

        isPaused = true;

        // Game freeze
        Time.timeScale = 0f;


        if (UIManager.instance != null)
        {
            UIManager.instance.ShowPausePanel();
        }
    }


    // =========================================================
    // RESUME
    // =========================================================

    public void ResumeGame()
    {
        PlayButtonSound();

        isPaused = false;

        // Normal game speed
        Time.timeScale = 1f;


        if (UIManager.instance != null)
        {
            UIManager.instance.HidePausePanel();
        }
    }


    // =========================================================
    // BUTTON SOUND
    // =========================================================

    private void PlayButtonSound()
    {
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayButtonClick();
        }
    }


    // =========================================================
    // QUIT GAME
    // =========================================================

    public void QuitGame()
    {
        PlayButtonSound();

        Application.Quit();
    }
}