using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KilledPiece : MonoBehaviour
{
    public bool falling;

    private float speed = 16.0f;
    private float gravity = 128.0f;

    private Vector2 moveDir;
    private RectTransform rectTransform;
    private Image image;

    public void Initialize(Sprite piece, Vector2 start)
    {
        falling = true;

        moveDir = Vector2.up;
        moveDir.x = Random.Range(-1.0f, 1.0f);
        moveDir *= speed / 2.0f;

        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        image.sprite = piece;
        rectTransform.anchoredPosition = start;

        Match3.score += 1;

        //Debug.Log("Total Score: " + Match3.score.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (falling == false)
        {
            return;
        }

        moveDir.y -= Time.deltaTime * gravity;
        moveDir.x = Mathf.Lerp(moveDir.x, 0, Time.deltaTime);
        rectTransform.anchoredPosition += moveDir * Time.deltaTime * speed;

        if (rectTransform.position.x < -128.0f || rectTransform.position.x > Screen.width + 128.0f || rectTransform.position.y < -128.0f || rectTransform.position.y > Screen.height + 128.0f)
        {
            falling = false;
        }
    }
}
