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
    public RelicUIManager relic_ui_manager;

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

                    pieceUI.GetComponent<PiecePanelClick>().SetSelected(true);
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

                pieceUI.GetComponent<PiecePanelClick>().SetSelected(true);
                //craftingManager.DoSpellPieces();
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

                pieceUI.GetComponent<PiecePanelClick>().SetSelected(true);
                //craftingManager.DoRelicPieces();
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

                pieceUI.GetComponent<PiecePanelClick>().SetSelected(true);
                //craftingManager.DoRelicPieces();
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
        craftable_pieces.Clear();

        // Update the crafting panel
        /*if (craftingManager != null)
        {
            craftingManager.DoSpellPieces();
            craftingManager.DoRelicPieces();
        }*/
    }

    public void RemovePlayerPieces()
    {
        PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();

        foreach (Piece piece in craftable_pieces)
        {
            UnityEngine.Debug.Log(piece.GetType());
            if (piece.GetType().IsSubclassOf(typeof(Spell)))
            {
                UnityEngine.Debug.Log("Delete spell.");
                player.spell_pieces.Remove((Spell)piece);
            }
                
            if (piece.GetType().IsSubclassOf(typeof(RelicPart)))
            {
                player.relic_pieces.Remove((RelicPart)piece);
                UnityEngine.Debug.Log("delete relic.");
            }
                
        }

    }

    public bool CanCraftSpell()
    {
        // Check if there's a base spell
        if (base_spell == null || base_spell.GetComponent<PieceUI>().spell_piece == null)
        {
            Debug.Log("Cannot craft spell: No base spell");
            return false;
        }
        return true;
    }

    public bool CanCraftRelic()
    {
        // Check if both trigger and effect are present
        if (trigger == null || trigger.GetComponent<PieceUI>().relic_piece == null)
        {
            Debug.Log("Cannot craft relic: No trigger");
            return false;
        }
        if (effect == null || effect.GetComponent<PieceUI>().relic_piece == null)
        {
            Debug.Log("Cannot craft relic: No effect");
            return false;
        }
        return true;
    }

    public void CraftSpell()
    {

        // Get the base spell
        Spell baseSpell = base_spell.GetComponent<PieceUI>().spell_piece;
        craftable_pieces.Add(baseSpell);
        
        // Apply modifiers in order
        for (int i = mod_spells.Length - 1; i >= 0; i--)
        {
            if (mod_spells[i] != null)
            {
                Spell modifier = mod_spells[i].GetComponent<PieceUI>().spell_piece;
                if (modifier != null && modifier.IsModifier())
                {
                    craftable_pieces.Add(modifier);
                    modifier.SetInnerSpell(baseSpell);
                    baseSpell = modifier;
                }
            }
        }

        // Add the crafted spell to the player's spellbook
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            PlayerController player = GameManager.Instance.player.GetComponent<PlayerController>();
            if (player != null && player.spellUIContainer != null)
            {
                player.spellcaster.reward_spell = baseSpell;
                player.spellUIContainer.AddSpell();
                RemovePlayerPieces();
                ClearPieces();
                craftingManager.DoSpellPieces();
            }
        }
    }

    public void CraftRelic()
    {

        // Get the trigger and effect
        RelicTriggers relicTrigger = trigger.GetComponent<PieceUI>().relic_piece as RelicTriggers;
        RelicEffects relicEffect = effect.GetComponent<PieceUI>().relic_piece as RelicEffects;

        craftable_pieces.Add(relicTrigger);
        craftable_pieces.Add(relicEffect);

        if (relicTrigger != null && relicEffect != null)
        {
            // Create a name based on the trigger and effect
            string relicName = relicTrigger.GetName() + " " + relicEffect.GetName();
            
            // Use the trigger's sprite as the base sprite
            int sprite = relicTrigger.GetIcon();
            
            // Combine trigger and effect descriptions
            string description = relicTrigger.GetName() + " triggers " + relicEffect.GetName();

            // Create the relic
            Relic newRelic = new Relic(relicName, sprite, description, relicTrigger, relicEffect);
            
            // Add the relic to the player's inventory
            if (GameManager.Instance != null && GameManager.Instance.player != null)
            {
                if (relic_ui_manager != null)
                {
                    relic_ui_manager.AddRelic(newRelic);
                    RemovePlayerPieces();
                    ClearPieces();
                    craftingManager.DoRelicPieces();
                }
            }
        }
    }
}
