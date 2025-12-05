using System.Collections.Generic;
using UnityEngine;

public class SoundManager_Test1 : MonoBehaviour
{
    public static SoundManager_Test1 instance;
    public enum SoundType { BGM, SFX, VO }

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

    [Header("Voice Over Clips")]
    public List<NamedClip> VoiceOverClips;

    private Dictionary<string, AudioClip> bgmDict;
    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> voDict;
    private List<(AudioSource source, SoundType type)> externalSources = new List<(AudioSource, SoundType)>();

    private AudioSource bgmPlayer;
    private AudioSource sfxPlayer;
    private AudioSource voPlayer;

    public AudioSource GetBGMPlayer() { return bgmPlayer; }
    public AudioSource GetSFXPlayer() { return sfxPlayer; }
    public AudioSource GetVOPlayer() { return voPlayer; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;

        sfxPlayer = gameObject.AddComponent<AudioSource>();

        voPlayer = gameObject.AddComponent<AudioSource>();

        bgmDict = new Dictionary<string, AudioClip>();
        foreach (var clip in bgmClips)
            bgmDict[clip.name] = clip.clip;

        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var clip in sfxClips)
            sfxDict[clip.name] = clip.clip;

        voDict = new Dictionary<string, AudioClip>();
        foreach (var clip in VoiceOverClips)
            voDict[clip.name] = clip.clip;
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
        if (tag.StartsWith("VOICE:"))
        {
            string name = tag.Substring("VOICE:".Length).Trim();
            PlayVoiceClipByName(name);
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
    public void PlayVoiceClipByName(string name)
    {
        if (voDict.TryGetValue(name, out AudioClip clip))
        {
            // ถ้า VO Player กำลังเล่นอยู่ ให้หยุดก่อน (ป้องกันเสียงซ้อนแบบไม่ตั้งใจ)
            if (voPlayer.isPlaying)
            {
                voPlayer.Stop();
            }
            voPlayer.clip = clip;
            voPlayer.Play();
        }
        else
        {
            Debug.LogWarning("❌ ไม่พบ Voice Clip: " + name);
        }
    }
    public void RegisterExternalAudio(AudioSource source, SoundType type)
    {
        // 1. หา Volume ที่ถูกตั้งค่าไว้ปัจจุบันจาก AudioSource หลัก
        float currentSystemVolume = GetVolumeByType(type);

        // 2. ตั้งค่า Volume ของแหล่งเสียงภายนอกให้ตรงกับ Volume ที่ระบบกำลังใช้
        source.volume = currentSystemVolume;

        externalSources.Add((source, type));
    }
    public void UnregisterExternalAudio(AudioSource source)
    {
        externalSources.RemoveAll(item => item.source == source);
    }
    private float GetVolumeByType(SoundType type)
    {
        if (type == SoundType.BGM) return bgmPlayer.volume;
        if (type == SoundType.SFX) return sfxPlayer.volume;
        if (type == SoundType.VO) return voPlayer.volume;
        return 1f;
    }
    public void SetVolume(SoundType type, float volumeValue)
    {
        // 1. อัปเดต AudioSource หลัก
        AudioSource player = null;
        if (type == SoundType.BGM) player = bgmPlayer;
        else if (type == SoundType.SFX) player = sfxPlayer;
        else if (type == SoundType.VO) player = voPlayer;

        if (player != null) player.volume = volumeValue;

        // 2. อัปเดต AudioSource ภายนอกทั้งหมดที่ลงทะเบียนไว้
        foreach (var item in externalSources)
        {
            if (item.type == type)
            {
                // อัปเดต volume ของแหล่งเสียงภายนอก
                // (เราสมมติว่า volumeValue คือค่า Final Volume ที่ต้องการ)
                item.source.volume = volumeValue;
            }
        }
    }
}
