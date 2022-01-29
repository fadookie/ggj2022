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

    private IEnumerator musicControlRoutine; 

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
        if(musicControlRoutine != null)
        {
            if(!musicControlRoutine.MoveNext())
            {
                musicControlRoutine = null;
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

    public void DeathMusic(float overPeriod)
    {
        musicControlRoutine = DeathMusicRoutine(overPeriod);
    }

    private IEnumerator DeathMusicRoutine(float overPeriod)
    {
        float initialPitch = musicPlayer.pitch;
        float initialVolume = musicPlayer.volume;

        float startTime = Time.unscaledTime;
        while (Time.unscaledTime < startTime + overPeriod)
        {
            float lerp = (Time.unscaledTime - startTime) / overPeriod;
            musicPlayer.pitch = Mathf.Lerp(initialPitch, 0, lerp);
            musicPlayer.volume = Mathf.Lerp(initialVolume, 0, lerp);
            yield return null;
        }

        musicPlayer.volume = 0;
        musicPlayer.pitch = 0;

        var sub = NormalMusicRoutine();
        while (sub.MoveNext())
        {
            yield return sub.Current;
        }
    }

    public void PlayerMusic(Player player)
    {
        musicControlRoutine = PlayerMusicRoutine(player);
    }

    private IEnumerator PlayerMusicRoutine(Player player)
    {
        float initialPitch = musicPlayer.pitch;
        float initialVolume = musicPlayer.volume;
        float pitchVelo = 0;
        float volumeVelo = 0;
        while (true)
        {
            float pitchGoal = 1 + Mathf.Pow(player.ElapsedPossessionTime / player.PossessionTimerDurationSec, 2) / 2f;
            musicPlayer.pitch = Mathf.SmoothDamp(musicPlayer.pitch, pitchGoal, ref pitchVelo, .5f, 9999, Time.unscaledDeltaTime);

            musicPlayer.volume = Mathf.SmoothDamp(musicPlayer.volume, 1, ref volumeVelo, 1, 9999, Time.unscaledDeltaTime);

            yield return null;
        }
    }

    public void NormalMusic()
    {
        musicControlRoutine = NormalMusicRoutine();
    }

    private IEnumerator NormalMusicRoutine()
    {
        if(musicPlayer.volume == 0)
        {
            musicPlayer.pitch = 1;
        }
        float pitchVelo = 0;
        float volumeVelo = 0;
        while (true)
        {
            musicPlayer.pitch = Mathf.SmoothDamp(musicPlayer.pitch, 1, ref pitchVelo, 1, 9999, Time.unscaledDeltaTime);

            musicPlayer.volume = Mathf.SmoothDamp(musicPlayer.volume, 1, ref volumeVelo, 1, 9999, Time.unscaledDeltaTime);

            yield return null;
        }
    }
}