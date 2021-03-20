using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Match3 : MonoBehaviour
{
    public static int score;
    public static int targetScore;
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;
    public RectTransform killedBoard;
    public TMP_Text timerText;
    public GameObject resultsPanel;
    [SerializeField]
    private AudioClip[] audioClips;

    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject killedPiece;

    [Header("Game Settings")]
    [SerializeField]
    [Range(5, 30)]
    private float time = 15.0f;
    [SerializeField]
    private int scoreToReach = 15;
    [SerializeField]
    [Range(3, 10)]
    private int width = 10; //10 // hard 10x6 / medium 8x6 / easy 6x6
    [SerializeField]
    [Range(3, 6)]
    private int height = 6; //6

    private int[] fills;
    private Node[,] board;

    private List<NodePiece> update;
    private List<FlippedPieces> flipped;
    private List<NodePiece> dead;
    private List<KilledPiece> killed;

    private float timeRemaining;
    private bool noTime = false;
    private bool gameFinished = false;

    private AudioSource audioSource;

    System.Random random;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        StartGame();
        timeRemaining = time;
        score = 0;
        targetScore = scoreToReach;

        InvokeRepeating(nameof(UpdateMatch3Game), 0.0f, 1.0f);
    }

    void Update()
    {
        if (gameFinished == false)
        {
            List<NodePiece> finishedUpdating = new List<NodePiece>();

            timerText.text = "Time: " + timeRemaining.ToString();

            if (score >= targetScore)
            {
                gameFinished = true;
                CancelInvoke(nameof(UpdateMatch3Game));
                resultsPanel.SetActive(true);

                audioSource.clip = audioClips[1];
                audioSource.Play();

                Debug.Log("Game Complete - You Win!");
            }

            for (int i = 0; i < update.Count; i++)
            {
                NodePiece piece = update[i];

                if (piece.UpdatePiece() == false)
                {
                    finishedUpdating.Add(piece);
                }
            }

            for (int i = 0; i < finishedUpdating.Count; i++)
            {
                NodePiece piece = finishedUpdating[i];
                FlippedPieces flip = GetFlipped(piece);
                NodePiece flippedPiece = null;

                int x = (int)piece.index.x;
                fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

                List<Point> connected = IsConnected(piece.index, true);
                bool wasFlipped = (flip != null);

                // If flipped make this update
                if (wasFlipped)
                {
                    flippedPiece = flip.GetOtherPiece(piece);
                    AddPoints(ref connected, IsConnected(flippedPiece.index, true));
                }

                // If match wasn't made
                if (connected.Count == 0)
                {
                    // If flipped
                    if (wasFlipped)
                    {
                        FlipPieces(piece.index, flippedPiece.index, false); // Flip pieces back
                    }
                }
                else // If match is made
                {
                    // Remove the node pieces that are connected
                    foreach (Point p in connected)
                    {
                        KillPiece(p);
                        Node node = GetNodeAtPoint(p);
                        NodePiece nodePiece = node.GetPiece();

                        if (nodePiece != null)
                        {
                            nodePiece.gameObject.SetActive(false);
                            dead.Add(nodePiece);
                        }

                        node.SetPiece(null);
                    }

                    audioSource.clip = audioClips[0];
                    audioSource.Play();

                    ApplyGravityToBoard();
                }

                // Remove the flip after update
                flipped.Remove(flip); 
                update.Remove(piece);
            }
        }
    }

    void UpdateMatch3Game()
    {
        if (noTime == false)
        {
            DecrementTimeRemaining(1.0f);
        }
        else // No Time Remaining
        {
            Debug.Log("No Time Remaining! - GAME OVER");

            audioSource.clip = audioClips[2];
            audioSource.Play();

            gameFinished = true;
            CancelInvoke(nameof(UpdateMatch3Game));
            resultsPanel.SetActive(true);
        }
    }

    void DecrementTimeRemaining(float seconds)
    {
        timeRemaining -= seconds;

        if (timeRemaining <= 0.0f)
        {
            noTime = true;
        }
    }

    public void ApplyGravityToBoard()
    {
        for (int x = 0; x < width; x++)
        {
            // Start at bottom and grab next
            for (int y = (height - 1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = GetNodeAtPoint(p);
                int val = GetValueAtPoint(p);

                if (val != 0)
                {
                    continue; // If not a hole, move to the next
                }

                for (int ny = (y - 1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = GetValueAtPoint(next);

                    if (nextVal == 0)
                    {
                        continue;
                    }

                    if (nextVal != -1)
                    {
                        Node gotten = GetNodeAtPoint(next);
                        NodePiece piece = gotten.GetPiece();

                        // Set hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        // Make new hole
                        gotten.SetPiece(null);
                    }
                    else // Use dead ones or create new pieces to fill holes (hit a -1)
                    {
                        int newVal = FillPiece();
                        NodePiece piece;
                        Point fallPnt = new Point(x, (-1 - fills[x]));

                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            piece = revived;

                            dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            piece = n;
                        }

                        piece.Initialize(newVal, p, pieces[newVal - 1]);
                        piece.rect.anchoredPosition = GetPositionFromPoint(fallPnt);

                        Node hole = GetNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }

                    break;
                }
            }
        }
    }

    FlippedPieces GetFlipped(NodePiece p)
    {
        FlippedPieces flip = null;

        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].GetOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }

        return flip;
    }

    void StartGame()
    {
        fills = new int[width];
        string seed = GetRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        killed = new List<KilledPiece>();

        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    void InitializeBoard()
    {
        board = new Node[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node((boardLayout.rows[y].row[x]) ? -1 : FillPiece(), new Point(x, y));
            }
        }
    }

    void VerifyBoard()
    {
        List<int> remove;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = GetValueAtPoint(p);

                if (val <= 0)
                {
                    continue;
                }

                remove = new List<int>();

                while (IsConnected(p, true).Count > 0)
                {
                    val = GetValueAtPoint(p);

                    if (!remove.Contains(val))
                    {
                        remove.Add(val);
                    }

                    SetValueAtPoint(p, NewValue(ref remove));
                }
            }
        }
    }

    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = GetNodeAtPoint(new Point(x, y));

                int val = node.value;

                if (val <= 0)
                {
                    continue;
                }

                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(64 + (128 * x), -64 - (128 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (GetValueAtPoint(one) < 0)
        {
            return;
        }

        Node nodeOne = GetNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.GetPiece();

        if (GetValueAtPoint(two) > 0)
        {
            Node nodeTwo = GetNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.GetPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if (main)
            {
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));
            }

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
        {
            ResetPiece(pieceOne);
        }
    }

    void KillPiece(Point p)
    {
        List<KilledPiece> available = new List<KilledPiece>();
        for (int i = 0; i < killed.Count; i++)
        {
            if (!killed[i].falling)
            {
                available.Add(killed[i]);
            }
        }

        KilledPiece set = null;

        if (available.Count > 0)
        {
            set = available[0];
        }
        else
        {
            GameObject kill = GameObject.Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);
        }

        int val = GetValueAtPoint(p) - 1;

        if (set != null && val >= 0 && val < pieces.Length)
        {
            set.Initialize(pieces[val], GetPositionFromPoint(p));
        }
    }

    List<Point> IsConnected(Point p, bool main)
    {
        List<Point> connected = new List<Point>();
        int val = GetValueAtPoint(p);
        Point[] directions =
        {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left
        };

        // Check if there are 2 or more same pieces in each direction
        foreach (Point dir in directions)
        {
            List<Point> line = new List<Point>();

            int same = 0;

            for (int i = 1; i < 3; i++)
            {
                Point check = Point.Add(p, Point.Multipy(dir, i));

                if (GetValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            // If there is more than 1 of the same piece in direction
            if (same > 1)
            {
                AddPoints(ref connected, line); // Add these points to the overarching connected list
            }
        }

        // Check if in the middle of two same pieces
        for (int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.Add(p, directions[i]), Point.Add(p, directions[i + 2]) };

            // Check both sides of piece
            foreach (Point next in check)
            {
                // Add to list
                if (GetValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }

            if (same > 1)
            {
                AddPoints(ref connected, line);
            }
        }

        // Check for 2x2
        for (int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;

            if (next >= 4)
            {
                next -= 4;
            }

            Point[] check = { Point.Add(p, directions[i]), Point.Add(p, directions[next]), Point.Add(p, Point.Add(directions[i], directions[next])) };

            // Check all sides of piece
            foreach (Point pnt in check)
            {
                // Add to list
                if (GetValueAtPoint(pnt) == val)
                {
                    square.Add(pnt);
                    same++;
                }
            }

            if (same > 2)
            {
                AddPoints(ref connected, square);
            }
        }

        // Check for other matches along current match
        if (main)
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, IsConnected(connected[i], false));
            }
        }

        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd)
            {
                points.Add(p);
            }
        }
    }

    int FillPiece()
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;

        return val;
    }

    int GetValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height)
        {
            return -1;
        }

        return board[p.x, p.y].value;
    }

    void SetValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    Node GetNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    int NewValue(ref List<int> remove)
    {
        List<int> available = new List<int>();

        for (int i = 0; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }

        foreach (int i in remove)
        {
            available.Remove(i);
        }

        if (available.Count <= 0)
        {
            return 0;
        }

        return available[random.Next(0, available.Count)];
    }

    string GetRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdeghijklmnopqrstuvwxyz1234567890!@#$%^&*()";

        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }

        return seed;
    }

    public Vector2 GetPositionFromPoint(Point p)
    {
        return new Vector2(64 + (128 * p.x), -64 - (128 * p.y));
    }
}