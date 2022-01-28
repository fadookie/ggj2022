using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClick : MonoBehaviour
{
    protected void Start()
    {
        GetComponent<Button>().onClick.AddListener(AudioManager.Instance.PlayClick);
    }
}
