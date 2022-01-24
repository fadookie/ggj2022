using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Button doneButton;
    [SerializeField] private VolumeControl volumeControlTemplate;

    [SerializeField]
    private string[] volumeParameters;

    private List<VolumeControl> volumeControls = new List<VolumeControl>();

    protected void Start()
    {
        volumeControlTemplate.gameObject.SetActive(false);
        doneButton.onClick.AddListener(() => gameObject.SetActive(false));

    }

    protected void OnEnable()
    {
        while(volumeControls.Count < volumeParameters.Length)
        {
            volumeControls.Add(Instantiate(volumeControlTemplate, volumeControlTemplate.transform.parent));
        }
        for(int i = 0; i< volumeParameters.Length; i++)
        {
            volumeControls[i].Setup(audioMixer, volumeParameters[i]);
            volumeControls[i].gameObject.SetActive(true);
        }
        for (int i = volumeParameters.Length; i < volumeControls.Count; i++)
        {
            volumeControls[i].gameObject.SetActive(false);
        }

        Time.timeScale = 0;
    }

    protected void OnDisable()
    {
        Time.timeScale = 1;
    }
}
