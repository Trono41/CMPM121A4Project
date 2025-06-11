using System.Collections.Generic;
using UnityEngine;

public class Craftable : MonoBehaviour
{

    public GameObject[] mod_spells;
    public GameObject base_spell;

    public GameObject trigger;
    public GameObject effect;

    private int craftable_pieces;

    void Start()
    {

    }

    void Update()
    {

    }

    public void AddPiece()
    {
        craftable_pieces++;
    }

    public int GetNumberOfPieces()
    {
        return craftable_pieces;
    }

}
