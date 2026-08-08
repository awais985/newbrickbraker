using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int lives = 3;

    // Puri game mein GameManager ka single global reference
    public static GameManager instance;

    private bool isPaused;

    private void Awake()
    {
        // Agar pehle se koi GameManager instance mojood hai
        // aur woh current object nahi hai
        if (instance != null && instance != this)
        {
            // Duplicate GameManager destroy karna
            Destroy(gameObject);

            // Neeche ka code run nahi karna
            return;
        }

        // Current GameManager ko main instance banana
        instance = this;
    }

    private void Update()
    {
        // Esc key sirf press hone ke moment par check karna
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Agar game pehle se paused hai
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

    // Current level ko dobara load karne wala method
    // Is method ko Restart button ke OnClick event se call karenge
    public void RestartLevel()
    {
        PlayButtonSound();
        // Agar game pause thi to time ko normal speed par lana
        // Time.timeScale = 1 ka matlab normal game speed
        Time.timeScale = 1f;

        // Jo scene abhi open hai uska build index lena
        int currentSceneIndex =
            SceneManager.GetActiveScene().buildIndex;

        // Current scene ko usi build index se dobara load karna
        // Is se poora level restart ho jayega
        SceneManager.LoadScene(currentSceneIndex);
    }

    // Puri game ko first level se dobara start karna
    public void RestartGame()
    {
        PlayButtonSound();

        // Agar game paused hai to time normal karna
        Time.timeScale = 1f;

        // Build Profiles mein index 0 wali scene load karna
        SceneManager.LoadScene(0);
    }


    // Next level load karne wala method
    // Is method ko Next Level button ke OnClick event se call karenge
    public void LoadNextLevel()
    {
        PlayButtonSound();

        // Agar game pause thi to next scene load karne se pehle
        // time ko normal speed par lana
        Time.timeScale = 1f;

        int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;

        if (LevelManager.instance != null)
        {
            bool hasNextLevel = LevelManager.instance.LevelCompleted();

            if (hasNextLevel)
            {
                if (UIManager.instance != null)
                {
                    UIManager.instance.HideLevelComplete();
                }
            }
            else
            {
                if (UIManager.instance != null)
                {
                    UIManager.instance.ShowGameComplete();
                }
            }
        }
    }
        // Current scene ke build index mein 1 add karna
        // Is se next scene ka index milega
//        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check karna ke next scene Build Profiles mein mojood hai ya nahi
        //
        // Example:
        // Total scenes = 2
        // Valid indexes = 0 aur 1
        //if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        //{
            // Agar next scene available hai
            // to us scene ko load karna
            //SceneManager.LoadScene(nextSceneIndex);
        //}
        //else
        //{
            // Agar next scene available nahi hai
            // to iska matlab saare levels co45mplete ho gaye
            //UIManager.instance.ShowGameComplete();

            // Baad mein yahan Game Complete panel show karenge
            // UIManager.instance.ShowGameComplete();
        //}
    //}

    // Main Menu se Level 1 start karna
    public void StartGame()
    {
        PlayButtonSound();

        // Game time normal rakhna
        Time.timeScale = 1f;

        // Build Profiles mein Level 1 ka index load karna
        SceneManager.LoadScene(1);
    }

    // Main Menu scene load karna
    public void LoadMainMenu()
    {
        PlayButtonSound();
        // Agar game paused ho to time normal karna
        Time.timeScale = 1f;

        // Build Profiles mein index 0 wali MainMenu scene load karna
        SceneManager.LoadScene(0);
    }

    public void LoseLife(BallMovement ballMovement)
    {
        lives--;

        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayLoseLife();
        }


        UIManager.instance.UpdateLivesText(lives);

        if (lives <= 0)
        {
            if (AudioClipManager.instance)
            {
                AudioClipManager.instance.PlayGameOver();
            }
            UIManager.instance.ShowGameOver();
        }
        else
        {
            if (AudioClipManager.instance)
            {
                AudioClipManager.instance.PlayLoseLife();
            }

            ballMovement.ResetBall();
        }
    }

    // Game ko pause karna
    public void PauseGame()
    {
        PlayButtonSound();

        isPaused = true;
        Debug.Log(isPaused);

        // Game ka waqt rokna
        Time.timeScale = 0f;

        // Pause panel show karna
        UIManager.instance.ShowPausePanel();
    }

    // Game ko dobara continue karna
    public void ResumeGame()
    {
        PlayButtonSound();

        isPaused = false;

        // Game ka waqt normal karna
        Time.timeScale = 1f;

        // Pause panel hide karna
        UIManager.instance.HidePausePanel();
    }

    private void PlayButtonSound()
    {
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayButtonClick();
        }
    }
    public void QuitGame()
    {
        PlayButtonSound();
        Application.Quit();
    }

}