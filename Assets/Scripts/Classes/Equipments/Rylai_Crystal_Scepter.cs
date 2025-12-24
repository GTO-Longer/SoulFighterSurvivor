using System;

namespace Classes.Equipments
{
    public class Rylai_Crystal_Scepter : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        
        public Rylai_Crystal_Scepter() : base("Rylai_Crystal_Scepter")
        {
            equipmentEffect = (_, target, _, _) =>
            {
                var frozen = new Buffs.Frozen(target, owner);
                frozen.buffIcon = equipmentIcon;
                target.GetBuff(frozen);
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