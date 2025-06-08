using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class ModifierSpell : Spell
{
    public Spell inner;

    public string damage_multiplier;
    public string damage_adder;

    public string mana_multiplier;
    public string mana_adder;

    public string speed_multiplier;
    public string speed_adder;

    public string cooldown_multiplier;
    public string cooldown_adder;

    public string spellpower_multiplier;
    public string spellpower_adder;

    public string lifetime_multiplier;
    public string lifetime_adder;

    public float delay;
    public float angle;

    public string projectile_trajectory;

    RPNEvaluator rpnEval = new RPNEvaluator();
    Dictionary<string, int> variables = new Dictionary<string, int>();

    public ModifierSpell()
    {
       
    }

    override public void SetInnerSpell(Spell inner)
    {
        this.inner = inner;
    }

    override public Spell GetInnerSpell()
    {
        return this.inner;
    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();

    }

    override public IEnumerator Cast(Vector3 where, Vector3 target, Vector3 direction, Hittable.Team team, SpellModifiers mods)
    {
        AddManaCostMods(mods);
        AddDamageMods(mods);
        AddProjectileMods(mods);
        AddCooldownMods(mods);
        AddSpellpowerMods(mods);

        yield return inner.Cast(where, target, direction, team, mods);
        yield return new WaitForEndOfFrame();
    }

    SpellModifiers AddManaCostMods(SpellModifiers mods)
    {
        if (mana_multiplier != null)
            mods.AddManaCostMod(new Multiplier(rpnEval.EvalFloat(mana_multiplier, variables)));
        if (mana_adder != null)
            mods.AddManaCostMod(new Adder(rpnEval.Eval(mana_adder, variables)));

        return mods;
    }

    SpellModifiers AddDamageMods(SpellModifiers mods)
    {
        if (damage_multiplier != null)
            mods.AddDamageMod(new Multiplier(rpnEval.EvalFloat(damage_multiplier, variables)));
        if (damage_adder != null)
            mods.AddDamageMod(new Adder(rpnEval.Eval(damage_adder, variables)));

        return mods;
    }

    SpellModifiers AddProjectileMods(SpellModifiers mods)
    {
        if (speed_multiplier != null)
            mods.AddSpeedMod(new FloatMultiplier(rpnEval.EvalFloat(speed_multiplier, variables)));
        if (speed_adder != null)
            mods.AddSpeedMod(new FloatAdder(rpnEval.EvalFloat(speed_adder, variables)));
        if (lifetime_multiplier != null)
            mods.AddLifetimeMod(new FloatMultiplier(rpnEval.EvalFloat(lifetime_multiplier, variables)));
        if (lifetime_adder != null)
            mods.AddLifetimeMod(new FloatAdder(rpnEval.EvalFloat(lifetime_adder, variables)));
        if (projectile_trajectory != null)
            mods.AddProjectileTrajectoryMod(new Setter<string>(projectile_trajectory));

        return mods;
    }

    SpellModifiers AddCooldownMods(SpellModifiers mods)
    {
        if (cooldown_multiplier != null)
            mods.AddCooldownMod(new FloatMultiplier(rpnEval.EvalFloat(cooldown_multiplier, variables)));
        if (cooldown_adder != null)
            mods.AddCooldownMod(new FloatAdder(rpnEval.Eval(cooldown_adder, variables)));

        return mods;
    }

    SpellModifiers AddSpellpowerMods(SpellModifiers mods)
    {
        if (spellpower_multiplier != null)
            mods.AddSpellpowerMod(new Multiplier(rpnEval.EvalFloat(spellpower_multiplier, variables)));
        if (spellpower_adder != null)
            mods.AddSpellpowerMod(new Adder(rpnEval.Eval(spellpower_adder, variables)));

        return mods;
    }

    public override string GetName()
    {
        return name;
    }

    public override int GetManaCost(SpellModifiers mods)
    {
        return inner.GetManaCost(AddManaCostMods(mods));
    }

    public override int GetDamage(SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();
        variables["wave"] = GameManager.Instance.GetWave();

        return inner.GetDamage(AddDamageMods(mods));
    }

    public override float GetCooldown(SpellModifiers mods)
    {
        return inner.GetCooldown(AddCooldownMods(mods));
    }

    public override int GetSpellpower(SpellModifiers mods)
    {
        return inner.GetSpellpower(AddSpellpowerMods(mods));
    }

    public override float GetSpeed(SpellModifiers mods)
    {
        return inner.GetSpeed(AddProjectileMods(mods));
    }

    public override float GetLifetime(SpellModifiers mods)
    {
        return inner.GetLifetime(AddProjectileMods(mods));
    }

    public override string GetProjectileTrajectory(SpellModifiers mods)
    {
        return inner.GetProjectileTrajectory(AddProjectileMods(mods));
    }

    public override int GetIcon()
    {
        return inner.GetIcon();
    }
}

public class DamageAmplifier : ModifierSpell
{
    public DamageAmplifier()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        damage_multiplier = properties["damage_multiplier"].ToObject<string>();
        mana_multiplier = properties["mana_multiplier"].ToObject<string>();
    }
}

public class SpeedAmplifier : ModifierSpell
{
    public SpeedAmplifier()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        speed_multiplier = properties["speed_multiplier"].ToObject<string>();
    }
}

public class Chaos : ModifierSpell
{
    public Chaos()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        damage_multiplier = properties["damage_multiplier"].ToObject<string>();
        projectile_trajectory = properties["projectile_trajectory"].ToObject<string>();
    }
}
public class Homing : ModifierSpell
{
    public Homing()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        mana_adder = properties["mana_adder"].ToObject<string>();
        damage_multiplier = properties["damage_multiplier"].ToObject<string>();
        projectile_trajectory = properties["projectile_trajectory"].ToObject<string>();
    }
}

public class RapidFire : ModifierSpell
{
    public RapidFire()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        damage_multiplier = properties["damage_multiplier"].ToObject<string>();
        speed_multiplier = properties["speed_multiplier"].ToObject<string>();
        mana_multiplier = properties["mana_multiplier"].ToObject<string>();
    }
}

public class Slug : ModifierSpell
{
    public Slug()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        damage_multiplier = properties["damage_multiplier"].ToObject<string>();
        speed_multiplier = properties["speed_multiplier"].ToObject<string>();
        mana_multiplier = properties["mana_multiplier"].ToObject<string>();
    }
}

public class Heavy : ModifierSpell
{
    public Heavy()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        damage_multiplier = properties["damage_multiplier"].ToObject<string>();
        speed_multiplier = properties["speed_multiplier"].ToObject<string>();
    }
}
