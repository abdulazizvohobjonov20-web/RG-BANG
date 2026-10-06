using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized audio management for RG BANG
/// Handles music, SFX, and voice lines
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();
    private float masterVolume = GameConstants.MASTER_VOLUME;
    private float musicVolume = GameConstants.MUSIC_VOLUME;
    private float sfxVolume = GameConstants.SFX_VOLUME;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (voiceSource == null) voiceSource = gameObject.AddComponent<AudioSource>();

        ApplyVolume();
    }

    public void PlayMusic(string clipName, bool loop = true, float fadeIn = 0.5f)
    {
        AudioClip clip = LoadAudioClip($"Music/{clipName}");
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlaySFX(string clipName, float pitch = 1f, float volume = 1f)
    {
        AudioClip clip = LoadAudioClip($"SFX/{clipName}");
        if (clip == null) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

    public void PlayAbilitySound(string abilityName)
    {
        PlaySFX($"Abilities/{abilityName}", 1f, 1f);
    }

    public void PlayCharacterVoice(string characterId, string voiceType)
    {
        AudioClip clip = LoadAudioClip($"Voices/{characterId}_{voiceType}");
        if (clip == null) return;

        voiceSource.PlayOneShot(clip);
    }

    private AudioClip LoadAudioClip(string path)
    {
        if (soundLibrary.ContainsKey(path))
            return soundLibrary[path];

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
            soundLibrary[path] = clip;
        else
            Debug.LogWarning($"Audio clip not found: {path}");

        return clip;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        musicSource.volume = masterVolume * musicVolume;
        sfxSource.volume = masterVolume * sfxVolume;
        voiceSource.volume = masterVolume * sfxVolume;
    }

    public void StopMusic(float fadeOut = 0.5f)
    {
        musicSource.Stop();
    }

    public void StopAllAudio()
    {
        musicSource.Stop();
        sfxSource.Stop();
        voiceSource.Stop();
    }
}
