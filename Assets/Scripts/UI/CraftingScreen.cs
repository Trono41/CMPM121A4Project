using UnityEngine;
using UnityEngine.UI;

public class CraftingScreen : MonoBehaviour
{
    public GameObject spellCraftPanel;
    public GameObject relicCraftPanel;
    public CraftingScreenManager crafting_screen_manager;
    public Craftable craftable;
    public Button craftButton;

    void Start()
    {
        // Make sure only one panel is active at a time
        if (spellCraftPanel != null && relicCraftPanel != null)
        {
            spellCraftPanel.SetActive(true);
            relicCraftPanel.SetActive(false);
        }

        // Add listener to craft button
        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnCraftButtonClick);
        }
    }

    public void SwitchToRelicCraft()
    {
        if (spellCraftPanel != null && relicCraftPanel != null)
        {
            // Reset all piece selections before switching
            PiecePanelClick.ResetAllPieceSelections();
            craftable.ClearPieces();
            
            spellCraftPanel.SetActive(false);
            relicCraftPanel.SetActive(true);
            crafting_screen_manager.DoRelicPieces();
        }
    }

    public void SwitchToSpellCraft()
    {
        if (spellCraftPanel != null && relicCraftPanel != null)
        {
            // Reset all piece selections before switching
            PiecePanelClick.ResetAllPieceSelections();
            craftable.ClearPieces();
            
            spellCraftPanel.SetActive(true);
            relicCraftPanel.SetActive(false);
            crafting_screen_manager.DoSpellPieces();
        }
    }

    public void OnCraftButtonClick()
    {
        if (spellCraftPanel.activeSelf)
        {
            // Spell crafting
            if (craftable.CanCraftSpell())
            {
                craftable.CraftSpell();
            }
        }
        else if (relicCraftPanel.activeSelf)
        {
            // Relic crafting
            if (craftable.CanCraftRelic())
            {
                craftable.CraftRelic();
            }
        }
    }
} 