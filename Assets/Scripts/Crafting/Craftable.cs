using System.Collections.Generic;
using UnityEngine;

public interface Craftable<T> where T : MonoBehaviour
{
    void Craft(T piece);
    void Start();
    void Update();
}
