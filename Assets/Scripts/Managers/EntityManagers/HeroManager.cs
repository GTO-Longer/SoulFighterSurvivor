using UnityEngine;
using DataManagement;
using Classes.Entities;
using Utilities;

namespace Managers.EntityManagers
{
    public class HeroManager : MonoBehaviour
    {
        public static Hero hero;

        private void Awake()
        {
            // 获取Hero实例
            var entityData = GetComponent<EntityData>();
            entityData.EntityInitialization();
            hero = (Hero)entityData.entity;
        }

        private void Update()
        {
            hero.Move();
            hero.TargetCheck();
            hero.Attack();
            hero.EntityUpdate();
            hero.Regenerate();
            hero.SkillCoolDown();

            // 技能释放
            if (Input.GetKeyDown(KeyCode.Q) && hero.canUseSkill)
            {
                // 快捷键升级技能
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.QSkill]);
                }
                else
                {
                    hero.QSkillRelease();
                }
            }

            if (Input.GetKeyDown(KeyCode.W) && hero.canUseSkill)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.WSkill]);
                }
                else
                {
                    hero.WSkillRelease();
                }
            }

            if (Input.GetKeyDown(KeyCode.E) && hero.canUseSkill)
            {

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.ESkill]);
                }
                else
                {
                    hero.ESkillRelease();
                }
            }

            if (Input.GetKeyDown(KeyCode.R) && hero.canUseSkill)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.RSkill]);
                }
                else
                {
                    hero.RSkillRelease();
                }
            }
        }
    }
}
