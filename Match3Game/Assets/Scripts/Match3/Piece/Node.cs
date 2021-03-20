[System.Serializable]
public class Node
{
    public int value; // 0 = blank, 1 = square, 2 = circle, 3 = star, 4 = triangle, 5 = hexagon, -1 = hole
    public Point index;
    private NodePiece piece;

    public Node(int val, Point i)
    {
        value = val;
        index = i;
    }

    public void SetPiece(NodePiece nodePiece)
    {
        piece = nodePiece;
        value = (piece == null) ? 0 : piece.value;

        if (piece == null)
        {
            return;
        }

        piece.SetIndex(index);
    }

    public NodePiece GetPiece()
    {
        return piece;
    }
}