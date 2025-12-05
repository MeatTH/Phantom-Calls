using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject soundPanel;
    //public Slider volumeSlider;
    //public Slider masterVolumeSlider;

    [Header("Volume Sliders")]
    public Slider masterVolumeSlider; // ใช้ควบคุม AudioListener หรือ Master Mixer
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider voVolumeSlider;

    private const string volumeKey = "GameVolume";

    void Start()
    {
        // โหลดค่าจาก PlayerPrefs หรือกำหนดเริ่มต้น
        //float savedVolume = PlayerPrefs.GetFloat(volumeKey, 1f);
        //volumeSlider.value = savedVolume;
        //AudioListener.volume = savedVolume;
        float savedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        masterVolumeSlider.value = savedMasterVolume;
        AudioListener.volume = savedMasterVolume;
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        // เชื่อม Event
        //volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        LoadAndConnectVolume(bgmVolumeSlider, "BGMVolume", 0.5f, OnBGMVolumeChanged);
        LoadAndConnectVolume(sfxVolumeSlider, "SFXVolume", 0.8f, OnSFXVolumeChanged);
        LoadAndConnectVolume(voVolumeSlider, "VOVolume", 1.0f, OnVOVolumeChanged);

        // ซ่อน Panel ตอนเริ่ม
        soundPanel.SetActive(false);
    }

    public void ToggleSoundPanel()
    {
        soundPanel.SetActive(!soundPanel.activeSelf);
    }

    /*public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(volumeKey, value);
    }*/
    private void LoadAndConnectVolume(Slider slider, string key, float defaultVal, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null) return;
        float savedVolume = PlayerPrefs.GetFloat(key, defaultVal);
        slider.value = savedVolume;

        // เรียก callback ครั้งแรกเพื่อตั้งค่า AudioSource.volume ทันที
        callback(savedVolume);

        slider.onValueChanged.AddListener(callback);
    }
    public void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        if (SoundManager_Test1.instance != null)
            SoundManager_Test1.instance.SetVolume(SoundManager_Test1.SoundType.BGM, value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (SoundManager_Test1.instance != null)
            SoundManager_Test1.instance.SetVolume(SoundManager_Test1.SoundType.SFX, value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void OnVOVolumeChanged(float value)
    {
        if (SoundManager_Test1.instance != null)
            SoundManager_Test1.instance.SetVolume(SoundManager_Test1.SoundType.VO, value);
        PlayerPrefs.SetFloat("VOVolume", value);
    }
}
