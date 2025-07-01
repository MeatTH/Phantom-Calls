using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject soundPanel;
    public Slider volumeSlider;

    private const string volumeKey = "GameVolume";

    void Start()
    {
        // โหลดค่าจาก PlayerPrefs หรือกำหนดเริ่มต้น
        float savedVolume = PlayerPrefs.GetFloat(volumeKey, 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        // เชื่อม Event
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // ซ่อน Panel ตอนเริ่ม
        soundPanel.SetActive(false);
    }

    public void ToggleSoundPanel()
    {
        soundPanel.SetActive(!soundPanel.activeSelf);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(volumeKey, value);
    }
}
