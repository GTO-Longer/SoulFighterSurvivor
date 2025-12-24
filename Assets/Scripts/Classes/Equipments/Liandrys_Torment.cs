using System;

namespace Classes.Equipments
{
    public class Liandrys_Torment : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        
        public Liandrys_Torment() : base("Liandrys_Torment")
        {
            equipmentEffect = (attacker, target, _, _) =>
            {
                var torment = new Buffs.Torment(target, attacker);
                torment.buffIcon = equipmentIcon;
                target.GetBuff(torment);
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