using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PiecePanelClick : MonoBehaviour, IPointerClickHandler
{
    private PieceUI pieceUI;
    private Image icon;
    private GameObject highlight;
    private bool isSelected;
    private Craftable craftable;

    void Start()
    {
        Debug.Log($"PiecePanelClick Start on {gameObject.name}");
        
        // Get the PieceUI component from the same GameObject
        pieceUI = GetComponent<PieceUI>();
        if (pieceUI == null)
        {
            Debug.LogError($"No PieceUI component found on {gameObject.name}");
        }
        else
        {
            Debug.Log($"Found PieceUI component on {gameObject.name}");
        }
        
        // Get the icon and highlight components
        icon = GetComponentInChildren<Image>();
        if (icon == null)
        {
            Debug.LogError($"No Image component found in children of {gameObject.name}");
        }
        
        // Find or create the highlight object
        highlight = transform.Find("Highlight")?.gameObject;
        if (highlight == null)
        {
            Debug.Log($"Creating Highlight GameObject for {gameObject.name}");
            highlight = new GameObject("Highlight");
            highlight.transform.SetParent(transform, false);
            
            // Add Image component for the highlight
            Image highlightImage = highlight.AddComponent<Image>();
            highlightImage.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent white
            
            // Set the highlight to be the same size as the parent
            RectTransform highlightRect = highlight.GetComponent<RectTransform>();
            RectTransform parentRect = GetComponent<RectTransform>();
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;
            highlightRect.sizeDelta = Vector2.zero;
            highlightRect.anchoredPosition = Vector2.zero;
        }
        
        // Initialize highlight state
        highlight.SetActive(false);

        // Find the Craftable component
        craftable = FindObjectOfType<Craftable>();
        if (craftable == null)
        {
            Debug.LogError("Craftable component not found in scene!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MouseClick();
    }

    public void MouseClick()
    {
        Debug.Log($"Piece clicked: {gameObject.name}");
        
        if (pieceUI == null)
        {
            Debug.LogError($"No PieceUI component found on {gameObject.name}");
            return;
        }

        if (craftable == null)
        {
            Debug.LogError("Craftable component not found!");
            return;
        }

        // Handle spell pieces
        if (pieceUI.spell_piece != null && !isSelected)
        {
            craftable.ShowSpellPiece(gameObject);
            //SetSelected(true);
        }
        // Handle relic pieces
        else if (pieceUI.relic_piece != null)
        {
            craftable.ShowRelicPiece(gameObject);
            //SetSelected(true);
        }
        else
        {
            return;
        }
    }

    public void SetSelected(bool selected)
    {
        Debug.Log($"Setting selected state to {selected} for {gameObject.name}");
        isSelected = selected;
        if (highlight != null)
        {
            UnityEngine.Debug.Log("Highlight Activated!");
            highlight.SetActive(selected);
        }
        else
        {
            Debug.LogWarning($"No highlight object found on {gameObject.name}");
        }
    }

    private void ResetAllSelections()
    {
        // Find all PiecePanelClick components in the scene
        PiecePanelClick[] allPieces = FindObjectsOfType<PiecePanelClick>();
        foreach (PiecePanelClick piece in allPieces)
        {
            if (piece != this) // Don't reset the current piece
            {
                piece.SetSelected(false);
            }
        }
    }

    // Call this method when switching between spell and relic crafting screens
    public static void ResetAllPieceSelections()
    {
        PiecePanelClick[] allPieces = FindObjectsOfType<PiecePanelClick>();
        foreach (PiecePanelClick piece in allPieces)
        {
            piece.SetSelected(false);
        }
    }
} 