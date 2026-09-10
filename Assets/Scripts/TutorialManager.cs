using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // =========================================================
    // TUTORIAL PANELS
    // =========================================================

    [Header("Tutorial Panels")]

    [SerializeField]
    private GameObject mobileTutorialPanel;

    [SerializeField]
    private GameObject desktopTutorialPanel;


    // =========================================================
    // TESTING
    // =========================================================

    [Header("Editor Testing")]

    // Unity Editor mein mobile tutorial test karne ke liye
    [SerializeField]
    private bool forceMobileInEditor = false;


    // =========================================================
    // PLAYER PREFS
    // =========================================================

    private const string TutorialSeenKey =
        "BrickBreaker_TutorialSeen";


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    private GameObject currentTutorialPanel;

    private float previousTimeScale;

    private bool tutorialOpen;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Pehle dono hide
        HideAllPanels();


        // Check tutorial pehle dekha hai ya nahi
        int tutorialSeen =
            PlayerPrefs.GetInt(
                TutorialSeenKey,
                0
            );


        if (tutorialSeen == 0)
        {
            ShowCorrectTutorial();
        }
    }


    // =========================================================
    // SHOW CORRECT TUTORIAL
    // =========================================================

    private void ShowCorrectTutorial()
    {
        bool isMobile =
            Application.isMobilePlatform;


#if UNITY_EDITOR

        // Editor testing ke liye
        if (forceMobileInEditor)
        {
            isMobile = true;
        }

#endif


        if (isMobile)
        {
            currentTutorialPanel =
                mobileTutorialPanel;
        }
        else
        {
            currentTutorialPanel =
                desktopTutorialPanel;
        }


        ShowTutorial();
    }


    // =========================================================
    // SHOW TUTORIAL
    // =========================================================

    private void ShowTutorial()
    {
        if (currentTutorialPanel == null)
        {
            Debug.LogError(
                "TutorialManager: Tutorial panel assign nahi hai."
            );

            return;
        }


        previousTimeScale =
            Time.timeScale;


        // Game pause
        Time.timeScale = 0f;


        currentTutorialPanel.SetActive(
            true
        );


        tutorialOpen = true;
    }


    // =========================================================
    // CLOSE TUTORIAL
    // =========================================================

    public void CloseTutorial()
    {
        if (!tutorialOpen)
        {
            return;
        }


        PlayerPrefs.SetInt(
            TutorialSeenKey,
            1
        );

        PlayerPrefs.Save();


        if (currentTutorialPanel != null)
        {
            currentTutorialPanel.SetActive(
                false
            );
        }


        tutorialOpen = false;


        Time.timeScale =
            previousTimeScale;
    }


    // =========================================================
    // SHOW AGAIN
    // =========================================================

    // Main Menu / Settings ke
    // How To Play button se call kar sakte ho
    public void ShowTutorialAgain()
    {
        HideAllPanels();

        ShowCorrectTutorial();
    }


    // =========================================================
    // HIDE ALL
    // =========================================================

    private void HideAllPanels()
    {
        if (mobileTutorialPanel != null)
        {
            mobileTutorialPanel.SetActive(
                false
            );
        }


        if (desktopTutorialPanel != null)
        {
            desktopTutorialPanel.SetActive(
                false
            );
        }


        tutorialOpen = false;
    }


    // =========================================================
    // RESET TUTORIAL
    // =========================================================

    public void ResetTutorialData()
    {
        PlayerPrefs.DeleteKey(
            TutorialSeenKey
        );

        PlayerPrefs.Save();


        Debug.Log(
            "Tutorial data reset."
        );
    }
}