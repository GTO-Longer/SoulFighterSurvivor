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
            // 获取Hero实例
            hero = (Hero)GetComponent<EntityData>().entity;
        }

        private void Update()
        {
            hero.Move();
            hero.TargetCheck();
            hero.SetRotate();
            hero.Attack();
            hero.EntityUpdate();
            hero.Regenerate();
            hero.SkillCoolDown();

            if (Input.GetKeyDown(KeyCode.Q) && hero.canUseSkill)
            {
                hero.QSkillRelease();
            }

            if (Input.GetKeyDown(KeyCode.W) && hero.canUseSkill)
            {
                hero.WSkillRelease();
            }

            if (Input.GetKeyDown(KeyCode.E) && hero.canUseSkill)
            {
                hero.ESkillRelease();
            }

            if (Input.GetKeyDown(KeyCode.R) && hero.canUseSkill)
            {
                hero.RSkillRelease();
            }
        }
    }
}
