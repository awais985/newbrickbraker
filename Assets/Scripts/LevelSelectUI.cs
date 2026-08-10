using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonParent;
    private int totalLevels;
    //[SerializeField] private GameObject[] lockIcons;

    private void Start()
    {
        GenerateLevelButtons();
    }

    private void GenerateLevelButtons()
    {
        if(LevelManager.instance != null)
        {
            totalLevels = LevelManager.instance.TotalLevels();
        }
        for (int i = 0; i < totalLevels; i++)
        {
            int levelNumber = i + 1;

            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

            bool unlocked = levelNumber <= unlockedLevel;

            GameObject newButton =
                Instantiate(levelButtonPrefab, buttonParent);

            LevelButtonUI buttonUI =
                newButton.GetComponent<LevelButtonUI>();

            buttonUI.Setup(levelNumber, unlocked);

            Button button =
                newButton.GetComponent<Button>();

            button.onClick.AddListener(() =>
                SelectLevel(levelNumber)
            );
        }
    }


    public void SelectLevel(int levelNumber)
    {
        int levelIndex = levelNumber - 1;

        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Gameplay");
    }

}
