using UnityEngine;

public class PanelAudioLink : MonoBehaviour
{
    // เลือกประเภทเสียงที่ต้องการให้ AudioSource นี้ตาม
    public SoundManager_Test1.SoundType audioType = SoundManager_Test1.SoundType.SFX;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("PanelAudioLink: ต้องมี AudioSource บน GameObject เดียวกัน");
    }

    private void OnEnable()
    {
        if (audioSource != null && SoundManager_Test1.instance != null)
        {
            // 1. ลงทะเบียน AudioSource นี้กับ SoundManager
            SoundManager_Test1.instance.RegisterExternalAudio(
                audioSource,
                audioType
            );
        }
    }

    private void OnDisable()
    {
        if (audioSource != null && SoundManager_Test1.instance != null)
        {
            // 2. ยกเลิกการลงทะเบียน
            SoundManager_Test1.instance.UnregisterExternalAudio(audioSource);
        }
    }
}