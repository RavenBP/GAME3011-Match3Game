using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopButton : MonoBehaviour
{
    [SerializeField]
    private GameObject StartButton;
    [SerializeField]
    private GameObject[] difficultyButtons;

    public void StopGame()
    {
        Destroy(GameObject.FindGameObjectWithTag("Match3Game"));

        this.gameObject.SetActive(false);

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].SetActive(true);
        }

        StartButton.SetActive(true);
    }
}
