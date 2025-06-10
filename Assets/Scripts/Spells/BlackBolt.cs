using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;

public class BlackBolt : Spell
{
    RPNEvaluator rpnEval = new RPNEvaluator();
    Dictionary<string, int> variables = new Dictionary<string, int>();

    public BlackBolt()
    {

    }

    public override void SetProperties(JObject properties)
    {
        name = properties["name"].ToString();
        icon = properties["icon"].ToObject<int>();
        description = properties["description"].ToObject<string>();
        damage = properties["damage"]["amount"].ToString();
        damage_type = Damage.TypeFromString(properties["damage"]["type"].ToString());
        mana_cost = properties["mana_cost"].ToString();
        cooldown = properties["cooldown"].ToString();
        projectile["trajectory"] = properties["projectile"]["trajectory"].ToString();
        projectile["speed"] = properties["projectile"]["speed"].ToString();
        projectile["sprite"] = properties["projectile"]["sprite"].ToString();
    }

    public override string GetName()
    {
        return name;
    }

    public override int GetIcon()
    {
        return icon;
    }

    public override int GetDamage(SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();
        variables["wave"] = GameManager.Instance.GetWave();

        int init_damage = rpnEval.Eval(damage, variables);
        int new_damage = ValueModifier<int>.ApplyModifiers(mods.damage, init_damage);

        return new_damage;
    }

    public override int GetManaCost(SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();

        int init_mana_cost = rpnEval.Eval(mana_cost, variables);
        int new_mana_cost = ValueModifier<int>.ApplyModifiers(mods.mana_cost, init_mana_cost);

        return new_mana_cost;
    }

    public override float GetSpeed(SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();

        int init_speed = rpnEval.Eval(projectile["speed"], variables);
        float new_speed = ValueModifier<float>.ApplyModifiers(mods.speed, init_speed);

        return new_speed;
    }


    public override float GetCooldown(SpellModifiers mods)
    {
        float cd = float.Parse(cooldown);

        float new_cd = ValueModifier<float>.ApplyModifiers(mods.cooldown, cd);

        return new_cd;
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Vector3 direction, Hittable.Team team, SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();
        int speed = rpn.Eval(projectile["speed"], variables);
        int sprite = int.Parse(projectile["sprite"]);
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(sprite, projectile["trajectory"], where, direction, GetSpeed(mods), GetOnHit(mods));
        yield return new WaitForEndOfFrame();
    }
}
