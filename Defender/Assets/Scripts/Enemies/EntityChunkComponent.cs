using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityChunkComponent : MonoBehaviour
{
    private Chunk owner;
    public void SetOwner(Chunk chunk)
    {
        owner = chunk;
    }

    public Chunk GetOwner()
    {
        return owner;
    }

}
