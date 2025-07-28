using System.Collections.Generic;
using UnityEngine;

public class SoundManager_Test1 : MonoBehaviour
{
    public static SoundManager_Test1 instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip bgmThisScene;
    public AudioClip clickSound;
    [System.Serializable]
    public class NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }

    public List<NamedAudioClip> bgmClips;
    public List<NamedAudioClip> sfxClips;

    private Dictionary<string, AudioClip> bgmDict;
    private Dictionary<string, AudioClip> sfxDict;


    void Start()
    {
        if (bgmThisScene != null && !bgmSource.isPlaying)
            SoundManager_Test1.instance.PlayBGM(bgmThisScene);
    }
    public void OnButtonClick()
    {
        SoundManager_Test1.instance.PlaySFX(clickSound);

    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            bgmDict = new Dictionary<string, AudioClip>();
            foreach (var item in bgmClips)
                if (!bgmDict.ContainsKey(item.name)) bgmDict.Add(item.name, item.clip);

            sfxDict = new Dictionary<string, AudioClip>();
            foreach (var item in sfxClips)
                if (!sfxDict.ContainsKey(item.name)) sfxDict.Add(item.name, item.clip);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void HandleSoundTag(string tag)
    {
        if (string.IsNullOrEmpty(tag)) return;

        if (tag.StartsWith("play_bgm:"))
        {
            string bgmName = tag.Substring("play_bgm:".Length).Trim();
            if (bgmDict.ContainsKey(bgmName))
            {
                bgmSource.clip = bgmDict[bgmName];
                bgmSource.Play();
            }
        }
        else if (tag.StartsWith("play_sfx:"))
        {
            string sfxName = tag.Substring("play_sfx:".Length).Trim();
            if (sfxDict.ContainsKey(sfxName))
            {
                sfxSource.PlayOneShot(sfxDict[sfxName]);
            }
        }
    }

}