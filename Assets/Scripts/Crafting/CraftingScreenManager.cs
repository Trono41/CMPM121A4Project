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

    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        }
        
        if (container != null)
        {
            pieceUIContainer = container.GetComponent<PieceUIContainer>();
        }

        if (EventBus.Instance != null)
        {
            EventBus.Instance.OnWaveEnd += UpdatePiecePanel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Try to get player reference if not set
        if (playerController == null && GameManager.Instance != null && GameManager.Instance.player != null)
        {
            playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        }
    }

    public void UpdatePiecePanel()
    {
        playerController.GetCraftingPieces();
        DoSpellPieces();
    }

    public void DoSpellPieces()
    {
        if (playerController == null || pieceUIContainer == null || container == null) return;

        UnityEngine.Debug.Log(playerController.spell_pieces.Count);
        int i = 0;

        UnityEngine.Debug.Log(playerController.spell_pieces.Count);

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
        if (playerController == null || pieceUIContainer == null || container == null) return;

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
