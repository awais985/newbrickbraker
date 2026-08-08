using Unity.VisualScripting;
using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    public static AudioClipManager instance;

    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip brickBreak;
    [SerializeField] private AudioClip paddleHit;
    [SerializeField] private AudioClip boundaryHitHit;
    [SerializeField] private AudioClip loseLife;
    [SerializeField] private AudioClip levelComplete;
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip panelSound;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayButtonClick()
    {
        if (AudioManager.instance != null && buttonClick != null)
        {
            AudioManager.instance.PlaySFX(buttonClick);
        }
    }

    public void PlayBrickBreak()
    {
        if (AudioManager.instance != null && brickBreak != null)
        {
            AudioManager.instance.PlaySFX(brickBreak);
        }
    }

    public void PlayPaddleHit()
    {
        if (AudioManager.instance != null && paddleHit != null)
        {
            AudioManager.instance.PlaySFX(paddleHit);
        }
    }

    public void PlayBoundaryHit()
    {
        if (AudioManager.instance != null && boundaryHitHit!= null)
        {
            AudioManager.instance.PlaySFX(boundaryHitHit);
        }
    }

    public void PlayLoseLife()
    {
        if (AudioManager.instance != null && loseLife != null)
        {
            AudioManager.instance.PlaySFX(loseLife);
        }
    }

    public void PlayGameOver()
    {
        if (AudioManager.instance != null && levelComplete != null)
        {
            AudioManager.instance.PlaySFX(gameOver);
        }
    }

    public void PlayLevelComplete()
    {
        if (AudioManager.instance != null && levelComplete != null)
        {
            AudioManager.instance.PlaySFX(levelComplete);
        }
    }

    public void PlayPanelSound()
    {
        if (AudioManager.instance != null && panelSound != null)
        {
            AudioManager.instance.PlaySFX(panelSound);
        }
    }
}