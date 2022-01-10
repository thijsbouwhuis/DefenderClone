using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    private int currentScore = 0;
    public Text scoreText;
    void Start()
    {
        UpdateScoreBoard(); ;
    }
    public void AddScore(int newScore)
    {
        currentScore += newScore;
        UpdateScoreBoard();
    }

    public void SetScore(int newScore)
    {
        currentScore = newScore;
        UpdateScoreBoard();
    }

    private void UpdateScoreBoard()
    {
        scoreText.text = "Score : " + currentScore.ToString();
    }
}
