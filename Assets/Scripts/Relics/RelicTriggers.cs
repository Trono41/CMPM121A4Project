using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Unity.Jobs;
using UnityEngine.Rendering.Universal;
using System.Diagnostics;
using System.Collections;

public class RelicTriggers : RelicPart
{

    protected RPNEvaluator rpn = new RPNEvaluator();
    protected Dictionary<string, int> variables = new Dictionary<string, int>();

    protected int amount;
    protected string until;
    protected PlayerController owner;
    protected bool applied;

    virtual public void Register(RelicEffects effect)
    {

    }

    virtual public void ApplyEffect()
    {

    }

    virtual public void RemoveEffect()
    {

    }

}

public class EnemyDeath : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public EnemyDeath(int sprite)
    {
        this.sprite = sprite;
        name = "EnemyDeath";
        EventBus.Instance.OnEnemyDeath += ApplyEffect;
    }

    override public string GetName()
    {
        return name;
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;

        if (effect.until != null)
        {
            if (effect.until == "cast-spell")
            {
                EventBus.Instance.OnCastSpell += RemoveEffect;
            }
            else if (effect.until == "move")
            {
                EventBus.Instance.OnMove += RemoveEffect;
            }
        }
    }

    override public void ApplyEffect()
    {
        effect.apply();
    }

    override public void RemoveEffect()
    {
        if (applied)
        {
            effect.remove();
            applied = false;
        }
    }

}

public class StandStill : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public StandStill(string amount, int sprite, PlayerController owner)
    {
        this.amount = rpn.Eval(amount, variables);
        this.sprite = sprite;
        name = "StandStill";
        EventBus.Instance.OnStandStill += StartTimer;
        this.owner = owner;

        StartTimer();
        //UnityEngine.Debug.Log("Coroutine Started!");
    }

    override public string GetName()
    {
        return name;
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;

        if (effect.until != null)
        {
            if (effect.until == "cast-spell")
            {
                EventBus.Instance.OnCastSpell += ResetTimer;
            }
            else if (effect.until == "move")
            {
                owner.unit.OnMove += ResetTimer;
            }
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(amount);
        ApplyEffect();
    }

    public void StartTimer()
    {
        CoroutineManager.Instance.Run(Timer());

    }

    public void ResetTimer()
    {
        RemoveEffect();
        CoroutineManager.Instance.Cancel(Timer());
        //UnityEngine.Debug.Log("Coroutine Restarted!");
    }

    override public void ApplyEffect()
    {
        effect.apply();
        applied = true;
    }

    override public void RemoveEffect()
    {
        //UnityEngine.Debug.Log("Spell cast!");
        if (applied)
        {
            effect.remove();
            applied = false;
            //UnityEngine.Debug.Log("Effect removed!");
        }
    }

}

public class TakeDamage : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public TakeDamage(int sprite)
    {
        this.sprite = sprite;
        name = "TakeDamage";
        EventBus.Instance.OnTakeDamage += ApplyEffect;
    }

    override public string GetName()
    {
        return name;
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;

        if (effect.until != null)
        {
            if (effect.until == "cast-spell")
            {
                EventBus.Instance.OnCastSpell += RemoveEffect;
            }
            else if (effect.until == "move")
            {
                EventBus.Instance.OnMove += RemoveEffect;
            }
        }
    }

    override public void ApplyEffect()
    {
        if (!applied)
        {
            //UnityEngine.Debug.Log("TakeDamage event sent.");
            effect.apply();
            applied = true;
            //UnityEngine.Debug.Log("Effect applied!");
        }
            
    }

    override public void RemoveEffect()
    {
        //UnityEngine.Debug.Log("Spell cast!");
        if (applied)
        {
            effect.remove();
            applied = false;
            //UnityEngine.Debug.Log("Effect removed!");
        }
    }
}

public class MaxMana : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public MaxMana(int sprite, PlayerController owner)
    {
        this.sprite = sprite;
        this.owner = owner;
        name = "MaxMana";
        EventBus.Instance.OnMaxMana += ApplyEffect;
    }

    override public string GetName()
    {
        return name;
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;

        if (effect.until != null)
        {
            if (effect.until == "cast-spell")
            {
                EventBus.Instance.OnCastSpell += RemoveEffect;
            }
            else if (effect.until == "move")
            {
                EventBus.Instance.OnMove += RemoveEffect;
            }
        }
    }

    override public void ApplyEffect()
    {
        if (!applied)
        {
            //UnityEngine.Debug.Log("OnMaxMana event sent.");
            effect.apply();
            applied = true;
            //UnityEngine.Debug.Log("Effect applied!");
        }
    }

    override public void RemoveEffect()
    {
        //UnityEngine.Debug.Log("Spell cast!");
        if (applied)
        {
            effect.remove();
            applied = false;
            //UnityEngine.Debug.Log("Effect removed!");
        }
    }

}

public class SpellDrop : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public SpellDrop(int sprite)
    {
        this.sprite = sprite;
        name = "SpellDrop";

        EventBus.Instance.OnSpellDrop += ApplyEffect;
        //UnityEngine.Debug.Log("SpellDrop event sent.");
    }

    override public string GetName()
    {
        return name;
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;
    }

    override public void ApplyEffect()
    {
        effect.apply();
        //UnityEngine.Debug.Log("Permanent Spellpower Added!");
    }
}

public class WaveStart : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public WaveStart(int sprite)
    {
        this.sprite = sprite;
        name = "WaveStart";
        EventBus.Instance.OnWaveStart += ApplyEffect;
        //UnityEngine.Debug.Log("WaveStart event sent");
    }

    override public string GetName()
    {
        return name;
    }

    public override void Register(RelicEffects effect)
    {
        this.effect = effect;
    }

    public override void ApplyEffect()
    {
        effect.apply();
        //UnityEngine.Debug.Log("In WaveStart() trigger: Applying effect!");
        RemoveEffect();
    }

    public override void RemoveEffect()
    {
        if (effect.until == "duration")
        {
            CoroutineManager.Instance.StartCoroutine(Duration());
        }
    }

    IEnumerator Duration()
    {
        //UnityEngine.Debug.Log("Starting effect duration timer");
        yield return new WaitForSeconds(30);
        effect.remove();
    }
}

public class WaveComplete : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public WaveComplete(int sprite)
    {
        this.sprite = sprite;
        name = "WaveComplete";
        EventBus.Instance.OnWaveEnd += ApplyEffect;
        //UnityEngine.Debug.Log("WaveComplete event sent");
    }

    override public string GetName()
    {
        return name;
    }

    public override void Register(RelicEffects effect)
    {
        this.effect = effect;
    }

    override public void ApplyEffect()
    {
        effect.apply();
        //UnityEngine.Debug.Log("MaxHp added");
    }
}

public class EnemyDamage : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public EnemyDamage(int sprite)
    {
        this.sprite = sprite;
        name = "EnemyDamage";
        EventBus.Instance.OnEnemyTakeDamage += ApplyEffect;
        //UnityEngine.Debug.Log("EnemyDamage event sent");
    }

    override public string GetName()
    {
        return name;
    }

    public override void Register(RelicEffects effect)
    {
        this.effect = effect;
    }

    public override void ApplyEffect()
    {
        effect.apply();
        //UnityEngine.Debug.Log("Regained mana");
    }
}
