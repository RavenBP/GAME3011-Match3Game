using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    public void EasyButton()
    {
        StartButton.difficulty = StartButton.Difficulty.EASY;
    }

    public void MediumButton()
    {
        StartButton.difficulty = StartButton.Difficulty.MEDIUM;
    }

    public void HardButton()
    {
        StartButton.difficulty = StartButton.Difficulty.HARD;
    }
}
