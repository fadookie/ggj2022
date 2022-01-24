using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject instructions;
    [SerializeField] private GameObject options;

    protected void Start()
    {
        continueButton.onClick.AddListener(() => {
            gameObject.SetActive(false);
            AudioManager.Instance.PlayClick();
        });
        newGameButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
            AudioManager.Instance.PlayClick();
        });
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Main Menu");
            AudioManager.Instance.PlayClick();
        });
        optionsButton.onClick.AddListener(() => {
            options.SetActive(true);
            AudioManager.Instance.PlayClick();
        });
        instructionsButton.onClick.AddListener(() => {
            instructions.SetActive(true);
            AudioManager.Instance.PlayClick();
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
            AudioManager.Instance.PlayClick();
        });
    }

    protected void OnEnable()
    {

        Time.timeScale = 0;
    }

    protected void OnDisable()
    {
        Time.timeScale = 1;
    }
}
