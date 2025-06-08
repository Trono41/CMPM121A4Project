using UnityEngine;

public class Piece<T>
{
    T item;

    public Piece(T item)
    {
        this.item = item;
    }
}
