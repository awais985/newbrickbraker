using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    // Doosri scripts se AudioClipManager ko
    // globally access karne ke liye
    public static AudioClipManager instance;


    // =========================================================
    // AUDIO CLIPS
    // =========================================================

    [Header("UI Sounds")]

    // Button click sound
    [SerializeField] private AudioClip buttonClick;

    // Panel open sound
    [SerializeField] private AudioClip panelSound;


    [Header("Gameplay Sounds")]

    // Brick destroy sound
    [SerializeField] private AudioClip brickBreak;

    // Paddle hit sound
    [SerializeField] private AudioClip paddleHit;

    // Boundary / wall hit sound
    [SerializeField] private AudioClip boundaryHit;

    // Life lose sound
    [SerializeField] private AudioClip loseLife;


    [Header("Game State Sounds")]

    // Level complete sound
    [SerializeField] private AudioClip levelComplete;

    // Game over sound
    [SerializeField] private AudioClip gameOver;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Agar pehle se AudioClipManager mojood hai
        // aur ye duplicate instance hai
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }


        // Current object ko main instance banana
        instance = this;


        // Scene change hone ke baad bhi
        // AudioClipManager destroy nahi hoga
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Debug.Log(gameObject);
    }


    // =========================================================
    // BUTTON SOUND
    // =========================================================

    public void PlayButtonClick()
    {
        PlayClip(buttonClick);
    }


    // =========================================================
    // PANEL SOUND
    // =========================================================

    public void PlayPanelSound()
    {
        PlayClip(panelSound);
    }


    // =========================================================
    // BRICK BREAK SOUND
    // =========================================================

    public void PlayBrickBreak()
    {
        PlayClip(brickBreak);
    }


    // =========================================================
    // PADDLE HIT SOUND
    // =========================================================

    public void PlayPaddleHit()
    {
        PlayClip(paddleHit);
    }


    // =========================================================
    // BOUNDARY HIT SOUND
    // =========================================================

    public void PlayBoundaryHit()
    {
        Debug.Log("Heelo");
        PlayClip(boundaryHit);
    }


    // =========================================================
    // LOSE LIFE SOUND
    // =========================================================

    public void PlayLoseLife()
    {
        PlayClip(loseLife);
    }


    // =========================================================
    // LEVEL COMPLETE SOUND
    // =========================================================

    public void PlayLevelComplete()
    {
        PlayClip(levelComplete);
    }


    // =========================================================
    // GAME OVER SOUND
    // =========================================================

    public void PlayGameOver()
    {
        PlayClip(gameOver);
    }


    // =========================================================
    // HELPER METHOD
    // =========================================================

    // Har method mein same null checks repeat karne ki
    // zaroorat nahi.
    //
    // Ye helper kisi bhi AudioClip ko
    // AudioManager ke through play karega.
    private void PlayClip(AudioClip clip)
    {
        // AudioManager available nahi hai
        if (AudioManager.instance == null)
        {
            return;
        }


        // Clip assign nahi hai
        if (clip == null)
        {
            return;
        }


        // SFX play karna
        AudioManager.instance.PlaySFX(
            clip
        );
    }
}