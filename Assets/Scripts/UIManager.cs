using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Puri game mein UIManager ka single global reference
    public static UIManager instance;

    // Canvas par current score show karne wala TextMeshPro text
    [SerializeField] private TextMeshProUGUI scoreText;

    // Canvas par current Live show karne wala TextMeshPro text
    [SerializeField] private TextMeshProUGUI liveText;


    // Current level complete hone par show hone wala panel
    [SerializeField] private GameObject levelCompletePanel;

    // Saare levels complete hone par show hone wala panel
    [SerializeField] private GameObject gameCompletePanel;

    // Game Over hone par show hone wala panel
    [SerializeField] private GameObject gameOverPanel;

    // Game Pause hone par show hone wala panel
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private GameObject pauseButton;

    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        // Agar pehle se koi UIManager instance mojood hai
        // aur woh current object nahi hai
        if (instance != null && instance != this)
        {
            // Duplicate UIManager destroy karna
            Destroy(gameObject);

            // Neeche ka code run nahi karna
            return;
        }

        // Current UIManager ko main instance banana
        instance = this;
    }

    private void Start()
    {
        if(levelCompletePanel != null)
        {
            // Game start par Level Complete panel hidden rakhna
            levelCompletePanel.SetActive(false);
        }

        if (gameCompletePanel != null)
        {
            // Game start par Game Complete panel hidden rakhna
            gameCompletePanel.SetActive(false);
        }
        // Game start par score 0 show karna
        UpdateScoreText(0);
    }

    // ScoreManager se updated score receive karke
    // Canvas par show karna
    public void UpdateScoreText(int score)
    {
        if(scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // Jab current level ki saari bricks destroy ho jayein
    // to yeh method call hoga
    public void ShowLevelComplete()
    {
        if(levelCompletePanel != null && AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayLevelComplete();
            // Level Complete panel visible karna
            levelCompletePanel.SetActive(true);

            // Game ki physics aur time-based movement pause karna
            Time.timeScale = 0f;
        }
    }

    public void HideLevelComplete()
    {
        if (levelCompletePanel != null && AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayLevelComplete();
            // Level Complete panel visible karna
            levelCompletePanel.SetActive(false);

            // Game ki physics aur time-based movement pause karna
            Time.timeScale = 1f;
        }
    }

    // Jab last level bhi complete ho jaye
    // aur koi next scene available na ho
    // to yeh method call hoga
    public void ShowGameComplete()
    {
        if(levelCompletePanel != null && gameCompletePanel!= null && AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayLevelComplete();

            // Pehle Level Complete panel hide karna
            // taake dono panels ek saath show na hon
            levelCompletePanel.SetActive(false);

            // Game Complete panel visible karna
            gameCompletePanel.SetActive(true);

            // Game ko paused rakhna
            Time.timeScale = 0f;

        }
    }

    public void UpdateLivesText(int lives)
    {
        if(liveText != null && AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayLoseLife();
            liveText.text = "Lives: " + lives.ToString();
        }
    }

    public void ShowGameOver()
    {


        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ShowPausePanel()
    {
        if(pausePanel != null && pauseButton != null && AudioClipManager.instance != null)  
        {
            AudioClipManager.instance.PlayPanelSound();
            pausePanel.SetActive(true);
            pauseButton.SetActive(false);
        }
    }

    public void HidePausePanel()
    {
        if (pausePanel != null && pauseButton != null)
        {
            pausePanel.SetActive(false);
            pauseButton.SetActive(true);
        }
    }

    public void ShowSettingPanel() {
        if(settingsPanel != null && AudioClipManager.instance != null)
        {
            AudioClipManager.instance.PlayPanelSound();

            settingsPanel.SetActive(true);
        }
    }

    public void HideSettingPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}