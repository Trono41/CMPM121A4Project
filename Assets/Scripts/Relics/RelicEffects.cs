using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public class RelicEffects : RelicPart
{
    protected RPNEvaluator rpn = new RPNEvaluator();
    protected Dictionary<string, int> variables = new Dictionary<string, int>();

    protected int amount;
    public string until;
    protected PlayerController owner;

    public virtual void apply()
    {

    }

    public virtual void remove()
    {

    }
}

public class GainMana : RelicEffects
{
    public GainMana(string amount, int sprite, PlayerController owner)
    {
        this.amount = rpn.Eval(amount, variables);
        name = "GainMana";
        this.sprite = sprite;
        this.owner = owner;
    }

    override public string GetName()
    {
        return name;
    }

    public override void apply()
    {
        owner.spellcaster.mana += amount;
        //UnityEngine.Debug.Log("Applied!");
    }
}

public class GainSpellPower : RelicEffects
{
    public GainSpellPower(string amount, int sprite, string until, PlayerController owner)
    {
        variables["wave"] = GameManager.Instance.GetWave();
        this.amount = rpn.Eval(amount, variables);
        this.sprite = sprite;
        this.until = until;
        this.owner = owner;
        name = "GainSpellPower";
    }

    override public string GetName()
    {
        return name;
    }

    public override void apply()
    {
        owner.spellpower += amount;
        //UnityEngine.Debug.Log("In GainSpellPower.apply()\nOwner's spellpower set to: " + owner.spellpower);
    }

    public override void remove()
    {
        owner.spellpower -= amount;
        //UnityEngine.Debug.Log("In GainSpellPower.remove()\nOwner's spellpower set to: " + owner.spellpower);
    }
}

public class GainTempSpellPower : RelicEffects
{
    public GainTempSpellPower(string amount, int sprite, string until, PlayerController owner)
    {
        variables["wave"] = GameManager.Instance.GetWave();
        this.amount = rpn.Eval(amount, variables);
        this.sprite = sprite;
        this.until = until;
        this.owner = owner;
        name = "GainTemporarySpellPower";
    }

    override public string GetName()
    {
        return name;
    }

    public override void apply()
    {
        owner.spellcaster.spellpower += amount;
        //UnityEngine.Debug.Log("In GainSpellPower.apply()\nOwner's spellpower set to: " + owner.spellcaster.spellpower);
    }

    public override void remove()
    {
        owner.spellcaster.spellpower -= amount;
        //UnityEngine.Debug.Log("In GainSpellPower.remove()\nOwner's spellpower set to: " + owner.spellcaster.spellpower);
    }
}

public class GainDefense : RelicEffects
{
    float defense_multiplier;

    public GainDefense(string amount, int sprite, string until, PlayerController owner)
    {
        variables["wave"] = GameManager.Instance.GetWave();
        defense_multiplier = rpn.EvalFloat(amount, variables);
        this.sprite = sprite;
        this.owner = owner;
        this.until = until;
        name = "GainDefense";
    }

    override public string GetName()
    {
        return name;
    }

    public override void apply()
    {
        owner.hp.defense *= defense_multiplier;
        //Debug.Log("In GainDefense.apply()\nOwner's defense set to: " + owner.hp.defense);
    }

    public override void remove()
    {
        owner.hp.defense /= defense_multiplier;
        //Debug.Log("In GainDefense.remove()\nOwner's defense set to: " + owner.hp.defense);
    }
}

public class RegainHP : RelicEffects
{
    public RegainHP(string amount, int sprite, PlayerController owner)
    {
        this.amount = rpn.Eval(amount, variables);
        this.sprite = sprite;
        this.owner = owner;
        name = "RegainHP";
    }

    override public string GetName()
    {
        return name;
    }

    public override void apply()
    {
        //Debug.Log("PlayerHP is: " + owner.hp.hp);
        owner.hp.hp += 10;
        if (owner.hp.hp > owner.hp.max_hp)
        {
            owner.hp.hp = owner.hp.max_hp;
        }
        //Debug.Log("PlayerHP is now: " + owner.hp.hp);
    }
}

public class GainMaxHP : RelicEffects
{
    public GainMaxHP(string amount, int sprite, PlayerController owner)
    {
        this.amount = rpn.Eval(amount, variables);
        this.sprite = sprite;
        this.owner = owner;
        name = "GainMaxHP";
    }

    override public string GetName()
    {
        return name;
    }

    public override void apply()
    {
        //Debug.Log("PlayerMaxHp is: " + owner.hp.max_hp);
        owner.hp.bonus_max_hp += 10;
    }
}

