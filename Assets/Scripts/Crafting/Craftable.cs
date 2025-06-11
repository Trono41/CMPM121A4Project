using System.Collections.Generic;
using UnityEngine;

public class Craftable : MonoBehaviour
{

    public GameObject[] mod_spells;
    public GameObject base_spell;

    public GameObject trigger;
    public GameObject effect;

    private int craftable_pieces;
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
                if (mod_spells[i] == null)
                {
                    mod_spells[i] = piece;
                    craftable_pieces++;
                    craftingManager.DoSpellPieces();
                    return;
                }
            }
            Debug.Log("No space for more modifier spells");
        }
        else
        {
            // Add base spell if there's no base spell yet
            if (base_spell == null)
            {
                base_spell = piece;
                craftable_pieces++;
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
            if (trigger == null)
            {
                trigger = piece;
                craftable_pieces++;
                craftingManager.DoRelicPieces();
            }
            else
            {
                Debug.Log("Trigger already exists");
            }
        }
        else if (relicComponent is RelicEffects)
        {
            if (effect == null)
            {
                effect = piece;
                craftable_pieces++;
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
            mod_spells[i] = null;
        }
        base_spell = null;

        // Clear relic pieces
        trigger = null;
        effect = null;

        craftable_pieces = 0;

        // Update the crafting panel
        if (craftingManager != null)
        {
            craftingManager.DoSpellPieces();
            craftingManager.DoRelicPieces();
        }
    }
>>>>>>> a22eac594cd4331284b06b5ff6c957fbb6645273
}
