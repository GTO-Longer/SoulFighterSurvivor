using Classes.Entities;
using Classes;
using UnityEngine;
using Utilities;

namespace DataManagement
{
    public class EntityData : MonoBehaviour
    {
        public EntityType type;
        public Entity entity;

        public void EntityInitialization()
        {
            if (type == EntityType.Enemy)
            {
                entity = new Enemy(gameObject, Team.Enemy);
            }
            else
            {
                entity = new Hero(gameObject, Team.Hero, "Ahri");
            }
        }
    }
}