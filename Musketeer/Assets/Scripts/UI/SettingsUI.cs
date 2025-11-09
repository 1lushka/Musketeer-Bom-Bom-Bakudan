using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsUI : MonoBehaviour
{
    [Header("Слайдеры")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Тексты")]
    [SerializeField] private TextMeshProUGUI masterText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI sfxText;

    [Header("AudioMixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string MASTER_PARAM = "MasterVol";
    private const string MUSIC_PARAM = "MusicVol";
    private const string SFX_PARAM = "SFXVol";

   

    private void OnEnable()
    {
        
        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        LoadVolumes();
    }

    private void OnDisable()
    {
        
        if (masterSlider) masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        if (musicSlider) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }

    private void LoadVolumes()
    {
        float master = PlayerPrefs.GetFloat("MasterVol", 1f);
        float music = PlayerPrefs.GetFloat("MusicVol", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVol", 1f);

        SetSliderValue(masterSlider, master);
        SetSliderValue(musicSlider, music);
        SetSliderValue(sfxSlider, sfx);

        ApplyToMixer(MASTER_PARAM, master);
        ApplyToMixer(MUSIC_PARAM, music);
        ApplyToMixer(SFX_PARAM, sfx);

        UpdateTexts();
    }

    private void SetSliderValue(Slider slider, float value)
    {
        if (slider) slider.value = value;
    }

    public void SetMasterVolume(float value)
    {
        ApplyToMixer(MASTER_PARAM, value);
        PlayerPrefs.SetFloat("MasterVol", value);
        UpdateTexts();
    }

    public void SetMusicVolume(float value)
    {
        ApplyToMixer(MUSIC_PARAM, value);
        PlayerPrefs.SetFloat("MusicVol", value);
        UpdateTexts();
    }

    public void SetSFXVolume(float value)
    {
        ApplyToMixer(SFX_PARAM, value);
        PlayerPrefs.SetFloat("SFXVol", value);
        UpdateTexts();
    }

    private void ApplyToMixer(string param, float sliderValue)
    {
        float dB = sliderValue > 0.001f ? Mathf.Log10(sliderValue) * 20f : -80f;
        audioMixer.SetFloat(param, dB);
    }

    private void UpdateTexts()
    {
        if (masterText) masterText.text = $"{(masterSlider?.value * 100 ?? 0):0}%";
        if (musicText) musicText.text = $"{(musicSlider?.value * 100 ?? 0):0}%";
        if (sfxText) sfxText.text = $"{(sfxSlider?.value * 100 ?? 0):0}%";
    }

    
    public void PlayTestSound() => AudioManager.Instance?.PlaySFX(AudioManager.SFX.ButtonClick);

   

 
}