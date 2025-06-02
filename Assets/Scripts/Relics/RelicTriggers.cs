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

public class RelicTriggers
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

    public EnemyDeath()
    {
        EventBus.Instance.OnEnemyDeath += ApplyEffect;
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;
    }

    override public void ApplyEffect()
    {
        effect.apply();
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

    public StandStill(string amount, PlayerController owner)
    {
        this.amount = rpn.Eval(amount, variables);
        EventBus.Instance.OnStandStill += StartTimer;
        this.owner = owner;

        StartTimer();
        UnityEngine.Debug.Log("Coroutine Started!");
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;
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
        UnityEngine.Debug.Log("Coroutine Restarted!");
    }

    override public void ApplyEffect()
    {
        effect.apply();
        applied = true;
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

    override public void RemoveEffect()
    {
        UnityEngine.Debug.Log("Spell cast!");
        if (applied)
        {
            effect.remove();
            applied = false;
            UnityEngine.Debug.Log("Effect removed!");
        }
    }

}

public class TakeDamage : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public TakeDamage()
    {
        EventBus.Instance.OnTakeDamage += ApplyEffect;
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
        UnityEngine.Debug.Log("TakeDamage event sent.");
        effect.apply();
        applied = true;
        UnityEngine.Debug.Log("Effect applied!");
    }

    override public void RemoveEffect()
    {
        UnityEngine.Debug.Log("Spell cast!");
        if (applied)
        {
            effect.remove();
            applied = false;
            UnityEngine.Debug.Log("Effect removed!");
        }
    }
}

public class MaxMana : RelicTriggers
{
    RelicEffects effect = new RelicEffects();

    public MaxMana(string amount, PlayerController owner)
    {
        this.amount = rpn.Eval(amount, variables);
        this.owner = owner;
        EventBus.Instance.OnMaxMana += ApplyEffect;
    }

    override public void Register(RelicEffects effect)
    {
        this.effect = effect;
    }

    override public void ApplyEffect()
    {
        effect.apply();
        if (effect.until != null)
        {
            if (effect.until == "cast-spell")
            {
                EventBus.Instance.OnCastSpell += effect.remove;
            }
            else if (effect.until == "move")
            {
                EventBus.Instance.OnMove += effect.remove;
            }
        }
    }
}
