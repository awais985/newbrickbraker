using UnityEngine;

public class SceneMusicController : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;

    private void Start()
    {
        if (AudioManager.instance != null && sceneMusic != null)
        {
            AudioManager.instance.PlayMusic(sceneMusic);
        }
    }
}