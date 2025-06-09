using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;

public class Bounce : ModifierSpell
{
    RPNEvaluator rpnEval = new RPNEvaluator();
    Dictionary<string, int> variables = new Dictionary<string, int>();

    public Bounce()
    {

    }

    override public void SetProperties(JObject properties)
    {
        isModifier = true;

        name = properties["name"].ToString();
        description = properties["description"].ToObject<string>();
        mana_multiplier = properties["mana_multiplier"].ToObject<string>();
    }

    public override Action<Hittable, Vector3> GetOnHit(SpellModifiers mods)
    {

        Damage dmg = new Damage(GetDamage(mods), Damage.Type.ARCANE);

        void OnHit(Hittable other, Vector3 impact)
        {
            if (other.team != team)
            {
                other.Damage(dmg);

                // create new base version of spell that goes in a cone opposite of the enemy
                variables["power"] = owner.GetSpellPower();
                int speed = rpn.Eval(inner.projectile["speed"], variables);
                int sprite = int.Parse(inner.projectile["sprite"]);

                this.team = Hittable.Team.PLAYER;

                float randomAngle = UnityEngine.Random.Range(0f, 360f);

                Vector3 rangleAngleVector = new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
                GameManager.Instance.projectileManager.CreateProjectile(sprite, GetProjectileTrajectory(mods), Vector3.Lerp(other.owner.transform.position, GameManager.Instance.player.transform.position, 0.2f), 
                        Vector3.Lerp(other.owner.transform.position, GameManager.Instance.player.transform.position, 0.1f), GetSpeed(mods), 
                        (Hittable other, Vector3 impact) => {
                            Damage dmg = new Damage(GetDamage(mods), Damage.Type.ARCANE);
                        });   
                
            }
        }

        return OnHit;
    }

    public override IEnumerator Cast(Vector3 where, Vector3 target, Vector3 direction, Hittable.Team team, SpellModifiers mods)
    {

        variables["power"] = owner.GetSpellPower();
        int speed = rpn.Eval(inner.projectile["speed"], variables);
        int sprite = int.Parse(inner.projectile["sprite"]);

        this.team = team;
        //UnityEngine.Debug.Log("Spellpower currently: " + variables["power"]);
        GameManager.Instance.projectileManager.CreateProjectile(sprite, GetProjectileTrajectory(mods), where, direction, GetSpeed(mods), GetOnHit(mods));
        //UnityEngine.Debug.Log("Spell made!");
        yield return new WaitForEndOfFrame();
    }

    /*override public Action<Hittable, Vector3> GetOnHit(SpellModifiers mods)
    {
        void OnHit(Hittable other, Vector3 impact)
        {
            inner.GetOnHit(mods);
            CoroutineManager.Instance.StartCoroutine(inner.Cast());
        }

        return OnHit;
    }*/
}
