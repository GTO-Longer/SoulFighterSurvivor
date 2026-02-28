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
        private bool upgrade = false;

        private void Awake()
        {
            // 获取Hero实例
            var entityData = GetComponent<EntityData>();
            entityData.EntityInitialization();
            hero = (Hero)entityData.entity;
        }

        private void Update()
        {
            if (!hero.isAlive)
            {
                return;
            }
            
            hero.ShowAttribute();
            hero.TargetCheck();
            hero.EntityUpdate();
            hero.Regenerate();
            hero.SkillCoolDown();
            
            // 被控制期间无法移动和攻击
            if (!hero.isControlled)
            {
                hero.Move();
                hero.Attack();

                // 快捷键升级技能
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.QSkill]);
                    upgrade = true;
                }
                
                // 显示技能范围指示器
                if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Q) && hero.canUseSkill)
                {
                    if (!upgrade)
                    {
                        hero.skillList[(int)SkillType.QSkill].ShowSkillRange();
                    }
                }

                // 技能释放
                if (Input.GetKeyUp(KeyCode.Q) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
                {
                    if (!upgrade)
                    {
                        hero.SkillUsed(hero.skillList[(int)SkillType.QSkill]);
                        hero.skillList[(int)SkillType.QSkill].HideSkillRange();
                    }
                }

                if (Input.GetKeyUp(KeyCode.Q) && upgrade)
                {
                    upgrade = false;
                }

                // 快捷键升级技能
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.WSkill]);
                    upgrade = true;
                }
                
                // 显示技能范围指示器
                if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.W) && hero.canUseSkill)
                {
                    if (!upgrade)
                    {
                        hero.skillList[(int)SkillType.WSkill].ShowSkillRange();
                    }
                }

                // 技能释放
                if (Input.GetKeyUp(KeyCode.W) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
                {
                    if (!upgrade)
                    {
                        hero.SkillUsed(hero.skillList[(int)SkillType.WSkill]);
                        hero.skillList[(int)SkillType.WSkill].HideSkillRange();
                    }
                }

                if (Input.GetKeyUp(KeyCode.W) && upgrade)
                {
                    upgrade = false;
                }

                // 快捷键升级技能
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.ESkill]);
                    upgrade = true;
                }
                
                // 显示技能范围指示器
                if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.E) && hero.canUseSkill)
                {
                    if (!upgrade)
                    {
                        hero.skillList[(int)SkillType.ESkill].ShowSkillRange();
                    }
                }

                // 技能释放
                if (Input.GetKeyUp(KeyCode.E) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
                {
                    if (!upgrade)
                    {
                        hero.SkillUsed(hero.skillList[(int)SkillType.ESkill]);
                        hero.skillList[(int)SkillType.ESkill].HideSkillRange();
                    }
                }

                if (Input.GetKeyUp(KeyCode.E) && upgrade)
                {
                    upgrade = false;
                }

                // 快捷键升级技能
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
                {
                    hero.SkillUpgrade(hero.skillList[(int)SkillType.RSkill]);
                    upgrade = true;
                }
                
                // 显示技能范围指示器
                if (!Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R) && hero.canUseSkill)
                {
                    if (!upgrade)
                    {
                        hero.skillList[(int)SkillType.RSkill].ShowSkillRange();
                    }
                }

                // 技能释放
                if (Input.GetKeyUp(KeyCode.R) && hero.canUseSkill && !PanelUIRoot.Instance.isPanelOpen)
                {
                    if (!upgrade)
                    {
                        hero.SkillUsed(hero.skillList[(int)SkillType.RSkill]);
                        hero.skillList[(int)SkillType.RSkill].HideSkillRange();
                    }
                }

                if (Input.GetKeyUp(KeyCode.R) && upgrade)
                {
                    upgrade = false;
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
            else
            {
                hero.ControlTimeUpdate();
            }
        }
    }
}
