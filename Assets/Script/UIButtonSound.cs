using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip clickSound; // ลากไฟล์เสียงที่ต้องการมาใส่ตรงนี้
    public string sfxName; // หรือใส่ชื่อ SFX ที่มีใน SoundManager

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
            // 1. ถ้ามีคลิปเสียงใส่มาในช่อง clickSound ให้เล่นไฟล์นั้น
        if (clickSound != null)
        {
            // สร้าง GameObject ชั่วคราวสำหรับเล่นเสียง (แบบข้าม Scene ได้)
            GameObject soundObj = new GameObject("TempButtonSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            
            source.clip = clickSound;
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            source.volume = sfxVolume;
            source.Play();

            // สำคัญ! ทำให้ Object นี้ไม่ถูกทำลายเมื่อเปลี่ยน Scene
            DontDestroyOnLoad(soundObj);

            // สั่งทำลายตัวเองเมื่อเล่นจบ
            //Destroy(soundObj, clickSound.length);
        }
        // 2. ถ้าไม่มีคลิป แต่มีชื่อ sfxName ให้เรียกผ่าน SoundManager
        else if (!string.IsNullOrEmpty(sfxName) && SoundManager_Test1.instance != null)
        {
            SoundManager_Test1.instance.PlaySFX(sfxName);
        }
    }
}
