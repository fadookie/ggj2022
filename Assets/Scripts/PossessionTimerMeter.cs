using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PossessionTimerMeter : MonoBehaviour
{
    private Image image;
    // Start is called before the first frame update
    void Start() {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        var percentTimeElapsed = 1 - Player.Instance.ElapsedPossessionTime / Player.Instance.PossessionTimerDurationSec;
        image.fillAmount = percentTimeElapsed;
    }
}
