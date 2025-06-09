using UnityEngine;

public class PieceUIContainer : MonoBehaviour
{
    public GameObject[] piece_uis;

    public void SetPieceUI(Piece piece, int index)
    {
        rewardRelics[index].GetComponent<RelicUI>().SetRelic(relic_to_set);
    }
}
