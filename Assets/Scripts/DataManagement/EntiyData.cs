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
                var heroName = "";
                if (PlayerData.Instance == null || PlayerData.Instance.heroName == null || PlayerData.Instance.heroName == "")
                {
                    // 测试用英雄
                    heroName = "Yasuo";
                }
                else
                {
                    heroName = PlayerData.Instance.heroName;
                }
                
                entity = new Hero(gameObject, Team.Hero, heroName);
            }
        }
    }
}