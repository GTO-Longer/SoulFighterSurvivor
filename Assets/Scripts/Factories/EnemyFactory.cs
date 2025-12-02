using Managers.EntityManagers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace Factories
{
    public class EnemyFactory : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public static EnemyFactory Instance { get; private set; }
        private ObjectPool<EnemyManager> _enemyPool;
        private float enemySpawnTimer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                enemyPrefab.SetActive(false);
                enemySpawnTimer = 0;

                // 定义对象池
                _enemyPool = new ObjectPool<EnemyManager>(
                    createFunc: () =>
                    {
                        var newEnemy = Instantiate(enemyPrefab, enemyPrefab.transform.parent).GetComponent<EnemyManager>();
                        newEnemy.gameObject.SetActive(false);
                        return newEnemy;
                    },
                    actionOnGet: enemy =>
                    {
                        enemy.EnemyDataInitialization();
                        enemy.enabled = true;
                    },
                    actionOnRelease: enemy =>
                    {
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
            enemy.gameObject.SetActive(true);
        }

        /// <summary>
        /// 回收对应EnemyManager
        /// </summary>
        public void Despawn(EnemyManager enemy)
        {
            _enemyPool.Release(enemy);
            enemy.ClearEnemyData();
        }

        private void Start()
        {
            Spawn(new Vector2(500, 0));
        }

        private void Update()
        {
            enemySpawnTimer += Time.deltaTime;
            if (enemySpawnTimer > 20 / (5 + HeroManager.hero.level) + 10)
            {
                Vector2 heroPosition = HeroManager.hero.gameObject.transform.position;
                
                // 随机角度
                var angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                // 随机半径
                var radius = Random.Range(1500f, 4500f);
                
                // 极坐标转直角坐标
                var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                var spawnPosition = heroPosition + offset;
                NavMesh.SamplePosition(spawnPosition, out var hit, 500f, NavMesh.AllAreas);
                
                Spawn(hit.position);
                enemySpawnTimer = 0;
            }
        }
    }
}