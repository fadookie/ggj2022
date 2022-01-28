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
        continueButton.onClick.AddListener(() => gameObject.SetActive(false));
        newGameButton.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));
        optionsButton.onClick.AddListener(() => options.SetActive(true));
        instructionsButton.onClick.AddListener(() => instructions.SetActive(true));
        quitButton.onClick.AddListener(() => Application.Quit());
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
