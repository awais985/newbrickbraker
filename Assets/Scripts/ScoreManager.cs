using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Puri game mein ScoreManager ka single global reference
    // Iske through doosri scripts score system ko access kar sakti hain
    public static ScoreManager instance;

    // Player ka current score store karega
    private int score;

    private void Awake()
    {
        // Agar pehle se koi ScoreManager instance mojood hai
        // aur woh current object nahi hai
        if (instance != null && instance != this)
        {
            // Duplicate ScoreManager GameObject destroy karna
            Destroy(gameObject);

            // Neeche ka code run nahi karna
            return;
        }

        // Current ScoreManager ko main instance banana
        instance = this;
    }

    // Score mein naya amount add karne wala method
    // Example:
    // AddScore(10) se score mein 10 add hoga
    public void AddScore(int amount)
    {
        // Current score mein received amount add karna
        score += amount;

        // Check karna ke UIManager available hai
        if (UIManager.instance != null)
        {
            // UI par updated score show karna
            UIManager.instance.UpdateScoreText(score);
        }
    }
}