using System;
using Modules.Score;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounterVisual : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI output;
    [SerializeField] private ScoreCounterMB scoreCounter;
    [SerializeField] private int numberOfCharacters = 2;


    private void Start()
    {
        UpdateScore(scoreCounter.ScoreCounterData.Value);
    }


    public void UpdateScore(float value)
    {
        output.text = $"{Math.Round(value, numberOfCharacters)}";
    }

    private void OnEnable()
    {
        scoreCounter.OnChangeValue.AddListener(UpdateScore);
    }

    private void OnDisable()
    {
        scoreCounter.OnChangeValue.RemoveListener(UpdateScore);
    }
}
