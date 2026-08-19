using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // =========================================================
    // SINGLETON
    // =========================================================

    // Doosri scripts se AudioManager ko globally access karne ke liye
    public static AudioManager instance;


    // =========================================================
    // AUDIO SOURCES
    // =========================================================

    [Header("Audio Sources")]

    // Background music play karega
    [SerializeField] private AudioSource musicSource;

    // Button, brick, paddle waghera ke SFX play karega
    [SerializeField] private AudioSource sfxSource;


    // =========================================================
    // MUSIC SETTINGS
    // =========================================================

    [Header("Music Settings")]

    // Music fade-in / fade-out speed
    [SerializeField] private float musicFadeSpeed = 2f;

    // Music ki maximum actual volume
    //
    // Example:
    // Slider = 1.0
    // Max Limit = 0.6
    // Actual Volume = 0.6
    [SerializeField, Range(0f, 1f)]
    private float maxMusicVolumeLimit = 0.6f;


    // =========================================================
    // PLAYER PREF KEYS
    // =========================================================

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private const string MusicMutedKey = "MusicMuted";
    private const string SFXMutedKey = "SFXMuted";


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    // Currently running music transition coroutine
    private Coroutine musicTransition;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        // Agar pehle se AudioManager mojood hai
        // to duplicate destroy karna
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }


        // Current object ko main AudioManager banana
        instance = this;


        // Scene change ke baad bhi AudioManager alive rahega
        DontDestroyOnLoad(gameObject);


        // Saved audio settings load karna
        LoadMusicSettings();
        LoadSFXSettings();
    }


    // =========================================================
    // LOAD MUSIC SETTINGS
    // =========================================================

    private void LoadMusicSettings()
    {
        if (musicSource == null)
        {
            return;
        }


        // Saved mute setting
        //
        // Default 0 = unmuted
        int savedMusicMute =
            PlayerPrefs.GetInt(
                MusicMutedKey,
                0
            );


        // Saved slider volume
        //
        // Default 1 = slider full
        float savedMusicVolume =
            PlayerPrefs.GetFloat(
                MusicVolumeKey,
                1f
            );


        savedMusicVolume =
            Mathf.Clamp01(savedMusicVolume);


        // Mute state apply
        musicSource.mute =
            savedMusicMute == 1;


        // Actual music volume par
        // maximum limit apply karna
        musicSource.volume =
            savedMusicVolume *
            maxMusicVolumeLimit;
    }


    // =========================================================
    // LOAD SFX SETTINGS
    // =========================================================

    private void LoadSFXSettings()
    {
        if (sfxSource == null)
        {
            return;
        }


        // Saved mute state
        int savedSFXMute =
            PlayerPrefs.GetInt(
                SFXMutedKey,
                0
            );


        // Saved SFX volume
        float savedSFXVolume =
            PlayerPrefs.GetFloat(
                SFXVolumeKey,
                1f
            );


        savedSFXVolume =
            Mathf.Clamp01(savedSFXVolume);


        // Settings apply
        sfxSource.mute =
            savedSFXMute == 1;

        sfxSource.volume =
            savedSFXVolume;
    }


    // =========================================================
    // PLAY SFX
    // =========================================================

    public void PlaySFX(AudioClip clip)
    {
        // Missing clip/source ko ignore karna
        if (clip == null ||
            sfxSource == null)
        {
            return;
        }


        // PlayOneShot se multiple SFX
        // ek hi source par overlap kar sakti hain
        sfxSource.PlayOneShot(clip);
    }


    // =========================================================
    // PLAY / CHANGE MUSIC
    // =========================================================

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null ||
            musicSource == null)
        {
            return;
        }


        // Agar same music already chal rahi hai
        // to usko restart nahi karna
        if (musicSource.clip == musicClip &&
            musicSource.isPlaying)
        {
            return;
        }


        // Previous transition chal rahi ho
        // to usko stop karna
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }


        // New transition start
        musicTransition =
            StartCoroutine(
                ChangeMusicRoutine(
                    musicClip
                )
            );
    }


    // =========================================================
    // MUSIC FADE TRANSITION
    // =========================================================

    private IEnumerator ChangeMusicRoutine(
        AudioClip newMusicClip
    )
    {
        // Player ki saved slider value
        float savedVolume =
            PlayerPrefs.GetFloat(
                MusicVolumeKey,
                1f
            );


        savedVolume =
            Mathf.Clamp01(savedVolume);


        // IMPORTANT:
        // Maximum music limit yahan bhi apply karni hai
        float targetVolume =
            savedVolume *
            maxMusicVolumeLimit;


        // -----------------------------------------------------
        // FADE OUT OLD MUSIC
        // -----------------------------------------------------

        if (musicSource.isPlaying)
        {
            while (musicSource.volume > 0f)
            {
                musicSource.volume =
                    Mathf.MoveTowards(
                        musicSource.volume,
                        0f,
                        musicFadeSpeed *
                        Time.unscaledDeltaTime
                    );

                yield return null;
            }
        }


        // Old music completely stop
        musicSource.Stop();


        // -----------------------------------------------------
        // SET NEW MUSIC
        // -----------------------------------------------------

        musicSource.clip =
            newMusicClip;


        // Background music continuously repeat hogi
        musicSource.loop =
            true;


        // Fade-in zero volume se start
        musicSource.volume =
            0f;


        musicSource.Play();


        // -----------------------------------------------------
        // FADE IN NEW MUSIC
        // -----------------------------------------------------

        while (musicSource.volume <
               targetVolume)
        {
            musicSource.volume =
                Mathf.MoveTowards(
                    musicSource.volume,
                    targetVolume,
                    musicFadeSpeed *
                    Time.unscaledDeltaTime
                );

            yield return null;
        }


        // Exact final volume
        musicSource.volume =
            targetVolume;


        // Transition finish
        musicTransition =
            null;
    }


    // =========================================================
    // PAUSE MUSIC
    // =========================================================

    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }


    // =========================================================
    // RESUME MUSIC
    // =========================================================

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }


    // =========================================================
    // STOP MUSIC
    // =========================================================

    public void StopMusic()
    {
        if (musicSource == null)
        {
            return;
        }


        // Current fade transition stop
        if (musicTransition != null)
        {
            StopCoroutine(
                musicTransition
            );

            musicTransition =
                null;
        }


        musicSource.Stop();
    }


    // =========================================================
    // SET SFX VOLUME
    // =========================================================

    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null)
        {
            return;
        }


        // Slider value 0–1 ke andar
        float safeVolume =
            Mathf.Clamp01(volume);


        // AudioSource par apply
        sfxSource.volume =
            safeVolume;


        // Save setting
        PlayerPrefs.SetFloat(
            SFXVolumeKey,
            safeVolume
        );

        PlayerPrefs.Save();
    }


    // =========================================================
    // SET MUSIC VOLUME
    // =========================================================

    public void SetMusicVolume(float volume)
    {
        if (musicSource == null)
        {
            return;
        }


        // Slider value ko 0–1 range mein rakhna
        float safeVolume =
            Mathf.Clamp01(volume);


        // Actual AudioSource volume
        //
        // Slider 100% ho tab bhi
        // maxMusicVolumeLimit se zyada nahi jayegi
        musicSource.volume =
            safeVolume *
            maxMusicVolumeLimit;


        // PlayerPrefs mein slider ki
        // original 0–1 value save karna
        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            safeVolume
        );

        PlayerPrefs.Save();
    }


    // =========================================================
    // TOGGLE SFX MUTE
    // =========================================================

    public void ToggleSFXMute()
    {
        if (sfxSource == null)
        {
            return;
        }


        // Current state reverse
        sfxSource.mute =
            !sfxSource.mute;


        // Save:
        // true  = 1
        // false = 0
        PlayerPrefs.SetInt(
            SFXMutedKey,
            sfxSource.mute ? 1 : 0
        );

        PlayerPrefs.Save();
    }


    // =========================================================
    // TOGGLE MUSIC MUTE
    // =========================================================

    public void ToggleMusicMute()
    {
        if (musicSource == null)
        {
            return;
        }


        // Current mute state reverse
        musicSource.mute =
            !musicSource.mute;


        // Save state
        PlayerPrefs.SetInt(
            MusicMutedKey,
            musicSource.mute ? 1 : 0
        );

        PlayerPrefs.Save();
    }


    // =========================================================
    // OPTIONAL GETTERS
    // =========================================================

    // Settings UI ko current Music volume chahiye ho
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(
            MusicVolumeKey,
            1f
        );
    }


    // Settings UI ko current SFX volume chahiye ho
    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(
            SFXVolumeKey,
            1f
        );
    }


    public bool IsMusicMuted()
    {
        return musicSource != null &&
               musicSource.mute;
    }


    public bool IsSFXMuted()
    {
        return sfxSource != null &&
               sfxSource.mute;
    }
}