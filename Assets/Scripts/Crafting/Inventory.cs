using System.Collections.Generic;
using UnityEngine;

public class Inventory<T>
{

    List<T> stored_items = new List<T>();

    public Inventory()
    {

    }

    public void Add(T item)
    {
        stored_items.Add(item);
    }

    public void Remove()
    {

    }
}
