using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    // Puri game mein UIManager ka global reference
    public static UIManager instance;


    // =========================================================
    // HUD REFERENCES
    // =========================================================

    [Header("HUD")]

    // Current score show karne wala TMP text
    [SerializeField] private TextMeshProUGUI scoreText;

    // Agar text-based lives bhi use karni hain
    [SerializeField] private TextMeshProUGUI liveText;


    // =========================================================
    // PANEL REFERENCES
    // =========================================================

    [Header("Panels")]

    // Current level complete hone par show hoga
    [SerializeField] private GameObject levelCompletePanel;

    // Saare levels complete hone par show hoga
    [SerializeField] private GameObject gameCompletePanel;

    // Lives 0 hone par show hoga
    [SerializeField] private GameObject gameOverPanel;

    // Pause menu
    [SerializeField] private GameObject pausePanel;

    // Settings menu
    [SerializeField] private GameObject settingsPanel;


    // =========================================================
    // BUTTON REFERENCES
    // =========================================================

    [Header("Buttons")]

    // Gameplay ka Pause button
    [SerializeField] private GameObject pauseButton;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Duplicate UIManager ko destroy karna
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Current UIManager ko main instance banana
        instance = this;
    }


    private void Start()
    {
        // Game start par temporary panels hide rakhna
        SetPanelState(levelCompletePanel, false);
        SetPanelState(gameCompletePanel, false);
        SetPanelState(gameOverPanel, false);
        SetPanelState(pausePanel, false);
        SetPanelState(settingsPanel, false);

        // Pause button normal gameplay mein visible
        SetPanelState(pauseButton, true);

        // Starting score
        UpdateScoreText(0);
    }


    // =========================================================
    // SCORE UI
    // =========================================================

    public void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Score: " + score;
        }
    }


    // =========================================================
    // LIVES UI
    // =========================================================

    public void UpdateLivesText(int lives)
    {
        // Ye method sirf UI update karega.
        // Sound GameManager handle karega.

        if (liveText != null)
        {
            liveText.text =
                "Lives: " + lives;
        }
    }


    // =========================================================
    // LEVEL COMPLETE
    // =========================================================

    public void ShowLevelComplete()
    {
        // Level complete sound sirf panel show
        // hone ke waqt play karna
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance
                .PlayLevelComplete();
        }

        SetPanelState(
            levelCompletePanel,
            true
        );

        // Gameplay freeze
        Time.timeScale = 0f;
    }


    public void HideLevelComplete()
    {
        // Panel hide karna
        SetPanelState(
            levelCompletePanel,
            false
        );

        // Gameplay resume
        Time.timeScale = 1f;
    }


    // =========================================================
    // GAME COMPLETE
    // =========================================================

    public void ShowGameComplete()
    {
        // Level Complete panel ko hide rakhna
        SetPanelState(
            levelCompletePanel,
            false
        );

        // Game Complete panel show
        SetPanelState(
            gameCompletePanel,
            true
        );

        // Completion sound
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance
                .PlayLevelComplete();
        }

        // Game freeze
        Time.timeScale = 0f;
    }


    // =========================================================
    // GAME OVER
    // =========================================================

    public void ShowGameOver()
    {
        SetPanelState(
            gameOverPanel,
            true
        );

        Time.timeScale = 0f;
    }


    // =========================================================
    // PAUSE PANEL
    // =========================================================

    public void ShowPausePanel()
    {
        // Panel opening sound
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance
                .PlayPanelSound();
        }

        // Pause menu show
        SetPanelState(
            pausePanel,
            true
        );

        // Pause button hide
        SetPanelState(
            pauseButton,
            false
        );
    }


    public void HidePausePanel()
    {
        // Pause menu hide
        SetPanelState(
            pausePanel,
            false
        );

        // Pause button wapas show
        SetPanelState(
            pauseButton,
            true
        );
    }


    // =========================================================
    // SETTINGS PANEL
    // =========================================================

    public void ShowSettingPanel()
    {
        // Panel opening sound
        if (AudioClipManager.instance != null)
        {
            AudioClipManager.instance
                .PlayPanelSound();
        }

        SetPanelState(
            settingsPanel,
            true
        );
    }


    public void HideSettingPanel()
    {
        SetPanelState(
            settingsPanel,
            false
        );
    }


    // =========================================================
    // HELPER METHOD
    // =========================================================

    // Repeated null check + SetActive code
    // ek jagah handle karna
    private void SetPanelState(
        GameObject panel,
        bool state
    )
    {
        if (panel != null)
        {
            panel.SetActive(state);
        }
    }
}