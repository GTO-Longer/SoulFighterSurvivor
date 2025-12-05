using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class LichBane : Equipment
    {
        private float damageCount => 0.75f * HeroManager.hero.baseAttackDamage + 0.66f * HeroManager.hero.abilityPower;
        private Action<Entity, Entity, float> equipmentEffect;
        
        public LichBane() : base("LichBane")
        {
            equipmentEffect = (attacker, target, _) =>
            {
                if (HeroManager.hero.isCuredBladeEffective && _passiveSkillActive)
                {
                    target.TakeDamage(target.CalculateAPDamage(attacker, damageCount), DamageType.AP, attacker);
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
            description = string.Format(_passiveSkillName + "\n冷却时间:" + _passiveSkillCD + "秒\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}