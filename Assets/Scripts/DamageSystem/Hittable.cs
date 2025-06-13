using UnityEngine;
using System;
using System.Collections.Generic;

public class Hittable
{
    public AudioClip damageSound = Resources.Load<AudioClip>("Sounds/Hurt");
    
    public enum Team { PLAYER, MONSTERS }
    public Team team;

    public int hp;
    public int max_hp;
    public int bonus_max_hp = 0;
    public float defense;

    public GameObject owner;

    RPNEvaluator rpnEval = new RPNEvaluator();
    Dictionary<string, int> variables = new Dictionary<string, int>();

    public void Damage(Damage damage)
    {
        SoundManager.instance.playSound(damageSound, owner.transform, 1f);

        damage.amount = (int) (damage.amount * defense);


        EventBus.Instance.DoDamage(owner.transform.position, damage, this);

        if (team == Team.PLAYER)
        {
            EventBus.Instance.DoTakeDamage();
        }

        hp -= damage.amount;
        if (hp <= 0)
        {
            hp = 0;
            OnDeath();
        }

    }

    public event Action OnDeath;

    public Hittable(int hp, float defense, Team team, GameObject owner)
    {
        this.hp = hp;
        this.max_hp = hp;
        this.team = team;
        this.owner = owner;
        this.defense = defense;
    }

    public void SetMaxHP(int max_hp)
    {
        float perc = this.hp * 1.0f / this.max_hp;
        this.max_hp = max_hp + this.bonus_max_hp;
        Debug.Log("Player max hp is: " + this.max_hp);
        this.hp = Mathf.RoundToInt(perc * max_hp);
    }

}
