using UnityEngine;

public class PieceUIContainer : MonoBehaviour
{
    public GameObject[] piece_uis;

    public void SetPieceUI(Spell piece, int index)
    {
        piece_uis[index].GetComponent<PieceUI>().SetPiece(piece);
    }

    public void SetPieceUI(RelicPart piece, int index)
    {
        piece_uis[index].GetComponent<PieceUI>().SetPiece(piece);
    }

    public void ClearPieces()
    {
        foreach (GameObject piece_ui in piece_uis)
        {
            piece_ui.GetComponent<PieceUI>().ClearPiece();
        }
    }
}
