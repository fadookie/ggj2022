using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    [SerializeField] private Button doneButton;
    [SerializeField] private VolumeControl volumeControlTemplate;


    private List<VolumeControl> volumeControls = new List<VolumeControl>();

    protected IEnumerator Start()
    {
        doneButton.onClick.AddListener(() => gameObject.SetActive(false));
        volumeControlTemplate.transform.parent.gameObject.SetActive(false);
        yield return null;
        volumeControlTemplate.transform.parent.gameObject.SetActive(true);
    }

    protected void OnEnable()
    {
        var volumeParameters = AudioManager.Instance.VolumeParameters;
        volumeControlTemplate.gameObject.SetActive(false);
        while (volumeControls.Count < volumeParameters.Length)
        {
            volumeControls.Add(Instantiate(volumeControlTemplate, volumeControlTemplate.transform.parent));
        }
        for(int i = 0; i< volumeParameters.Length; i++)
        {
            volumeControls[i].Setup(volumeParameters[i]);
            volumeControls[i].gameObject.SetActive(true);
        }
        for (int i = volumeParameters.Length; i < volumeControls.Count; i++)
        {
            volumeControls[i].gameObject.SetActive(false);
        }
    }
}
