using System;
using System.Numerics;
using Utilities;

namespace Classes.Equipments
{
    public class HorizonFocus : Equipment
    {
        private Action<Entity, Entity, float> equipmentEffect;
        public HorizonFocus() : base("HorizonFocus")
        {
            equipmentEffect = (attacker, target, damageCount) =>
            {
                var targetPosition = new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.y);
                var attackerPosition = new Vector2(attacker.gameObject.transform.position.x, attacker.gameObject.transform.position.y);
                if (Vector2.Distance(targetPosition, attackerPosition) > 1000)
                {
                    target.TakeDamage(damageCount * 0.20f, DamageType.Real, attacker);
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
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}