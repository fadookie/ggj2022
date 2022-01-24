using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button instructionsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject instructions;

    protected void Start()
    {
        newGameButton.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));
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
