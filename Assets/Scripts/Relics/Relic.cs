using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Relic : Piece
{

    string name;
    int sprite;
    string description;
    PlayerController owner;

    RelicTriggers trigger;
    RelicEffects effect;

    public Relic(string name, int sprite, string description, RelicTriggers trigger, RelicEffects effect) 
    {
        this.name = name;
        this.sprite = sprite;
        this.description = description;
        this.trigger = trigger;
        this.effect = effect;
    }

    public string GetName()
    {
        return name;
    }


    public int GetIcon()
    {
        return sprite;
    }

    public string GetDescription()
    {
        return description;
    }

    public void SetOwner(PlayerController owner)
    {
        this.owner = owner;
    }
    
}