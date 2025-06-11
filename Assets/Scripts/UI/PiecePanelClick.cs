using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PiecePanelClick : MonoBehaviour, IPointerClickHandler
{
    private PieceUI pieceUI;
    private Image icon;
    private GameObject highlight;
    private bool isSelected;

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
        
        highlight = transform.Find("Highlight")?.gameObject;
        if (highlight == null)
        {
            Debug.LogWarning($"No Highlight GameObject found in {gameObject.name}");
        }
        
        // Initialize highlight state
        if (highlight != null)
        {
            highlight.SetActive(false);
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

        // Handle spell pieces
        if (pieceUI.spell_piece != null)
        {
            HandleSpellClick(pieceUI.spell_piece);
        }
        // Handle relic pieces
        else if (pieceUI.relic_piece != null)
        {
            HandleRelicClick(pieceUI.relic_piece);
        }
        else
        {
            Debug.LogError($"No spell or relic piece found on {gameObject.name}");
        }
    }

    private void HandleSpellClick(Spell spell)
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null)
        {
            Debug.LogError("GameManager or player not found");
            return;
        }

        PlayerController playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found");
            return;
        }

        Debug.Log($"Current spell pieces count: {playerController.spell_pieces.Count}");
        
        // Check if we can add more pieces
        if (playerController.spell_pieces.Count < 3) // Max 3 pieces (2 modifiers + 1 base)
        {
            // If it's a modifier, it must go in the first two slots
            if (spell.IsModifier() && playerController.spell_pieces.Count >= 2)
            {
                Debug.Log("Cannot add more modifiers. Maximum of 2 modifiers allowed.");
                return;
            }
            // If it's a base spell, it must go in the last slot
            else if (!spell.IsModifier() && playerController.spell_pieces.Count < 2)
            {
                Debug.Log("Must add modifiers before adding base spell.");
                return;
            }

            Debug.Log($"Adding spell to player's pieces: {spell.GetType().Name}");
            // Add the piece to the player's inventory
            playerController.spell_pieces.Add(spell);

            // Update the crafting panel
            CraftingScreenManager manager = FindObjectOfType<CraftingScreenManager>();
            if (manager != null)
            {
                Debug.Log("Updating crafting panel");
                manager.DoSpellPieces();
            }
            else
            {
                Debug.LogError("CraftingScreenManager not found");
            }

            SetSelected(true);
        }
        else
        {
            Debug.Log("Cannot add more pieces. Maximum of 3 pieces allowed.");
        }
    }

    private void HandleRelicClick(RelicPart relic)
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null)
        {
            Debug.LogError("GameManager or player not found");
            return;
        }

        PlayerController playerController = GameManager.Instance.player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found");
            return;
        }

        Debug.Log($"Adding relic to player's pieces: {relic.GetType().Name}");
        // Add the piece to the player's inventory
        playerController.relic_pieces.Add(relic);

        // Update the crafting panel
        CraftingScreenManager manager = FindObjectOfType<CraftingScreenManager>();
        if (manager != null)
        {
            Debug.Log("Updating crafting panel");
            manager.DoRelicPieces();
        }
        else
        {
            Debug.LogError("CraftingScreenManager not found");
        }

        SetSelected(true);
    }

    public void SetSelected(bool selected)
    {
        Debug.Log($"Setting selected state to {selected} for {gameObject.name}");
        isSelected = selected;
        if (highlight != null)
        {
            highlight.SetActive(selected);
        }
        else
        {
            Debug.LogWarning($"No highlight object found on {gameObject.name}");
        }
    }
} 