using System;
using Managers.EntityManagers;
using UnityEngine;

namespace Classes.Equipments
{
    public class EvilCleaver : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private float damagePercent => 0.05f + 0.002f * HeroManager.hero.attackPenetration;
        
        public EvilCleaver() : base("EvilCleaver")
        {
            equipmentEffect = (attacker, target, _, _) =>
            {
                if (_passiveSkillActive)
                {
                    var burnt = new Buffs.Burnt(target, attacker, damagePercent *
                                                                  (1 + Mathf.Min(1, (target.maxHealthPoint - owner.maxHealthPoint) / 2000)));
                    burnt.buffIcon = equipmentIcon;
                    target.GetBuff(burnt);
                    
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AbilityEffect += equipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damagePercent, damagePercent * 2);
            return true;
        }
    }
}