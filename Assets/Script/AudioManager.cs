using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip menuMusic;
    public string[] scenesToStopMusic; // รายชื่อ scene ที่จะหยุดเพลง

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        audioSource.Play();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. เช็คว่า Scene นี้ต้องหยุดเพลงหรือไม่
        bool shouldStop = false;
        foreach (string sceneName in scenesToStopMusic)
        {
            if (scene.name == sceneName)
            {
                shouldStop = true;
                break;
            }
        }

        // 2. สั่งการทำงาน
        if (shouldStop)
        {
            // ถ้าอยู่ในรายชื่อ Scene ที่ต้องหยุด -> หยุดเพลง
            StopMusic();
        }
        else
        {
            // ✅ ถ้าไม่อยู่ในรายชื่อ (เช่น กลับมา Main Menu) -> ให้เล่นเพลง
            // เช็คก่อนว่าเพลงเล่นอยู่ไหม เพื่อกันเสียงสะดุดถ้ามันเล่นอยู่แล้ว
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.clip = menuMusic; // ใส่คลิปเพลงเมนู
                audioSource.Play();           // สั่งเล่น
            }
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SwitchMusic(AudioClip newClip)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
