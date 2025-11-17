using System.Collections;
using System.Collections.Generic;
using Classes;
using UnityEngine;
using Utilities;

public class EntityData : MonoBehaviour
{
    public EntityType type;
    public Entity entity;
    void Awake()
    {
        if (type == EntityType.Enemy)
        {
            entity = new Classes.Enemy(this.gameObject);
        }
        else
        {
            entity = new Classes.Hero(this.gameObject);
        }
    }

    void Update()
    {
        
    }
}
