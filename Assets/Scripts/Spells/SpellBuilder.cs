using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System;


public class SpellBuilder
{

    List<string> spellnames = new List<string>(); // List of all spells from the JSON file
    JObject properties;
    string name;

    //HashSet<int> used = new HashSet<int>();

    private Spell MakeSpell(string s)
    {
        if (s == "random")
        {
            int val = UnityEngine.Random.Range(0, spellnames.Count - 1);
            name = spellnames[val];
        }

        //while (used.Contains(val))
        //{
            //val = UnityEngine.Random.Range(0, spells.Count - 1);
        //}

        //used.Add(val);
        

        if (name == "arcane_bolt")
            return new ArcaneBolt();
        else if (name == "magic_missile")
            return new MagicMissile();
        else if (name == "arcane_blast")
            return new ArcaneBlast();
        else if (name == "arcane_spray")
            return new ArcaneSpray();

        else if (name == "damage_amp")
        {
            return new DamageAmplifier();
        }
        else if (name == "speed_amp")
        {
            return new SpeedAmplifier();
        }
        else if (name == "chaos")
        {
            return new Chaos();
        }
        else if (name == "homing")
        {
            return new Homing();
        }

        else if (name == "doubler")
        {
            return new Doubler();
        }
        else if (name == "splitter")
        {
            return new Splitter();
        }

        return new ArcaneBolt();
    }

    // Creates a Spell object and assigns it the corresponding attributes to the JSON file

    public Spell BuildSpells(string mod, string name, SpellCaster owner)
    {

        ModifierSpell mod_spell = new Splitter();

        JObject jobject = properties[mod].Value<JObject>();
        mod_spell.SetProperties(jobject);
        mod_spell.SetOwner(owner);

        Spell inner = new ArcaneBolt();

        JObject jobject1 = properties[name].Value<JObject>();
        inner.SetProperties(jobject1);
        inner.SetOwner(owner);

        mod_spell.AddChild(inner.GetName());
        mod_spell.SetInnerSpell(inner);

        UnityEngine.Debug.Log(mod_spell);
        UnityEngine.Debug.Log(inner);

        return mod_spell;
    }

    // Building random spells

    public Spell BuildSpell(SpellCaster owner)
    {
        Spell spell = MakeSpell("random");

        UnityEngine.Debug.Log(name);

        JObject jobject = properties[name].Value<JObject>();
        spell.SetProperties(jobject);
        spell.SetOwner(owner);

        UnityEngine.Debug.Log(jobject);

        if (spell.IsModifier())
        {
            ModifierSpell mod_spell = (ModifierSpell) spell;
            Spell inner = BuildSpell(owner);
            mod_spell.AddChild(inner.GetName());
            mod_spell.SetInnerSpell(inner);

            return mod_spell;
        }

        return spell;
    }

    public Spell BuildSpell(string name, SpellCaster owner)
    {
        Spell spell = MakeSpell(name);

        //UnityEngine.Debug.Log(name);

        JObject jobject = properties[name].Value<JObject>();
        spell.SetProperties(jobject);
        spell.SetOwner(owner);

        UnityEngine.Debug.Log(jobject);

        if (spell.IsModifier())
        {
            ModifierSpell mod_spell = (ModifierSpell)spell;
            Spell inner = BuildSpell(owner);
            mod_spell.AddChild(inner.GetName());
            mod_spell.SetInnerSpell(inner);

            return mod_spell;
        }

        return spell;
    }

    public SpellBuilder()
    {
        var spelltext = Resources.Load<TextAsset>("spells");

        properties = JObject.Parse(spelltext.text);
        foreach (var a in properties)
        {
            spellnames.Add(a.Key);
        }
    }

    void Start()
    {
        
    }

}
