using UnityEngine;

public class PauseDetector : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        }
    }
}
