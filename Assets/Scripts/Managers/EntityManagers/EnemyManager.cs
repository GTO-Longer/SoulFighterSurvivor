using Classes.Entities;
using DataManagement;
using Factories;
using UnityEngine;


namespace Managers.EntityManagers
{
    public class EnemyManager : MonoBehaviour
    {
        public Enemy enemy;

        /// <summary>
        /// 初始化敌人数据
        /// </summary>
        public void EnemyDataInitialization(Vector2 position)
        {
            var entityData = GetComponent<EntityData>();
            entityData.EntityInitialization();
            enemy = (Enemy)entityData.entity;
            enemy.agent.Warp(position);
            enemy.target = HeroManager.hero;
            enemy.isAlive = true;
            enemy.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (enemy == null || !enemy.isAlive || HeroManager.hero == null || !HeroManager.hero.isAlive)
            {
                return;
            }

            enemy.GainExperience();
            enemy.EntityUpdate();

            if (!enemy.isControlled)
            {
                enemy.Move();
                enemy.Attack();
            }
            else
            {
                enemy.ControlTimeUpdate();
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
            
            // 清空buffList
            for (var index = enemy.buffList.Count - 1; index >= 0; index--)
            {
                enemy.buffList[index].RemoveBuff();
            }
            
            enemy = null;
        }
    }
}
