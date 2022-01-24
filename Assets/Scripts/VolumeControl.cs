using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    private AudioMixer audioMixer;
    private string parameter;

    public void Setup(AudioMixer audioMixer, string parameter)
    {
        this.audioMixer = audioMixer;
        this.parameter = parameter;

        audioMixer.GetFloat(parameter, out float volume);
        slider.value = volume;
        text.text = parameter;
    }

    protected void Update()
    {
        audioMixer.SetFloat(parameter, slider.value);
    }
}
