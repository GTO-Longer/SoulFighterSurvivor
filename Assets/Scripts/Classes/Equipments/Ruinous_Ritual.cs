using System;
using Managers.EntityManagers;
using Utilities;
using Random = UnityEngine.Random;

namespace Classes.Equipments
{
    public class Ruinous_Ritual : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private float damageCount => 70 + 0.15f * HeroManager.hero.abilityPower;
        
        public Ruinous_Ritual() : base("Ruinous_Ritual")
        {
            equipmentEffect = (attacker, target, _, _) =>
            {
                var anger = new Buffs.Anger(attacker, attacker);
                anger.buffIcon = equipmentIcon;
                attacker.GainBuff(anger);

                if (Random.Range(0f, 1f) <= owner.criticalRate)
                {
                    target.TakeDamage(target.CalculateAPDamage(attacker, damageCount), DamageType.AP, attacker);
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AbilityEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}