using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("Level Button Setup")]

    // Level button ka prefab
    [SerializeField] private GameObject levelButtonPrefab;

    // Jis parent ke andar saare buttons spawn honge
    // Example:
    // Scroll View -> Viewport -> Content
    [SerializeField] private Transform buttonParent;

    // Total kitne levels show karne hain
    [SerializeField] private int totalLevels = 10;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Start()
    {
        // Scene start hote hi
        // saare level buttons generate karna
        GenerateLevelButtons();
    }


    // =========================================================
    // GENERATE LEVEL BUTTONS
    // =========================================================

    private void GenerateLevelButtons()
    {
        // Safety check
        if (levelButtonPrefab == null)
        {
            Debug.LogError(
                "LevelSelectUI: Level Button Prefab assign nahi hai."
            );

            return;
        }

        if (buttonParent == null)
        {
            Debug.LogError(
                "LevelSelectUI: Button Parent assign nahi hai."
            );

            return;
        }


        // Player ka highest unlocked level lena
        //
        // Default = 1
        // matlab new player ke liye sirf Level 1 unlocked hoga
        int unlockedLevel =
            PlayerPrefs.GetInt(
                "UnlockedLevel",
                1
            );


        // Total levels ke mutabiq buttons create karna
        for (int i = 0; i < totalLevels; i++)
        {
            // Array/index zero-based hai
            //
            // i = 0 → Level 1
            // i = 1 → Level 2
            int levelNumber = i + 1;


            // Check karna ke current level unlocked hai ya locked
            bool unlocked =
                levelNumber <= unlockedLevel;


            // -------------------------------------------------
            // CREATE BUTTON
            // -------------------------------------------------

            GameObject newButton =
                Instantiate(
                    levelButtonPrefab,
                    buttonParent
                );


            // -------------------------------------------------
            // LEVEL BUTTON UI SETUP
            // -------------------------------------------------

            LevelButtonUI buttonUI =
                newButton.GetComponent<LevelButtonUI>();


            if (buttonUI != null)
            {
                // Level number aur lock state bhejna
                buttonUI.Setup(
                    levelNumber,
                    unlocked
                );
            }
            else
            {
                Debug.LogError(
                    "Level Button Prefab par LevelButtonUI script nahi lagi."
                );
            }


            // -------------------------------------------------
            // BUTTON COMPONENT
            // -------------------------------------------------

            Button button =
                newButton.GetComponent<Button>();


            if (button == null)
            {
                Debug.LogError(
                    "Level Button Prefab par Button component nahi laga."
                );

                continue;
            }


            // Locked level ko clickable nahi rakhna
            button.interactable = unlocked;


            // Sirf unlocked level par click listener add karna
            if (unlocked)
            {
                // levelNumber local variable hai,
                // isliye har button apna correct number save karega
                button.onClick.AddListener(
                    () => SelectLevel(levelNumber)
                );
            }
        }
    }


    // =========================================================
    // SELECT LEVEL
    // =========================================================

    public void SelectLevel(int levelNumber)
    {
        // Human-readable level number ko
        // zero-based index mein convert karna
        //
        // Level 1 → index 0
        // Level 2 → index 1
        int levelIndex =
            levelNumber - 1;


        // Selected level index save karna
        PlayerPrefs.SetInt(
            "SelectedLevel",
            levelIndex
        );


        // Data disk par save karna
        PlayerPrefs.Save();


        // Gameplay scene load karna
        SceneManager.LoadScene(
            "Gameplay"
        );
    }
}