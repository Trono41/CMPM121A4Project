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
    RelicBuilder relic_builder;
    PlayerController player;

    public void Start()
    {
        relic_data = ReadRelicData();
        BuildRelicObjects(relic_data);
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

    public RelicTriggers BuildTrigger(JObject trigger_object)
    {
        string trigger_type = trigger_object["type"].ToObject<string>();
        string amount;

        if (trigger_type == "take-damage")
        {
            UnityEngine.Debug.Log("Attempting to build take-damage trigger");
            return new TakeDamage();
        }
        else if (trigger_type == "stand-still")
        {
            amount = trigger_object["amount"].ToObject<string>();
            return new StandStill(amount, player);
        }
        else if (trigger_type == "on-max-mana")
        {
            return new MaxMana(player);
        }
        else if (trigger_type == "on-kill")
        {
            return new EnemyDeath();
        }
        else if (trigger_type == "on-spell-drop")
        {
            UnityEngine.Debug.Log("Attempting to build spell-drop effect");
            return new SpellDrop();
        }
        else if (trigger_type == "wave-start")
        {
            UnityEngine.Debug.Log("Attempting to build wave-start trigger");
            return new WaveStart();
        }
        else if (trigger_type == "wave-complete")
        {
            UnityEngine.Debug.Log("Attempting to build wave-complete trigger");
            return new WaveComplete();
        }
        else if (trigger_type == "enemy-damage")
        {
            UnityEngine.Debug.Log("Attempting to build enemy-damage trigger");
            return new EnemyDamage();
        }

        return new RelicTriggers();
    }

    public RelicEffects BuildEffect(JObject effect_object)
    {
        string effect_type = effect_object["type"].ToObject<string>();
        string amount;
        string until;

        if (effect_type == "gain-mana")
        {
            amount = effect_object["amount"].ToObject<string>();
            return new GainMana(amount, player);
        }
        else if (effect_type == "gain-spellpower")
        {
            UnityEngine.Debug.Log("Attempting to build gain-spellpower effect");
            amount = effect_object["amount"].ToObject<string>();
            until = effect_object["until"].ToObject<string>();
            return new GainSpellPower(amount, until, player);
        }
        else if (effect_type == "gain-temp-spellpower")
        {
            UnityEngine.Debug.Log("Attempting to build gain-spellpower effect");
            amount = effect_object["amount"].ToObject<string>();
            until = effect_object["until"].ToObject<string>();
            return new GainTempSpellPower(amount, until, player);
        }
        else if (effect_type == "gain-defense")
        {
            UnityEngine.Debug.Log("Attempting to build gain-defense effect");
            amount = effect_object["amount"].ToObject<string>();
            until = effect_object["until"].ToObject<string>();
            return new GainDefense(amount, until, player);
        }
        else if (effect_type == "regain-hp")
        {
            UnityEngine.Debug.Log("Attempting to build regain-hp effect");
            amount = effect_object["amount"].ToObject<string>();
            return new RegainHP(amount, player);
        }
        else if (effect_type == "gain-max-hp")
        {
            UnityEngine.Debug.Log("Attempting to build gain-max-hp effect");
            amount = effect_object["amount"].ToObject<string>();
            return new GainMaxHP(amount, player);
        }

        return new RelicEffects();
    }

}
