using UnityEngine;

public class CraftingScreen : MonoBehaviour
{
    public GameObject spellCraftPanel;
    public GameObject relicCraftPanel;

    void Start()
    {
        // Make sure only one panel is active at a time
        if (spellCraftPanel != null && relicCraftPanel != null)
        {
            spellCraftPanel.SetActive(true);
            relicCraftPanel.SetActive(false);
        }
    }

    public void SwitchToRelicCraft()
    {
        if (spellCraftPanel != null && relicCraftPanel != null)
        {
            spellCraftPanel.SetActive(false);
            relicCraftPanel.SetActive(true);
        }
    }

    public void SwitchToSpellCraft()
    {
        if (spellCraftPanel != null && relicCraftPanel != null)
        {
            spellCraftPanel.SetActive(true);
            relicCraftPanel.SetActive(false);
        }
    }
} 