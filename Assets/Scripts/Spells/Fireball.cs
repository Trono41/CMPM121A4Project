using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.U2D;
using System;

public class Fireball : Spell
{
    RPNEvaluator rpnEval = new RPNEvaluator();
    string N;
    string secondary_damage;
    Dictionary<string, string> secondary_projectile = new Dictionary<string, string>();
    Dictionary<string, int> variables = new Dictionary<string, int>();

    public Fireball()
    {

    }

    public override void SetProperties(JObject properties)
    {
        name = properties["name"].ToString();
        icon = properties["icon"].ToObject<int>();
        description = properties["description"].ToObject<string>();
        N = properties["N"].ToString();
        damage = properties["damage"]["amount"].ToString();
        secondary_damage = properties["secondary_damage"].ToString();
        damage_type = Damage.TypeFromString(properties["damage"]["type"].ToString());
        mana_cost = properties["mana_cost"].ToString();
        cooldown = properties["cooldown"].ToString();
        projectile["trajectory"] = properties["projectile"]["trajectory"].ToString();
        projectile["speed"] = properties["projectile"]["speed"].ToString();
        projectile["sprite"] = properties["projectile"]["sprite"].ToString();
        secondary_projectile["trajectory"] = properties["secondary_projectile"]["trajectory"].ToString();
        secondary_projectile["speed"] = properties["secondary_projectile"]["speed"].ToString();
        secondary_projectile["lifetime"] = properties["secondary_projectile"]["lifetime"].ToString();
        secondary_projectile["sprite"] = properties["secondary_projectile"]["sprite"].ToString();
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

        int init_damage = rpnEval.Eval(damage, variables);
        int new_damage = ValueModifier<int>.ApplyModifiers(mods.damage, init_damage);

        return new_damage;
    }

    public int GetSecondaryDamage()
    {
        variables["power"] = owner.GetSpellPower();
        return rpn.Eval(secondary_damage, variables);
    }

    public override int GetManaCost(SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();

        int init_mana_cost = rpnEval.Eval(mana_cost, variables);
        int new_mana_cost = ValueModifier<int>.ApplyModifiers(mods.mana_cost, init_mana_cost);

        return new_mana_cost;
    }

    public override float GetCooldown(SpellModifiers mods)
    {
        float cd = float.Parse(cooldown);

        float new_cd = ValueModifier<float>.ApplyModifiers(mods.cooldown, cd);

        return new_cd;
    }

    public override float GetSpeed(SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();

        int init_speed = rpnEval.Eval(projectile["speed"], variables);
        float new_speed = ValueModifier<float>.ApplyModifiers(mods.speed, init_speed);

        return new_speed;
    }

    public override float GetLifetime(SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();

        float init_lifetime = rpnEval.EvalFloat(projectile["lifetime"], variables);
        float new_lifetime = ValueModifier<float>.ApplyModifiers(mods.lifetime, init_lifetime);

        return new_lifetime;
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Vector3 d, Hittable.Team team, SpellModifiers mods)
    {
        variables["power"] = owner.GetSpellPower();
        int speed = rpn.Eval(projectile["speed"], variables);
        int sprite = int.Parse(projectile["sprite"]);
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(sprite, projectile["trajectory"], where, d, GetSpeed(mods), GetOnHit(mods));
        yield return new WaitForEndOfFrame();
    }

    public override Action<Hittable, Vector3> GetOnHit(SpellModifiers mods)
    {

        Damage dmg = new Damage(GetDamage(mods), damage_type);

        void OnHit(Hittable other, Vector3 impact)
        {
            variables["power"] = owner.GetSpellPower();
            int SecondaryProjectileCount = rpn.Eval(N, variables);
            int speed = rpn.Eval(secondary_projectile["speed"], variables);
            int sprite = int.Parse(secondary_projectile["sprite"]);
            float lifetime = rpn.EvalFloat(secondary_projectile["lifetime"], variables);

            if (other.team != team)
            {
                var ec = other.owner.GetComponent<EnemyController>();
                if (ec.resistance == damage_type)
                {
                    dmg.amount /= 2;
                }
                if (ec.weakness == damage_type)
                {
                    dmg.amount *= 2;
                }
                other.Damage(dmg);
                for (int i = 0; i < SecondaryProjectileCount; i++)
                {
                    float launch_angle = ((Mathf.PI * 2) / SecondaryProjectileCount) * i;
                    Vector3 launch_direction = new Vector3(Mathf.Cos(launch_angle), Mathf.Sin(launch_angle), 0);
                    GameManager.Instance.projectileManager.CreateProjectile(sprite, secondary_projectile["trajectory"], impact, launch_direction, speed, OnSecondHit, lifetime);
                }
            }
        }

        return OnHit;
    }

    public void OnSecondHit(Hittable other, Vector3 impact)
    {
        Damage secondary = new Damage(GetSecondaryDamage(), damage_type);
        if (other.team != team)
        {
            var ec = other.owner.GetComponent<EnemyController>();
            if (ec.resistance == damage_type)
            {
                secondary.amount /= 2;
            }
            if (ec.weakness == damage_type)
            {
                secondary.amount *= 2;
            }
            other.Damage(secondary);
        }
    }
}
