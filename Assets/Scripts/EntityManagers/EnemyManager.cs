using Classes.Entities;
using DataManagement;
using Factories;
using UnityEngine;


namespace EntityManagers
{
    public class EnemyManager : MonoBehaviour
    {
        public Enemy enemy;

        /// <summary>
        /// 初始化敌人数据
        /// </summary>
        public void EnemyDataInitialization()
        {
            var entityData = GetComponent<EntityData>();
            entityData.EntityInitialization();
            enemy = (Enemy)entityData.entity;
            enemy.target = HeroManager.hero;
        }

        private void Update()
        {
            if (enemy != null)
            {
                enemy.Move();
                enemy.Attack();
            }
        }

        /// <summary>
        /// 清除该敌人数据
        /// </summary>
        public void ClearEnemyData()
        {
            enemy.gameObject.SetActive(false);
            
            // 解除玩家对敌人的锁定
            if (Equals(HeroManager.hero.target.Value, enemy))
            {
                HeroManager.hero.target.Value = null;
            }
            
            // 删除物体Agent
            enemy.agent.RemoveAgent();
            
            // 删除物体状态条
            StateBarFactory.Instance.Despawn(enemy);
            
            enemy = null;
        }
    }
}
