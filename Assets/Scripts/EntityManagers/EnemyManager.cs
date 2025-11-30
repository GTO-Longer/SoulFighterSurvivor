using Classes.Entities;
using DataManagement;
using UnityEngine;


namespace EntityManagers
{
    public class EnemyManager : MonoBehaviour
    {
        public Enemy enemy;

        private void Start()
        {
            enemy = (Enemy)GetComponent<EntityData>().entity;
            enemy.target = HeroManager.hero;
        }

        private void Update()
        {
            enemy.Move();
            enemy.Attack();
        }
    }
}
