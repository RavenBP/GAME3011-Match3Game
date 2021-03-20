using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyText : MonoBehaviour
{
    private TMP_Text difficultyText;

    private void Start()
    {
        difficultyText = GetComponent<TMP_Text>();

        InvokeRepeating(nameof(UpdateText), 0.0f, 0.5f);
    }

    private void UpdateText()
    {
        switch (StartButton.difficulty)
        {
            case StartButton.Difficulty.EASY:
                difficultyText.text = "Current Difficulty: Easy";
                break;
            case StartButton.Difficulty.MEDIUM:
                difficultyText.text = "Current Difficulty: Medium";
                break;
            case StartButton.Difficulty.HARD:
                difficultyText.text = "Current Difficulty: Hard";
                break;
        }
    }
}
