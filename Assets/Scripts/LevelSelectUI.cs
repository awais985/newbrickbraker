using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private int totalLevels = 10;
    //[SerializeField] private GameObject[] lockIcons;

    private void Start()
    {
        GenerateLevelButtons();
    }

    private void GenerateLevelButtons()
    {
        for (int i = 0; i < totalLevels; i++)
        {
            int levelNumber = i + 1;

            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

            bool unlocked = levelNumber <= unlockedLevel;

            GameObject newButton = Instantiate(levelButtonPrefab, buttonParent);

            LevelButtonUI buttonUI = newButton.GetComponent<LevelButtonUI>();

            buttonUI.Setup(levelNumber, unlocked);

            Button button = newButton.GetComponent<Button>();

            button.onClick.AddListener(() => SelectLevel(levelNumber));
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
