using UnityEngine;
using DataManagement;

namespace Entities.Hero
{
    public class HeroManager : MonoBehaviour
    {
        public static Classes.Hero hero;

        private void Awake()
        {
            // 创建Hero实例
            hero = (Classes.Hero)GetComponent<EntityData>().entity;
        }

        private void Update()
        {
            hero.Move();
            hero.TargetCheck();
            hero.SetRotate();
            hero.Attack();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                hero.QSkillRelease(hero, null);
            }
        }
    }
}
