using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button button;
   // [SerializeField] private GameObject lockIcon;
    public void Setup(int levelNumber,bool unlocked)
    {
        levelText.text = levelNumber.ToString();

        button.interactable = unlocked;

        //if (lockIcon != null)
        //{
        //    lockIcon.SetActive(!unlocked);
        //}
    }
}
