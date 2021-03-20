using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces instance;
    private Match3 game;

    private NodePiece moving;
    private Point newIndex;
    private Vector2 mouseStart;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        game = GetComponent<Match3>();
    }

    void Update()
    {
        if (moving != null)
        {
            Vector2 dir = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 normalizedDir = dir.normalized;
            Vector2 absDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            newIndex = Point.Clone(moving.index);
            Point add = Point.Zero;

            if (dir.magnitude > 64) // If cursor is 64 pixels away from the starting point of the cursor
            {
                // Make add either (1, 0) | (-1, 0) | (0, 1) | (0, -1) depending on the direction of the mouse point
                if (absDir.x > absDir.y)
                {
                    add = (new Point((normalizedDir.x > 0) ? 1 : -1, 0));
                }
                else if (absDir.y > absDir.x)
                {
                    add = (new Point(0, (normalizedDir.y > 0) ? -1 : 1));
                }
            }

            newIndex.Add(add);

            Vector2 pos = game.GetPositionFromPoint(moving.index);

            if (!newIndex.Equals(moving.index))
            {
                pos += Point.Multipy(new Point(add.x, -add.y), 16).ToVector();
            }

            moving.MovePositionTo(pos);
        }
    }

    public void MovePiece(NodePiece piece)
    {
        if (moving != null)
        {
            return;
        }

        moving = piece;
        mouseStart = Input.mousePosition;
    }

    public void DropPiece()
    {
        if (moving == null)
        {
            return;
        }

        //Debug.Log("Dropped");

        if (!newIndex.Equals(moving.index))
        {
            game.FlipPieces(moving.index, newIndex, true);
        }
        else
        {
            game.ResetPiece(moving);
        }

        moving = null;
    }
}