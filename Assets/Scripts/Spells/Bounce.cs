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
