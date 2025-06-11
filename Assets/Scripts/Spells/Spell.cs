using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;

public class Spell : Piece
{
    public float last_cast;
    public SpellCaster owner;
    public Hittable.Team team;

    public string name;
    public string description;
    public int icon;

    // Modifiable Values
    public string damage;
    public Damage.Type damage_type;
    public string mana_cost;
    public string cooldown;
    public string duration;
    public Dictionary<string, string> projectile = new Dictionary<string, string>();

    public RPNEvaluator rpn = new RPNEvaluator();

    public bool isModifier = false;

    public Spell()
    {

    }

    virtual public void SetInnerSpell(Spell inner)
    {

    }

    virtual public Spell GetInnerSpell()
    {
        return this;
    }


    public bool IsModifier()
    {
        return isModifier;
    }

    public void AddChild(string name)
    {
        return;
    }

    virtual public void SetProperties(JObject properties)
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
        return;
    }

    public void SetOwner(SpellCaster owner)
    {
        this.owner = owner;
    }


    public int GetManaCost()
    {
        return GetManaCost(new SpellModifiers());
    }

    public int GetDamage()
    {
        return GetDamage(new SpellModifiers());
    }

    public float GetCooldown()
    {
        return GetCooldown(new SpellModifiers());
    }

    public int GetSpellpower()
    {
        return GetSpellpower(new SpellModifiers());
    }

    public float GetSpeed()
    {
        return GetSpeed(new SpellModifiers());
    }

    public float GetLifetime()
    {
        return GetLifetime(new SpellModifiers());
    }

    public string GetProjectileTrajectory()
    {
        return GetProjectileTrajectory(new SpellModifiers());
    }

    //

    public virtual string GetName()
    {
        return name;
    }

    public virtual int GetIcon()
    {
        return icon;
    }
    
    public virtual string GetDescription() 
    { 
        return description;
    }

    public virtual int GetManaCost(SpellModifiers mods)
    {
        return 0;
    }

    public virtual int GetDamage(SpellModifiers mods)
    {
        return 0;
    }

    public virtual float GetCooldown(SpellModifiers mods)
    {
        return 0;
    }

    public virtual int GetSpellpower(SpellModifiers mods)
    {
        return owner.GetSpellPower();
    }

    public virtual float GetSpeed(SpellModifiers mods)
    {
        return 0;
    }

    public virtual float GetLifetime(SpellModifiers mods)
    {
        return 0;
    }

    public virtual string GetProjectileTrajectory(SpellModifiers mods)
    {
        return "";
    }

    //

    public bool IsReady()
    {
        return (last_cast + GetCooldown() < Time.time);
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Vector3 direction, Hittable.Team team)
    {
        return Cast(where, target, direction, team, new SpellModifiers());
    }

    public virtual IEnumerator Cast(Vector3 where, Vector3 target, Vector3 direction, Hittable.Team team, SpellModifiers mods)
    {
        this.team = team;
        GameManager.Instance.projectileManager.CreateProjectile(0, "straight", where, direction, 15f, GetOnHit(mods));
        yield return new WaitForEndOfFrame();
    }

    virtual public Action<Hittable, Vector3> GetOnHit(SpellModifiers mods)
    {

        Damage dmg = new Damage(GetDamage(mods), damage_type);

        void OnHit(Hittable other, Vector3 impact)
        {
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
            }
        }

        return OnHit;
    }

}





