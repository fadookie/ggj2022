using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{

    [SerializeField] private Slider slider;
    private AudioMixerGroup audioMixerGroup;

    public void Setup(AudioMixerGroup audioMixerGroup)
    {
        this.audioMixerGroup = audioMixerGroup;
        //slider.value = audioMixerGroup.
    }

    protected void Update()
    {
        
    }

}
