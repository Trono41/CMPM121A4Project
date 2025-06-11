using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

public class CraftingScreenManager : MonoBehaviour
{

    public GameObject container;
    public PieceUIContainer pieceUIContainer;

    public GameObject player;
    public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        pieceUIContainer = container.GetComponent<PieceUIContainer>();

        EventBus.Instance.OnWaveEnd += DoSpellPieces;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoSpellPieces()
    {
        UnityEngine.Debug.Log(playerController.spell_pieces.Count);
        int i = 0;

        pieceUIContainer.ClearPieces();
        foreach (var piece in playerController.spell_pieces)
        {
            UnityEngine.Debug.Log(piece);
            pieceUIContainer.SetPieceUI(piece, i);
            i++;
        }

        container.SetActive(true);
    }

    public void DoRelicPieces()
    {
        UnityEngine.Debug.Log(playerController.spell_pieces.Count);
        int i = 0;

        pieceUIContainer.ClearPieces();
        foreach (var piece in playerController.relic_pieces)
        {
            UnityEngine.Debug.Log(piece);
            pieceUIContainer.SetPieceUI(piece, i);
            i++;
        }

        container.SetActive(true);
    }
}
