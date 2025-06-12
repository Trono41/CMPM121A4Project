using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using static System.Net.Mime.MediaTypeNames;

public class Craftable : MonoBehaviour
{

    public GameObject[] mod_spells;
    public GameObject base_spell;

    public GameObject trigger;
    public GameObject effect;

    List<Piece> craftable_pieces = new List<Piece>();

    private int num_pieces;
    private CraftingScreenManager craftingManager;

    void Start()
    {
        craftingManager = FindObjectOfType<CraftingScreenManager>();
        if (craftingManager == null)
        {
            Debug.LogError("CraftingScreenManager not found!");
        }
    }

    void Update()
    {
        // Update logic if needed
    }

    public void ShowSpellPiece(GameObject piece)
    {
        if (craftingManager == null) return;

        PieceUI pieceUI = piece.GetComponent<PieceUI>();
        if (pieceUI == null)
        {
            Debug.LogError($"No PieceUI component found on {piece.name}");
            return;
        }

        Spell spellComponent = pieceUI.spell_piece;
        if (spellComponent == null)
        {
            Debug.LogError($"No spell piece assigned to PieceUI on {piece.name}");
            return;
        }

        // Check if it's a modifier or base spell
        if (spellComponent.IsModifier())
        {
            // Add to modifier spells if there's space
            for (int i = 0; i < mod_spells.Length; i++)
            {
                if (mod_spells[i].GetComponent<PieceUI>().spell_piece == null)
                {
                    mod_spells[i].GetComponent<PieceUI>().SetPiece(spellComponent);
                    num_pieces++;
                    craftingManager.DoSpellPieces();
                    return;
                }
            }
            Debug.Log("No space for more modifier spells");
        }
        else
        {
            // Add base spell if there's no base spell yet
            if (base_spell.GetComponent<PieceUI>().spell_piece == null)
            {
                base_spell.GetComponent<PieceUI>().SetPiece(spellComponent);
                num_pieces++;
                craftingManager.DoSpellPieces();
            }
            else
            {
                Debug.Log("Base spell already exists");
            }
        }
    }

    public void ShowRelicPiece(GameObject piece)
    {
        if (craftingManager == null) return;

        PieceUI pieceUI = piece.GetComponent<PieceUI>();
        if (pieceUI == null)
        {
            Debug.LogError($"No PieceUI component found on {piece.name}");
            return;
        }

        RelicPart relicComponent = pieceUI.relic_piece;
        if (relicComponent == null)
        {
            Debug.LogError($"No relic piece assigned to PieceUI on {piece.name}");
            return;
        }

        // Check if it's a trigger or effect by checking the type
        if (relicComponent is RelicTriggers)
        {
            if (trigger.GetComponent<PieceUI>().relic_piece == null)
            {
                trigger.GetComponent<PieceUI>().SetPiece(relicComponent);
                num_pieces++;
                craftingManager.DoRelicPieces();
            }
            else
            {
                Debug.Log("Trigger already exists");
            }
        }
        else if (relicComponent is RelicEffects)
        {
            if (effect.GetComponent<PieceUI>().relic_piece == null)
            {
                effect.GetComponent<PieceUI>().SetPiece(relicComponent);
                num_pieces++;
                craftingManager.DoRelicPieces();
            }
            else
            {
                Debug.Log("Effect already exists");
            }
        }
        else
        {
            Debug.LogError($"Unknown RelicPart type: {relicComponent.GetType().Name}");
        }
    }

    public void ClearPieces()
    {
        // Clear spell pieces
        for (int i = 0; i < mod_spells.Length; i++)
        {
            mod_spells[i].GetComponent<PieceUI>().spell_piece = null;
            mod_spells[i].GetComponent<PieceUI>().icon.GetComponent<Image>().sprite = null;
        }
        base_spell.GetComponent<PieceUI>().spell_piece = null;
        base_spell.GetComponent<PieceUI>().icon.GetComponent<Image>().sprite = null;

        // Clear relic pieces
        trigger.GetComponent<PieceUI>().relic_piece = null;
        trigger.GetComponent<PieceUI>().icon.GetComponent<Image>().sprite = null;
        effect.GetComponent<PieceUI>().relic_piece = null;
        effect.GetComponent<PieceUI>().icon.GetComponent<Image>().sprite = null;

        num_pieces = 0;

        // Update the crafting panel
        if (craftingManager != null)
        {
            craftingManager.DoSpellPieces();
            craftingManager.DoRelicPieces();
        }
    }

}
