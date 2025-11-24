using System.Collections.Generic;
using UnityEngine;

public class SoundManager_Test1 : MonoBehaviour
{
    public static SoundManager_Test1 instance;

    [System.Serializable]
    public class NamedClip
    {
        public string name;
        public AudioClip clip;
    }

    [Header("BGM Clips")]
    public List<NamedClip> bgmClips;

    [Header("SFX Clips")]
    public List<NamedClip> sfxClips;

    private Dictionary<string, AudioClip> bgmDict;
    private Dictionary<string, AudioClip> sfxDict;

    private AudioSource bgmPlayer;
    private AudioSource sfxPlayer;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;

        sfxPlayer = gameObject.AddComponent<AudioSource>();

        bgmDict = new Dictionary<string, AudioClip>();
        foreach (var clip in bgmClips)
            bgmDict[clip.name] = clip.clip;

        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var clip in sfxClips)
            sfxDict[clip.name] = clip.clip;
    }

    public void HandleSoundTag(string tag)
    {
        if (tag.StartsWith("play_bgm:"))
        {
            string name = tag.Substring("play_bgm:".Length).Trim();
            PlayBGM(name);
        }
        else if (tag == "stop_bgm")
        {
            StopBGM();
        }
        if (tag.StartsWith("play_sound:"))
        {
            string name = tag.Substring("play_sound:".Length).Trim();
            PlaySFX(name);
        }
    }

    private void PlayBGM(string name)
    {
        if (bgmDict.TryGetValue(name, out AudioClip clip))
        {
            if (bgmPlayer.clip != clip)
            {
                bgmPlayer.clip = clip;
                bgmPlayer.Play();
            }
        }
        else
        {
            Debug.LogWarning("❌ ไม่พบ BGM: " + name);
        }
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out AudioClip clip))
        {
            sfxPlayer.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("❌ ไม่พบ SFX: " + name);
        }
    }
}
