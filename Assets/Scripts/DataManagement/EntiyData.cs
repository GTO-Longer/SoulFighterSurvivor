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
        public string heroName;

        private void Awake()
        {
            if (type == EntityType.Enemy)
            {
                entity = new Enemy(this.gameObject);
            }
            else
            {
                entity = new Hero(this.gameObject, heroName);
            }
        }
    }
}