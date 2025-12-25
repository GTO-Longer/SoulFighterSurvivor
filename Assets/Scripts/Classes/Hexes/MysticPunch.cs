using System;
using Classes.Entities;
using UnityEngine;
using Utilities;

namespace Classes.Hexes
{
    public class MysticPunch : Hex
    {
        private Action<Entity, Entity, float, float> HexEffect;
        
        public MysticPunch() : base("MysticPunch")
        {
            HexEffect = (_, _, _, _) =>
            {
                var hero = owner as Hero;
                foreach (var skill in hero.skillList)
                {
                    if (skill.skillType is >= SkillType.QSkill and <= SkillType.RSkill)
                    {
                        skill.coolDownTimer += Mathf.Max(1f, (skill.actualSkillCoolDown - skill.coolDownTimer) * 0.15f);
                    }
                }
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.AttackEffect += HexEffect;
        }

        public override void OnHexRemove()
        {
            owner.AttackEffect -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}