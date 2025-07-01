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
        // ��Ŵ��Ҩҡ PlayerPrefs ���͡�˹��������
        float savedVolume = PlayerPrefs.GetFloat(volumeKey, 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        // ����� Event
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // ��͹ Panel �͹�����
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
