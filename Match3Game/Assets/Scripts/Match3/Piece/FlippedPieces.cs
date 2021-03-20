[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece a, NodePiece b)
    {
        one = a; two = b;
    }

    public NodePiece GetOtherPiece(NodePiece nodePiece)
    {
        if (nodePiece == one)
        {
            return two;
        }
        else if (nodePiece == two)
        {
            return one;
        }
        else
        {
            return null;
        }
    }
}