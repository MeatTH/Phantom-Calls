using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickSound; // ลากไฟล์เสียงใส่จาก Inspector
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        // 1. ลองใช้ SoundManager ก่อน (เพื่อให้ดังตาม Volume ที่ตั้ง และลดปัญหาเสียงหายตอนเปลี่ยนฉาก)
        if (SoundManager_Test1.instance != null && clickSound != null)
        {
            SoundManager_Test1.instance.GetSFXPlayer().PlayOneShot(clickSound);
        }
        else if (clickSound != null)
        {
            // 2. Fallback: ถ้าไม่มี SoundManager ใช้ PlayClipAtPoint 
            // วิธีนี้จะสร้าง AudioSource ชั่วคราว ทำให้ไม่ติด Error "disabled source"
            float volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position, volume);
        }
        // 3. Fallback สุดท้าย: ใช้ AudioSource ตัวเอง (แต่เพิ่มการเช็คเพื่อกัน Crash)
        else if (audioSource != null && audioSource.isActiveAndEnabled && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPlayButtonClicked()
    {
        PlayClickSound();
        SceneManager.LoadScene("Selectstory");
    }

    public void OnExitButtonClicked()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void OnBackToMainButtonClicked()
    {
        PlayClickSound();
        SceneManager.LoadScene("Mainmenu");
    }
}