using UnityEngine;
using DataManagement;
using Classes.Entities;

namespace EntityManagers
{
    public class HeroManager : MonoBehaviour
    {
        public static Hero hero;

        private void Awake()
        {
            // 创建Hero实例
            hero = (Hero)GetComponent<EntityData>().entity;
        }

        private void Update()
        {
            hero.Move();
            hero.TargetCheck();
            hero.SetRotate();
            hero.Attack();
            hero.EntityUpdate(hero);

            if (Input.GetKeyDown(KeyCode.Q))
            {
                hero.QSkillRelease(hero, null);
            }
        }
    }
}
