using System;
using Utilities;

namespace Classes.Equipments
{
    public class EdgeBow : Equipment
    {
        private Action<Entity, Entity, float, float> equipmentEffect;
        private bool isDark;
        
        public EdgeBow() : base("EdgeBow")
        {
            equipmentEffect = (attacker, target, _, ratio) =>
            {
                if (isDark)
                {
                    var darkness = new Buffs.Darkness(attacker, attacker);
                    darkness.buffIcon = null;
                    attacker.GetBuff(darkness);
                }
                else
                {
                    var brightness = new Buffs.Brightness(attacker, attacker);
                    brightness.buffIcon = null;
                    attacker.GetBuff(brightness);
                }
                
                isDark = !isDark;
                target.TakeDamage(target.CalculateAPDamage(attacker, 30 * ratio), DamageType.AP, attacker);
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