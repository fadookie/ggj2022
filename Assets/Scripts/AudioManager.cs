using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    public string[] VolumeParameters => volumeParameters;
    public AudioMixer AudioMixer => audioMixer;

    [SerializeField] private AudioClip Click;
    [SerializeField] private AudioClip Possession;
    [SerializeField] private AudioClip TakeDamage;
    [SerializeField] private AudioClip Death;
    [SerializeField] private AudioClip NPCAggro;
    [SerializeField] private AudioClip NPCDeAggro;
    
    [SerializeField] private AudioClip[] Music;

    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioSource musicPlayer;

    [SerializeField] private AudioMixer audioMixer;


    [SerializeField] private string[] volumeParameters;

    private Queue<AudioClip> MusicQueue;
    public bool PlayMusic { get; set; }

    public enum Sound
    {
        Click,
        Possession,
        TakeDamage,
        Death,
        NPCAggro,
        NPCDeAggro,
    }

    void Awake()
    {
        // Singleton setup;
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        instance = this;

        ShuffleMusicQueue();
        PlayMusic = true;

        foreach (string volumeParameter in volumeParameters)
        {
            float volume; ;
            if (PlayerPrefs.HasKey(volumeParameter))
            {
                volume = PlayerPrefs.GetFloat(volumeParameter);
                audioMixer.SetFloat(volumeParameter, volume);
            }
            else
            {
                audioMixer.GetFloat(volumeParameter, out volume);
                PlayerPrefs.SetFloat(volumeParameter, volume);
            }
        }
    }

    private void Update() {
        if (PlayMusic && !musicPlayer.isPlaying) {
            try {
                var newClip = MusicQueue.Dequeue();
                PlayMusicClip(newClip);
            } catch (InvalidOperationException) {
                ShuffleMusicQueue();
            }
        }
    }

    void ShuffleMusicQueue() {
        var musicListShuffled = Music.ToList();
        musicListShuffled.Shuffle();
        MusicQueue = new Queue<AudioClip>(musicListShuffled);
    }

    public void PlayClick() {
        PlaySound(Sound.Click);
    }

    public void PlaySound(Sound sound)
    {
        PlayClip(GetAudioClip(sound));
    }

    public AudioClip GetAudioClip(Sound sound) {
        switch (sound)
        {
            case Sound.Click:
                return Click;
            case Sound.Possession:
                return Possession;
            case Sound.TakeDamage:
                return TakeDamage;
            case Sound.Death:
                return Death;
            case Sound.NPCAggro:
                return NPCAggro;
            case Sound.NPCDeAggro:
                return NPCDeAggro;
            default:
                throw new ArgumentOutOfRangeException(nameof(sound), sound, null);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        sfxPlayer.PlayOneShot(clip);
    }
    
    private void PlayMusicClip(AudioClip clip)
    {
        musicPlayer.PlayOneShot(clip);
    }
}