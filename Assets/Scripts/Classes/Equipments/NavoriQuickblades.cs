using System;
using Classes.Entities;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class NavoriQuickblades : Equipment
    {
        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public NavoriQuickblades() : base("NavoriQuickblades")
        {
            equipmentEffect = (_, _, _, _) =>
            {
                var hero = owner as Hero;
                foreach (var skill in hero.skillList)
                {
                    if (skill.skillType is >= SkillType.QSkill and <= SkillType.ESkill)
                    {
                        skill.coolDownTimer += (skill.actualSkillCoolDown - skill.coolDownTimer) * 0.15f;
                    }
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AttackEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.AttackEffect -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}