using System;
using EntityManagers;
using UnityEngine;
using UnityEngine.Pool;

namespace Factories
{
    public class EnemyFactory : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public static EnemyFactory Instance { get; private set; }
        private ObjectPool<EnemyManager> _enemyPool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                enemyPrefab.SetActive(false);

                _enemyPool = new ObjectPool<EnemyManager>(
                    createFunc: () =>
                    {
                        var newEnemy = Instantiate(enemyPrefab, enemyPrefab.transform.parent).GetComponent<EnemyManager>();
                        newEnemy.gameObject.SetActive(false);
                        return newEnemy;
                    },
                    actionOnGet: enemy =>
                    {
                        enemy.gameObject.SetActive(true);
                        enemy.EnemyDataInitialization();
                        enemy.enabled = true;
                    },
                    actionOnRelease: enemy =>
                    {
                        enemy.ClearEnemyData();
                        enemy.enabled = false;
                    },
                    actionOnDestroy: enemy => Destroy(enemy.gameObject),
                    collectionCheck: true,
                    defaultCapacity: 10,
                    maxSize: 1000
                );
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 在指定位置创建Enemy
        /// </summary>
        public void Spawn(Vector2 position)
        {
            var enemy = _enemyPool.Get();
            enemy.enemy.agent.Warp(position);
        }

        /// <summary>
        /// 回收对应EnemyManaer
        /// </summary>
        public void Despawn(EnemyManager enemy)
        {
            _enemyPool.Release(enemy);
        }

        private void Start()
        {
            Spawn(new Vector2(500, 0));
        }
    }
}