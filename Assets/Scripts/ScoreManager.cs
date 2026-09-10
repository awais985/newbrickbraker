using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    public static ScoreManager instance;


    // =========================================================
    // SCORE DATA
    // =========================================================

    // Current level mein player ka score
    private int score;

    // Human-readable level number
    // Level 1 = 1
    // Level 2 = 2
    private int currentLevelNumber = 1;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    // =========================================================
    // START / RESET LEVEL SCORE
    // =========================================================

    public void StartLevel(int levelNumber)
    {
        // Current level save
        currentLevelNumber = levelNumber;

        // Har level ka current score 0 se start
        score = 0;


        // UI update
        if (UIManager.instance != null)
        {
            // Current score
            UIManager.instance.UpdateScoreText(score);

            // Isi level ka saved Best Score
            UIManager.instance.UpdateBestScoreText(
                GetBestScore()
            );
        }
    }


    // =========================================================
    // ADD SCORE
    // =========================================================

    public void AddScore(int amount)
    {
        // Score add
        score += amount;


        // Current score UI
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateScoreText(score);
        }


        // Current level ka previous Best Score
        int bestScore = GetBestScore();


        // Agar current score previous best se zyada hai
        if (score > bestScore)
        {
            // New Best save
            PlayerPrefs.SetInt(
                GetBestScoreKey(),
                score
            );

            PlayerPrefs.Save();


            // Best Score UI update
            if (UIManager.instance != null)
            {
                UIManager.instance.UpdateBestScoreText(
                    score
                );
            }
        }
    }


    // =========================================================
    // CURRENT SCORE GETTER
    // =========================================================

    public int GetCurrentScore()
    {
        return score;
    }


    // =========================================================
    // BEST SCORE GETTER
    // =========================================================

    public int GetBestScore()
    {
        return PlayerPrefs.GetInt(
            GetBestScoreKey(),
            0
        );
    }


    // =========================================================
    // BEST SCORE PLAYERPREF KEY
    // =========================================================

    private string GetBestScoreKey()
    {
        return "BestScore_Level_" +
               currentLevelNumber;
    }
}