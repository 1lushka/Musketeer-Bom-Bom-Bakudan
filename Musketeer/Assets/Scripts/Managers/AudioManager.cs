using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    [Header("Музыка")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("SFX")]
    [SerializeField] private AudioClip[] sfxClips;

    // все звуки в enum
    public enum SFX
    {
        ButtonClick,
        Shoot,
        EnemyDie,
        BossStun,
        SpawnEnemy,
        WaveStart,
        WaveComplete,
        Victory,
        GameOver,
    }

    private Dictionary<SFX, AudioClip> sfxDictionary;
    private AudioSource musicSource;
    private List<AudioSource> sfxSourcePool = new List<AudioSource>();
    private int poolSize = 10;

    [Range(0f, 1f)]
    public float masterVolume;
    [Range(0f, 1f)]
    public float musicVolume;
    [Range(0f, 1f)]
    public float sfxVolume;

    private void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        InitializeSFXDictionary();
        SetupAudioSources();
        SubscribeToGameEvents();
    }

    private void InitializeSFXDictionary()
    {
        sfxDictionary = new Dictionary<SFX, AudioClip>();
        int enumCount = System.Enum.GetNames(typeof(SFX)).Length;


        for (int i = 0; i < enumCount; i++)
        {
            SFX sound = (SFX)i;
            AudioClip clip = i < sfxClips.Length ? sfxClips[i] : null;
            sfxDictionary[sound] = clip;

            if (clip == null)
                Debug.LogWarning($"[AudioManager] Нет звука для SFX: {sound}");
        }
    }

    private void SetupAudioSources()
    {
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.outputAudioMixerGroup = musicMixerGroup;

       
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = sfxMixerGroup;
            sfxSourcePool.Add(source);
        }
    }

    private void SubscribeToGameEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameWin += () => PlaySFX(SFX.Victory);
            GameManager.Instance.OnGameOver += () => PlaySFX(SFX.GameOver);
            GameManager.Instance.OnWaveStarted += (wave) => PlaySFX(SFX.WaveStart);
            GameManager.Instance.OnWaveCompleted += (wave) => PlaySFX(SFX.WaveComplete);

            
            GameManager.Instance.OnGamePaused += () => musicSource.Pause();
            GameManager.Instance.OnGameResumed += () => musicSource.UnPause();
        }
    }

    private void Update()
    {
       
        if (GameManager.Instance == null) return;

        AudioClip targetMusic = GameManager.Instance.CurrentState == GameManager.GameState.MainMenu
            ? menuMusic : gameMusic;

        if (musicSource.clip != targetMusic)
        {
            PlayMusic(targetMusic, true);
        }
    }

    public void PlaySFX(SFX sfx, float volumeScale = 1f)
    {
        if (!sfxDictionary.TryGetValue(sfx, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"[AudioManager] SFX не найден: {sfx}");
            return;
        }

        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.clip = clip;
            source.volume = sfxVolume * masterVolume * volumeScale;
            source.pitch = 1f; // можно потом рандомить в небольшом дипазоне
            source.Play();
        }
    }

    public void PlayMusic(AudioClip music, bool fade = false, float fadeDuration = 1f)
    {
        if (music == null || musicSource.clip == music) return;

        if (fade)
        {
            StartCoroutine(FadeMusicAndSwitch(music, fadeDuration));
        }
        else
        {
            musicSource.Stop();
            musicSource.clip = music;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }
    }

    private IEnumerator FadeMusicAndSwitch(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;

       
        float timer = 0;
        while (timer < duration / 2)
        {
            timer += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / (duration / 2));
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        
        timer = 0;
        while (timer < duration / 2)
        {
            timer += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0, musicVolume * masterVolume, timer / (duration / 2));
            yield return null;
        }

        musicSource.volume = musicVolume * masterVolume;
    }

    private AudioSource GetAvailableSFXSource()
    {
        
        foreach (AudioSource source in sfxSourcePool)
        {
            if (!source.isPlaying)
                return source;
        }

        
        AudioSource oldest = sfxSourcePool[0];
        oldest.Stop();
        return oldest;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume * masterVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    private void UpdateVolumes()
    {
        musicSource.volume = musicVolume * masterVolume;
       
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic() => musicSource.Pause();
    public void UnPauseMusic() => musicSource.UnPause();

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}