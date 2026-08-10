using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData[] levels;
    [SerializeField] private BrickSpawner brickSpawner;
    //public BrickData[] brickLayout;

    private int currentIndex;
    private LevelData currentLevelData;
    public static LevelManager instance;

    private void Awake()
    {
        if(instance!= null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 0);

        currentIndex = Mathf.Clamp(
            selectedLevel,
            0,
            levels.Length - 1
        );


        LoadLevel(currentIndex);
    }

    public int TotalLevels()
    {
        return levels.Length;
    }

    public bool CheckNextLevelAvailable()
    {
        if (currentIndex < levels.Length - 1)
        {
            return true;
        }
        return false;
    }

    public bool LevelCompleted()
    {
        if(currentIndex < levels.Length - 1)
        {
            currentIndex++;

            LoadLevel(currentIndex);
            
            return true;
        }

        return false;
    }
    public void LoadLevel(int index)
    {
        brickSpawner.ClearLevel();

        currentLevelData = levels[index];
        
        brickSpawner.BuildLevel(currentLevelData);
    }
    public void RestartCurrentLevel()
    {
        LoadLevel(currentIndex);
    }
    public void UnlockNextLevel()
    {
       
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        int nextLevelNumber = currentIndex + 2;

        if (nextLevelNumber > unlockedLevel)
        {
            PlayerPrefs.SetInt(
                "UnlockedLevel",
                nextLevelNumber
            );

            PlayerPrefs.Save();
        }
    }
}
