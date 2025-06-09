using UnityEngine;
using System;

public class EventBus 
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action OnWaveStart;
    public event Action OnWaveEnd;
    public event Action OnThirdWave;

    public void DoWaveStart()
    {
        OnWaveStart?.Invoke();
    }

    public void DoWaveEnd()
    {
        OnWaveEnd?.Invoke();
    }

    public void DoBuildRelics()
    {
        OnThirdWave?.Invoke();
    }

    //RelicTrigger Events

    public event Action<Vector3, Damage, Hittable> OnDamage;
    public event Action OnEnemyTakeDamage;
    public event Action OnTakeDamage;
    public event Action OnEnemyDeath;
    public event Action OnStandStill;
    public event Action OnMaxMana;
    public event Action OnCastSpell;
    public event Action OnMove;
    public event Action OnSpellDrop;

    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
        DoEnemyTakeDamage();
    }

    public void DoEnemyTakeDamage()
    {
        OnEnemyTakeDamage?.Invoke();
    }

    public void DoTakeDamage()
    {
        OnTakeDamage?.Invoke();
    }

    public void DoEnemyDeath()
    {
        OnEnemyDeath?.Invoke();
    }

    public void DoStandStill()
    {
        OnStandStill?.Invoke();
    }

    public void DoMaxMana()
    {
        OnMaxMana?.Invoke();
    }

    public void DoCastSpell()
    {
        OnCastSpell?.Invoke();
    }

    public void DoOnMove()
    {
        OnMove?.Invoke();
    }

    public void DoSpellDrop()
    {
        OnSpellDrop?.Invoke();    
    }

}
