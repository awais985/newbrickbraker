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
        currentIndex = 0;
        LoadLevel(currentIndex);
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
}
