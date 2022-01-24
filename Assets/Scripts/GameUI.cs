using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public PauseMenu PauseMenu => pauseMenu;
    [SerializeField] private PauseMenu pauseMenu;

    public GameOverMenu GameOverMenu => gameOverMenu;
    [SerializeField] private GameOverMenu gameOverMenu;

    public Component Instructions => instructions.transform;
    [SerializeField] private GameObject instructions;
    public Image DeathFade => deathFade;
    [SerializeField] private Image deathFade;

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Instructions.gameObject.activeSelf)
            {
                Instructions.gameObject.SetActive(false);
            }
            else if (GameOverMenu.gameObject.activeSelf)
            {
            }
            else
            {
                PauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
            }
        }
    }
}
