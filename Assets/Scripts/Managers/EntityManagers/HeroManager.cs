using UnityEngine;
using DataManagement;
using Classes.Entities;
using Components.UI;
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
            hero.ShowAttribute();
            hero.Move();
            hero.TargetCheck();
            hero.Attack();
            hero.EntityUpdate();
            hero.Regenerate();
            hero.SkillCoolDown();

            // 技能释放
            if (Input.GetKeyDown(KeyCode.Q) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
            {
                // 快捷键升级技能
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.QSkill]);
                }
                else
                {
                    hero.SkillUsed(hero.skillList[(int)SkillType.QSkill]);
                }
            }

            if (Input.GetKeyDown(KeyCode.W) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.WSkill]);
                }
                else
                {
                    hero.SkillUsed(hero.skillList[(int)SkillType.WSkill]);
                }
            }

            if (Input.GetKeyDown(KeyCode.E) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.ESkill]);
                }
                else
                {
                    hero.SkillUsed(hero.skillList[(int)SkillType.ESkill]);
                }
            }

            if (Input.GetKeyDown(KeyCode.R) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.RSkill]);
                }
                else
                {
                    hero.SkillUsed(hero.skillList[(int)SkillType.RSkill]);
                }
            }

            if (Input.GetKeyDown(KeyCode.D) && !PanelUIRoot.Instance.isPanelOpen)
            {
                hero.SkillUsed(hero.skillList[(int)SkillType.DSkill]);
            }

            if (Input.GetKeyDown(KeyCode.F) && !PanelUIRoot.Instance.isPanelOpen)
            {
                hero.SkillUsed(hero.skillList[(int)SkillType.FSkill]);
            }

            for (int index = 0; index < 6; index++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + index) && !PanelUIRoot.Instance.isPanelOpen)
                {
                    hero.EquipmentActiveSkillRelease(index);
                }
            }
        }
    }
}
