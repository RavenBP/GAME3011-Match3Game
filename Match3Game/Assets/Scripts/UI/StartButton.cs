using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class could probably be renamed to GameManager...
public partial class StartButton : MonoBehaviour
{
    [SerializeField]
    private GameObject MediumMatch3GamePrefab;
    [SerializeField]
    private GameObject EasyMatch3GamePrefab;
    [SerializeField]
    private GameObject HardMatch3GamePrefab;

    [SerializeField]
    private GameObject Canvas;
    [SerializeField]
    private GameObject StopButton;

    public static Difficulty difficulty = Difficulty.MEDIUM;

    public void StartGame()
    {
        switch (difficulty)
        {
            case Difficulty.EASY:
                Instantiate(EasyMatch3GamePrefab, Canvas.transform);
                break;
            case Difficulty.MEDIUM:
                Instantiate(MediumMatch3GamePrefab, Canvas.transform);
                break;
            case Difficulty.HARD:
                Instantiate(HardMatch3GamePrefab, Canvas.transform);
                break;
            default:
                Instantiate(MediumMatch3GamePrefab, Canvas.transform);
                break;
        }

        this.gameObject.SetActive(false);

        GameObject[] difficultyButtons = GameObject.FindGameObjectsWithTag("DifficultyButton");

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].SetActive(false);
        }

        StopButton.SetActive(true);
    }
}
