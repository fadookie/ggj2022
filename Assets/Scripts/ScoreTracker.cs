using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    private static ScoreTracker instance;

    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    public int Score { get; set; }

    protected void Start()
    {
        instance = this;
    }

    protected void Update()
    {
        scoreText.text = Score.ToString();
    }

    public static bool TryGetInstance(out ScoreTracker scoreTracker)
    {
        scoreTracker = instance;
        return instance != null;
    }
}
