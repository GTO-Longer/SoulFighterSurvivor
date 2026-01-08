using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class Guinsoos_Rageblade : Equipment
    {
        private float damageCount => 10 + 0.03f * HeroManager.hero.abilityPower + 0.06f * HeroManager.hero.attackDamage;
        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public Guinsoos_Rageblade() : base("Guinsoos_Rageblade")
        {
            equipmentEffect = (attacker, target, _, ratio) =>
            {
                var rage = new Buffs.Rage(attacker, attacker);
                rage.buffIcon = equipmentIcon;
                attacker.GainBuff(rage);
                target.TakeDamage(target.CalculateAPDamage(attacker, damageCount * ratio), DamageType.AP, attacker);
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
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}