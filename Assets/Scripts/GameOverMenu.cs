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
        newGameButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
            AudioManager.Instance.PlayClick();
        });
        mainMenuButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Main Menu");
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
