using System;
using UnityEngine;

public class Piece<T>
{
    T item;

    public Piece(T item)
    {
        this.item = item;
    }

    public T GetItem()
    {
        return item;
    }

    public Type Type()
    {
        return item.GetType();
    }
}
