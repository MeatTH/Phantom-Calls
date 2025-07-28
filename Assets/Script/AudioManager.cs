using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    public string gameSceneName = "Gameplay"; // หรือเปลี่ยนเป็นชื่อ Scene ที่คุณใช้จริง

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
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;

        PlayMenuMusic();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName)
        {
            // ถ้าเป็นฉากเกม → เล่นเพลงเกม
            SwitchMusic(gameMusic);
        }
        else
        {
            // ถ้าไม่ใช่ → กลับไปเล่นเพลงเมนู
            SwitchMusic(menuMusic);
        }
    }

    public void PlayMenuMusic()
    {
        SwitchMusic(menuMusic);
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
        if (audioSource == null || newClip == null) return;

        if (audioSource.clip == newClip && audioSource.isPlaying)
            return;

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();
    }
}
