using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System;

public class PieceUI : MonoBehaviour
{

    public Spell spell_piece;
    public RelicPart relic_piece;
    public GameObject icon;
    public TextMeshProUGUI name;

    public void SetPiece(Spell piece)
    {
        this.spell_piece = piece;

        //UnityEngine.Debug.Log("PIECEUI SET");
        if (spell_piece.IsModifier())
        {
            ModifierSpell mod_spell_piece = spell_piece as ModifierSpell;
            GameManager.Instance.spellIconManager.PlaceSprite(mod_spell_piece.GetOwnIcon(), icon.GetComponent<Image>());
            name.text = spell_piece.GetName();
        }
        else
        {
            GameManager.Instance.spellIconManager.PlaceSprite(spell_piece.GetIcon(), icon.GetComponent<Image>());
            name.text = spell_piece.GetName();
        }
    }       

    public void SetPiece(RelicPart piece)
    {
        this.relic_piece = piece;

        GameManager.Instance.relicIconManager.PlaceSprite(relic_piece.GetIcon(), icon.GetComponent<Image>());
        if (relic_piece.GetName() == "GainTemporarySpellPower")
        {
            name.text = "GainTempSpellpower";
        }
        else
            name.text = relic_piece.GetName();
    }

    public void ClearPiece()
    {
        spell_piece = null;
        relic_piece = null;
        icon.GetComponent<Image>().sprite = null;
        name.text = "";
    }
}
