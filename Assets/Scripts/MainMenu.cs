using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;


    [SerializeField] private GameObject instructions;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject credits;

    protected void Start()
    {
        startButton.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        instructionsButton.onClick.AddListener(() => instructions.SetActive(true));
        optionsButton.onClick.AddListener(() => options.SetActive(true));
        creditsButton.onClick.AddListener(() => credits.SetActive(true));
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    protected void OnEnable()
    {
        AudioManager.Instance.SetMusicPitch(1, 1);
    }
}
