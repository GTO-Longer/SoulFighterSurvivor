using System;
using Classes.Buffs;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class TrinityForce : Equipment
    {
        private float damageCount => 2f * HeroManager.hero.baseAttackDamage;
        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public TrinityForce() : base("TrinityForce")
        {
            equipmentEffect = (attacker, target, _, ratio) =>
            {
                if (HeroManager.hero.isCuredBladeEffective && _passiveSkillActive)
                {
                    target.TakeDamage(target.CalculateADDamage(attacker, damageCount * ratio), DamageType.AD, attacker);
                    
                    var speedBonus = new SpeedBonus(owner, owner, 3f, 33f);
                    owner.GetBuff(speedBonus);
                    
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AttackEffect += equipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.AttackEffect -= equipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}