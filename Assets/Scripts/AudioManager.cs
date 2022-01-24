using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    [SerializeField] private AudioClip Possession;
    [SerializeField] private AudioClip TakeDamage;
    [SerializeField] private AudioClip Death;
    [SerializeField] private AudioClip[] Music;

    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioSource musicPlayer;

    private Queue<AudioClip> MusicQueue;
    public bool PlayMusic { get; set; }

    public enum Sound
    {
        Possession,
        TakeDamage,
        Death,
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

    public void PlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.Possession:
                PlayClip(Possession);
                break;
            case Sound.TakeDamage:
                PlayClip(TakeDamage);
                break;
            case Sound.Death:
                PlayClip(Death);
                break;
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