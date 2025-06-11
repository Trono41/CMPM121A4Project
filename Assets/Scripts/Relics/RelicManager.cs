using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Diagnostics;

public class RelicManager : MonoBehaviour
{

    private static RelicManager theInstance;
    public static RelicManager Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = GameObject.FindFirstObjectByType<RelicManager>();
            return theInstance;
        }
    }

    private List<Relic> all_relics = new List<Relic>();

    JArray relic_data;
    List<JObject> relic_objects;
    List<RelicTriggers> relic_triggers = new List<RelicTriggers>();
    List<RelicEffects> relic_effects = new List<RelicEffects>();
    RelicBuilder relic_builder;
    PlayerController player;

    public void Start()
    {
        relic_data = ReadRelicData();
        BuildRelicObjects(relic_data);
        BuildTriggers();
        BuildEffects();
        relic_builder = new RelicBuilder();
        player = GameObject.FindFirstObjectByType<PlayerController>();
    }

    public JArray ReadRelicData()
    {
        var relictext = Resources.Load<TextAsset>("relics");

        return JArray.Parse(relictext.text);
    }

    public void BuildRelicObjects(JArray relic_data)
    {
        relic_objects = new List<JObject>();

        foreach (JObject relic_object in relic_data)
        {
            relic_objects.Add(relic_object);
            //UnityEngine.Debug.Log("Relic: " + relic_object["name"]);
        }
    }

    public Relic BuildRelic()
    {
        int i = UnityEngine.Random.Range(0, relic_objects.Count);
        return relic_builder.BuildRelic(relic_objects[i]);
    }

    public RelicPart GetRelicPart()
    {
        return relic_triggers.Find(x => x.GetName() == "EnemyDeath");
    }

    public RelicPart GetTrigger()
    {
        return relic_triggers.Find(x => x.GetName() == "EnemyDeath");
    }

    public RelicPart GetEffect()
    {
        return relic_effects.Find(x => x.GetName() == "GainMana");
    }

    public void BuildTriggers()
    {
        foreach (var relic_object in relic_objects)
        {
            relic_triggers.Add(BuildTrigger(relic_object["trigger"].ToObject<JObject>()));
        }
    }

    public void BuildEffects()
    {
        foreach (var relic_object in relic_objects)
        {
            relic_effects.Add(BuildEffect(relic_object["effect"].ToObject<JObject>()));
        }
    }

    public RelicTriggers BuildTrigger(JObject trigger_object)
    {
        string trigger_type = trigger_object["type"].ToObject<string>();
        string amount;
        int sprite = trigger_object["sprite"].ToObject<int>();

        if (trigger_type == "take-damage")
        {
            //UnityEngine.Debug.Log("Attempting to build take-damage trigger");
            return new TakeDamage(sprite);
        }
        else if (trigger_type == "stand-still")
        {
            amount = trigger_object["amount"].ToObject<string>();
            return new StandStill(amount, sprite, player);
        }
        else if (trigger_type == "on-max-mana")
        {
            return new MaxMana(sprite, player);
        }
        else if (trigger_type == "on-kill")
        {
            return new EnemyDeath(sprite);
        }
        else if (trigger_type == "on-spell-drop")
        {
            //UnityEngine.Debug.Log("Attempting to build spell-drop effect");
            return new SpellDrop(sprite);
        }
        else if (trigger_type == "wave-start")
        {
            //UnityEngine.Debug.Log("Attempting to build wave-start trigger");
            return new WaveStart(sprite);
        }
        else if (trigger_type == "wave-complete")
        {
            //UnityEngine.Debug.Log("Attempting to build wave-complete trigger");
            return new WaveComplete(sprite);
        }
        else if (trigger_type == "enemy-damage")
        {
            //UnityEngine.Debug.Log("Attempting to build enemy-damage trigger");
            return new EnemyDamage(sprite);
        }

        return new RelicTriggers();
    }

    public RelicEffects BuildEffect(JObject effect_object)
    {
        string effect_type = effect_object["type"].ToObject<string>();
        string amount;
        int sprite = effect_object["sprite"].ToObject<int>();
        string until;

        if (effect_type == "gain-mana")
        {
            amount = effect_object["amount"].ToObject<string>();
            return new GainMana(amount, sprite, player);
        }
        else if (effect_type == "gain-spellpower")
        {
            //UnityEngine.Debug.Log("Attempting to build gain-spellpower effect");
            amount = effect_object["amount"].ToObject<string>();
            until = effect_object["until"].ToObject<string>();
            return new GainSpellPower(amount, sprite, until, player);
        }
        else if (effect_type == "gain-temp-spellpower")
        {
            //UnityEngine.Debug.Log("Attempting to build gain-spellpower effect");
            amount = effect_object["amount"].ToObject<string>();
            until = effect_object["until"].ToObject<string>();
            return new GainTempSpellPower(amount, sprite, until, player);
        }
        else if (effect_type == "gain-defense")
        {
            //UnityEngine.Debug.Log("Attempting to build gain-defense effect");
            amount = effect_object["amount"].ToObject<string>();
            until = effect_object["until"].ToObject<string>();
            return new GainDefense(amount, sprite, until, player);
        }
        else if (effect_type == "regain-hp")
        {
            //UnityEngine.Debug.Log("Attempting to build regain-hp effect");
            amount = effect_object["amount"].ToObject<string>();
            return new RegainHP(amount, sprite, player);
        }
        else if (effect_type == "gain-max-hp")
        {
            //UnityEngine.Debug.Log("Attempting to build gain-max-hp effect");
            amount = effect_object["amount"].ToObject<string>();
            return new GainMaxHP(amount, sprite, player);
        }

        return new RelicEffects();
    }

}

public class RelicPart
{

    protected int sprite;
    protected string name;

    virtual public int GetIcon()
    {
        return sprite;
    }

    virtual public string GetName()
    {
        return name;
    }
}
