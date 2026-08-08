using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // AudioManager ko doosri scripts se access karne ke liye singleton
    public static AudioManager instance;

    // Background music play karne wala AudioSource
    [SerializeField] private AudioSource musicSource;

    // Brick hit, button click waghera play karne wala AudioSource
    [SerializeField] private AudioSource sfxSource;

    // Music fade-out aur fade-in ki speed
    [SerializeField] private float musicFadeSpeed = 2f;

    [SerializeField] private float maxMusicVolumeLimit = 0.6f;


    // Is reference mein currently chalne wali
    // music transition coroutine save hogi
    private Coroutine musicTransition;

    private void Awake()
    {
        // Agar pehle se AudioManager mojood hai
        // aur ye us se alag duplicate object hai
        if (instance != null && instance != this)
        {
            // Duplicate AudioManager ko delete karna
            Destroy(gameObject);
            return;
        }

        // Current AudioManager ko singleton instance banana
        instance = this;

        // Scene change hone par AudioManager ko destroy na karna
        DontDestroyOnLoad(gameObject);

        // Saved music settings load karna
        LoadMusicSettings();

        // Saved SFX settings load karna
        LoadSFXSettings();
    }

    private void LoadMusicSettings()
    {
        // Music AudioSource assign na ho to method rok dena
        if (musicSource == null)
        {
            return;
        }

        // Saved music mute state lena
        // Setting na mile to default 0, yani unmuted
        int savedMusicMute =
            PlayerPrefs.GetInt("MusicMuted", 0);

        // Saved music volume lena
        // Setting na mile to default full volume 1
        float savedMusicVolume =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        // 1 ho to mute true, 0 ho to mute false
        musicSource.mute = savedMusicMute == 1;

        // Saved volume AudioSource par apply karna
        musicSource.volume = savedMusicVolume * maxMusicVolumeLimit;
    }

    private void LoadSFXSettings()
    {
        // SFX AudioSource assign na ho to method rok dena
        if (sfxSource == null)
        {
            return;
        }

        // Saved SFX mute state lena
        int savedSFXMute =
            PlayerPrefs.GetInt("SFXMuted", 0);

        // Saved SFX volume lena
        float savedSFXVolume =
            PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Saved mute state apply karna
        sfxSource.mute = savedSFXMute == 1;

        // Saved volume apply karna
        sfxSource.volume = savedSFXVolume;
    }

    // Short sound effect play karna
    public void PlaySFX(AudioClip clip)
    {
        // Clip ya SFX source missing ho to sound play na karna
        if (clip == null || sfxSource == null)
        {
            return;
        }

        // Ek hi AudioSource se multiple SFX overlap ho sakti hain
        sfxSource.PlayOneShot(clip);
    }

    // Scene ke mutabiq background music play/change karna
    public void PlayMusic(AudioClip musicClip)
    {
        // Music clip ya Music AudioSource missing ho to return
        if (musicClip == null || musicSource == null)
        {
            return;
        }

        // Agar yehi music pehle se chal rahi hai
        // to usay dobara restart na karna
        if (musicSource.clip == musicClip &&
            musicSource.isPlaying)
        {
            return;
        }

        // Agar pehle se koi music transition chal rahi hai
        // to usay stop karna
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
        }

        // Nayi music transition start karke reference save karna
        musicTransition = StartCoroutine(
            ChangeMusicRoutine(musicClip)
        );
    }

    // Purani music fade-out aur nayi music fade-in karna
    private IEnumerator ChangeMusicRoutine(
        AudioClip newMusicClip
    )
    {
        // Player ki saved music volume lena
        float targetVolume =
            PlayerPrefs.GetFloat("MusicVolume", 1f);

        // Value ko safe 0–1 range mein rakhna
        targetVolume = Mathf.Clamp01(targetVolume);

        // Agar pehle koi music chal rahi hai
        // to usay dheere-dheere fade-out karna
        if (musicSource.isPlaying)
        {
            while (musicSource.volume > 0f)
            {
                musicSource.volume = Mathf.MoveTowards(
                    musicSource.volume,
                    0f,
                    musicFadeSpeed * Time.unscaledDeltaTime
                );

                // Next frame tak wait karna
                yield return null;
            }
        }

        // Nayi music clip assign karna
       musicSource.clip = newMusicClip;


        // Background music repeat hoti rahe
        musicSource.loop = true;

        // Fade-in zero volume se start karna
        musicSource.volume = 0f;

        // Nayi music play karna
        musicSource.Play();

        // Music ko zero se saved volume tak fade-in karna
        while (musicSource.volume < targetVolume)
        {
            musicSource.volume = Mathf.MoveTowards(
                musicSource.volume,
                targetVolume,
                musicFadeSpeed * Time.unscaledDeltaTime
            );

            // Next frame tak wait karna
            yield return null;
        }

        // Exact target volume ensure karna
        musicSource.volume = targetVolume;

        // Transition complete ho gayi
        musicTransition = null;
    }

    // Background music temporary pause karna
    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    // Paused music ko wahi position se continue karna
    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    // Background music ko completely stop karna
    public void StopMusic()
    {
        if (musicSource == null)
        {
            return;
        }

        // Agar transition coroutine chal rahi ho to stop karna
        if (musicTransition != null)
        {
            StopCoroutine(musicTransition);
            musicTransition = null;
        }

        musicSource.Stop();
    }

    // SFX Slider se volume change karna
    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null)
        {
            return;
        }

        // Slider value ko safe 0–1 range mein rakhna
        float safeVolume = Mathf.Clamp01(volume);

        // SFX volume apply karna
        sfxSource.volume = safeVolume;

        // Volume save karna
        PlayerPrefs.SetFloat(
            "SFXVolume",
            safeVolume
        );

        PlayerPrefs.Save();
    }

    // Music Slider se volume change karna
    public void SetMusicVolume(float volume)
    {
        if (musicSource == null)
        {
            return;
        }

        // Slider ki value 0 se 1 ke andar rakhna
        float musicVolume = Mathf.Clamp01(volume);

        // Slider 100% ho tab bhi actual music maximum 60% hogi
        musicSource.volume =
            musicVolume * maxMusicVolumeLimit;

        // PlayerPrefs mein slider ki original value save karna
        PlayerPrefs.SetFloat(
            "MusicVolume",
            musicVolume
        );

        PlayerPrefs.Save();
    }

    // SFX mute/unmute toggle karna
    public void ToggleSFXMute()
    {
        if (sfxSource == null)
        {
            return;
        }

        // Current mute state ko ulta karna
        sfxSource.mute = !sfxSource.mute;

        // true ko 1 aur false ko 0 save karna
        PlayerPrefs.SetInt(
            "SFXMuted",
            sfxSource.mute ? 1 : 0
        );

        PlayerPrefs.Save();
    }

    // Music mute/unmute toggle karna
    public void ToggleMusicMute()
    {
        if (musicSource == null)
        {
            return;
        }

        // Current music mute state ko ulta karna
        musicSource.mute = !musicSource.mute;

        // Music ki apni mute state save karna
        PlayerPrefs.SetInt(
            "MusicMuted",
            musicSource.mute ? 1 : 0
        );

        PlayerPrefs.Save();
    }
}