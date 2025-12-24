using System;
using Managers.EntityManagers;

namespace Classes.Equipments
{
    public class CursedBlood : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        
        public CursedBlood() : base("CursedBlood")
        {
            equipmentEffect = (attacker, target, _, _) =>
            {
                var tyrant = new Buffs.Tyrant(target, attacker);
                tyrant.buffIcon = equipmentIcon;
                target.GetBuff(tyrant);
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