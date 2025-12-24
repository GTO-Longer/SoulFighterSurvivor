using System;

namespace Classes.Equipments
{
    public class DanceOfShadow : Equipment
    {
        private Action<Entity> equipmentEffect;
        
        public DanceOfShadow() : base("DanceOfShadow")
        {
            equipmentEffect = (_) =>
            {
                owner.agent.isGhost = true;
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.EntityUpdateEvent += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.EntityUpdateEvent -= equipmentEffect;
            owner.agent.isGhost = false;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}