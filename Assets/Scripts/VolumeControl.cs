using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    private string parameter;

    public void Setup(string parameter)
    {
        slider.minValue = -80;
        slider.maxValue = 0;

        this.parameter = parameter;

        AudioManager.Instance.AudioMixer.GetFloat(parameter, out float volume);
        slider.value = volume;
        text.text = parameter;
    }

    protected void Update()
    {
        AudioManager.Instance.AudioMixer.SetFloat(parameter, slider.value);
        PlayerPrefs.SetFloat(parameter, slider.value);
    }
}
