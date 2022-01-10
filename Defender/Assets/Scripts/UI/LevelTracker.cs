using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTracker : MonoBehaviour
{
    private int currentLevel = 0;
    public Text levelText;

    public void SetLevel(int newLevel)
    {
        currentLevel = newLevel;

        levelText.text = "Wave : " + (currentLevel + 1).ToString();
    }
}
