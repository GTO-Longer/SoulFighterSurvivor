using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class Stormsurge : Equipment
    {
        private float damageCount => 180 + 0.5f * HeroManager.hero.abilityPower;
        private Action<Entity, Entity, float> equipmentEffect;
        private float damageSum;
        
        public Stormsurge() : base("Stormsurge")
        {
            equipmentEffect = (attacker, target, damageCount) =>
            {
                damageSum += damageCount;
                Async.SetAsync(2.5f, null, null, () => damageSum -= damageCount);

                if (damageSum >= target.maxHealthPoint.Value * 0.25f)
                {
                    Async.SetAsync(2, null, null, () => target.TakeDamage(target.CalculateAPDamage(attacker, damageCount), DamageType.AP, attacker));
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.EntityUpdateEvent += equipmentTimerUpdate;
            owner.AbilityEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            base.OnEquipmentRemove();
            
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            owner.AbilityEffect -= equipmentEffect;
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}