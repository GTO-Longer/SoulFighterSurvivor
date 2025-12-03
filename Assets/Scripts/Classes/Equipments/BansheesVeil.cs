using System;
using UnityEngine;

namespace Classes.Equipments
{
    public class BansheesVeil : Equipment
    {
        private Action<Entity> equipmentEffect;
        
        public BansheesVeil() : base("BansheesVeil")
        {
            equipmentEffect = (_) =>
            {
                if (_passiveSkillCDTimer < _passiveSkillCD)
                {
                    _passiveSkillCDTimer += Time.deltaTime;
                }
                else
                {
                    
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.EntityUpdateEvent += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            base.OnEquipmentRemove();
            
            owner.EntityUpdateEvent -= equipmentEffect;
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}