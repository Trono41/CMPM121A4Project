using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellCaster 
{
    public int mana;
    public int max_mana;
    public int mana_reg;
    public int spellpower;
    public Hittable.Team team;
    public Spell spell;
    public Spell reward_spell;
    public SpellBuilder spellbuilder = new SpellBuilder();

    SpellUIContainer spellUIContainer;

    public List<Spell> spellbook = new List<Spell>();

    void Start()
    {
        
    }

    public IEnumerator ManaRegeneration()
    {
        while (true)
        {
            mana += mana_reg;
            mana = Mathf.Min(mana, max_mana);
            if (mana >= max_mana)
                EventBus.Instance.DoMaxMana();
            yield return new WaitForSeconds(1);
        }
    }

    public SpellCaster(int mana, int mana_reg, int spellpower, Hittable.Team team)
    {
        this.mana = mana;
        this.max_mana = mana;
        this.mana_reg = mana_reg;
        this.spellpower = spellpower;
        this.team = team;
        spell = spellbuilder.BuildSpell("arcane_bolt", this);

        spellUIContainer = GameObject.FindGameObjectsWithTag("spelluicontainer")[0].GetComponent<SpellUIContainer>();

        // spell = spellbuilder.BuildSpells("chaos", "doubler", "arcane_spray", this);
        spellbook.Add(spell);

        EventBus.Instance.OnWaveEnd += GenerateRandomSpell;

        //Debug.Log("Parent Spell: " + spell);
    }

    void FixedUpdate()
    {

    }

    public IEnumerator Cast(Vector3 where, Vector3 target)
    {
        Vector3 direction = target - where;
        if (mana >= spell.GetManaCost() && spell.IsReady())
        {
            mana -= spell.GetManaCost();
            yield return spell.Cast(where, target, direction, team);
        }

        EventBus.Instance.DoCastSpell();
        yield break;
    }

    public void GenerateRandomSpell()
    {
        reward_spell = spellbuilder.BuildSpell(this);
    }

    public void AddSpell()
    {
        Debug.Log("Spell Added");

        if (spellbook.Count >= 4)
            return;

        spellbook.Add(reward_spell);
    }

    public void DropSpell(int spell_to_drop)
    {
        Spell spell_temp = spellbook.ElementAt<Spell>(spell_to_drop);
        spellbook.RemoveAt(spell_to_drop);

        EventBus.Instance.DoSpellDrop();

        if (spell_temp == spell)
            spell = spellbook.ElementAt<Spell>(0);
    }

    public void ChangeSpell()
    {
        int curr = spellbook.IndexOf(spell);
        Debug.Log(curr);

        spellUIContainer.UnHighlightSpell(curr);

        if (curr >= spellbook.Count - 1)
        {
            curr = 0;
        }
        else
        {
            curr += 1;
        }

        spell = spellbook[curr];

        spellUIContainer.HighlightSpell(curr);

    }

    public int GetSpellPower()
    {
        return spellpower;
    }

}
