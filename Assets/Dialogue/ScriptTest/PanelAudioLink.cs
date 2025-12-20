using UnityEngine;

public class PanelAudioLink : MonoBehaviour
{
    // เลือกประเภทเสียงที่จะลิงก์ (BGM หรือ SFX)
    public SoundManager_Test1.SoundType audioType = SoundManager_Test1.SoundType.SFX;

    private AudioSource audioSource;
    private bool isRegistered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("PanelAudioLink: ต้องมี AudioSource บน GameObject เดียวกัน");
    }

    private void Start()
    {
        // เรียกใน Start จะชัวร์กว่า Awake เพราะรอให้ SoundManager สร้าง Instance เสร็จก่อน
        RegisterWithManager();
    }

    private void OnEnable()
    {
        // กรณี Object ถูก Disable แล้ว Enable กลับมาใหม่
        RegisterWithManager();
    }

    private void OnDisable()
    {
        if (SoundManager_Test1.instance != null && audioSource != null)
        {
            SoundManager_Test1.instance.UnregisterExternalAudio(audioSource);
            isRegistered = false;
        }
    }

    private void RegisterWithManager()
    {
        if (isRegistered) return; // ถ้าลงทะเบียนแล้วไม่ต้องทำซ้ำ

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (SoundManager_Test1.instance != null)
        {
            SoundManager_Test1.instance.RegisterExternalAudio(audioSource, audioType);
            isRegistered = true;
        }
        else
        {
            // Fallback: ถ้ายังหา SoundManager ไม่เจอ (อาจจะอยู่คนละ Scene หรือ Initialize ไม่ทัน)
            // ให้พยายามดึงค่าจาก PlayerPrefs มาใช้แก้ขัดไปก่อน
            FallbackSetVolume();
        }
    }

    private void FallbackSetVolume()
    {
        string key = "SFXVolume"; // Default
        float defaultVal = 1f;

        if (audioType == SoundManager_Test1.SoundType.BGM)
        {
            key = "BGMVolume";
            defaultVal = 0.5f;
        }
        else if (audioType == SoundManager_Test1.SoundType.VO)
        {
            key = "VOVolume";
        }

        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat(key, defaultVal);
        }
    }
}