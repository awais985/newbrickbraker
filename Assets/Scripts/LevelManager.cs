using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("Level References")]

    // Saare levels ka catalog / collection
    [SerializeField] private LevelCatalog levelCatalog;

    // Current level ke bricks build / clear karne wala spawner
    [SerializeField] private BrickSpawner brickSpawner;


    // =========================================================
    // SINGLETON
    // =========================================================

    // Puri game mein LevelManager ka global reference
    public static LevelManager instance;


    // =========================================================
    // RUNTIME LEVEL DATA
    // =========================================================

    // Current level ka array index
    //
    // Example:
    // Level 1 = index 0
    // Level 2 = index 1
    private int currentIndex;

    // Jo level currently load hai uska data
    private LevelData currentLevelData;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Agar already koi LevelManager mojood hai
        // aur woh current object nahi hai
        if (instance != null && instance != this)
        {
            // Duplicate remove karna
            Destroy(gameObject);
            return;
        }

        // Current LevelManager ko main instance banana
        instance = this;
    }


    private void Start()
    {
        // Safety check
        if (levelCatalog == null ||
            levelCatalog.levels == null ||
            levelCatalog.levels.Length == 0)
        {
            Debug.LogError("LevelCatalog mein koi level available nahi hai.");
            return;
        }

        if (brickSpawner == null)
        {
            Debug.LogError("BrickSpawner assign nahi hai.");
            return;
        }


        // Level Select scene mein player ne jo level select kiya tha
        // uska index PlayerPrefs se lena
        //
        // Agar key available nahi ho
        // to default index 0 = Level 1
        int selectedLevel =
            PlayerPrefs.GetInt(
                "SelectedLevel",
                0
            );


        // Selected index ko valid range mein rakhna
        currentIndex = Mathf.Clamp(
            selectedLevel,
            0,
            levelCatalog.levels.Length - 1
        );


        // Selected level build karna
        LoadLevel(currentIndex);
    }


    // =========================================================
    // TOTAL LEVEL COUNT
    // =========================================================

    public int TotalLevels()
    {
        // Agar catalog available nahi hai
        if (levelCatalog == null ||
            levelCatalog.levels == null)
        {
            return 0;
        }

        return levelCatalog.levels.Length;
    }


    // =========================================================
    // CHECK NEXT LEVEL
    // =========================================================

    public bool CheckNextLevelAvailable()
    {
        // Agar current index last level se pehle hai
        // to next level available hai
        return currentIndex <
               levelCatalog.levels.Length - 1;
    }


    // =========================================================
    // LEVEL COMPLETED
    // =========================================================

    public bool LevelCompleted()
    {
        // Check karna ke next level available hai
        if (!CheckNextLevelAvailable())
        {
            // Current level last level tha
            return false;
        }


        // Player ke liye next level unlock karna
        UnlockNextLevel();


        // Next level index par jana
        currentIndex++;


        // Next level build karna
        LoadLevel(currentIndex);


        return true;
    }


    // =========================================================
    // LOAD LEVEL
    // =========================================================

    public void LoadLevel(int index)
    {
        // Safety check
        if (levelCatalog == null ||
            levelCatalog.levels == null ||
            levelCatalog.levels.Length == 0)
        {
            return;
        }


        if (brickSpawner == null)
        {
            return;
        }


        // Invalid index se bachna
        index = Mathf.Clamp(
            index,
            0,
            levelCatalog.levels.Length - 1
        );


        // Current index update karna
        currentIndex = index;


        // Previous level ke bricks remove karna
        brickSpawner.ClearLevel();


        // Required LevelData lena
        currentLevelData =
            levelCatalog.levels[currentIndex];


        // Safety check
        if (currentLevelData == null)
        {
            Debug.LogError(
                "LevelData missing at index: " +
                currentIndex
            );

            return;
        }


        // New level ke bricks create karna
        brickSpawner.BuildLevel(
            currentLevelData
        );
    }


    // =========================================================
    // RESTART CURRENT LEVEL
    // =========================================================

    public void RestartCurrentLevel()
    {
        // Same current index dobara load karna
        LoadLevel(currentIndex);
    }


    // =========================================================
    // UNLOCK NEXT LEVEL
    // =========================================================

    public void UnlockNextLevel()
    {
        // Player ka currently highest unlocked level lena
        //
        // Default = 1
        // matlab new player ke liye Level 1 unlocked hai
        int unlockedLevel =
            PlayerPrefs.GetInt(
                "UnlockedLevel",
                1
            );


        // currentIndex zero-based hai:
        //
        // currentIndex 0 = Level 1
        //
        // Complete Level 1
        // → unlock Level 2
        //
        // Isliye +2
        int nextLevelNumber =
            currentIndex + 2;


        // Maximum actual levels se zyada
        // unlock number save nahi karna
        nextLevelNumber = Mathf.Min(
            nextLevelNumber,
            TotalLevels()
        );


        // Sirf tab PlayerPrefs update karna
        // jab player ne naya highest level unlock kiya ho
        if (nextLevelNumber > unlockedLevel)
        {
            PlayerPrefs.SetInt(
                "UnlockedLevel",
                nextLevelNumber
            );


            // Data disk par save karna
            PlayerPrefs.Save();
        }
    }


    // =========================================================
    // OPTIONAL GETTERS
    // =========================================================

    // UI waghera ko current level number chahiye ho
    public int GetCurrentLevelNumber()
    {
        // Human-readable level number
        return currentIndex + 1;
    }


    // Current LevelData ki zaroorat ho
    public LevelData GetCurrentLevelData()
    {
        return currentLevelData;
    }
}