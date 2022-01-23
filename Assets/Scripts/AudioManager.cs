using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    public AudioClip Possession;
    public AudioClip TakeDamage;
    public AudioClip Death;

    private AudioSource player;

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

        player = GetComponent<AudioSource>();
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
        player.PlayOneShot(clip);
    }
}