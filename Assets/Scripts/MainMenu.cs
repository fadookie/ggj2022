using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;


    [SerializeField] private GameObject instructions;
    [SerializeField] private GameObject credits;

    protected void Start()
    {
        startButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
            AudioManager.Instance.PlayClick();
        });
        instructionsButton.onClick.AddListener(() => {
            instructions.SetActive(true);
            AudioManager.Instance.PlayClick();
        });
        creditsButton.onClick.AddListener(() => {
            credits.SetActive(true);
            AudioManager.Instance.PlayClick();
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
            AudioManager.Instance.PlayClick();
        });
    }
}
