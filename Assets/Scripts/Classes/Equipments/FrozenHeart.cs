using System;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class FrozenHeart : Equipment
    {
        private Action<Entity> equipmentEffect;
        
        public FrozenHeart() : base("FrozenHeart")
        {
            equipmentEffect = (attacker) =>
            {
                var enemies = ToolFunctions.IsOverlappingOtherTagAll(attacker.gameObject, 300);
                if (enemies != null)
                {
                    foreach (var enemy in enemies)
                    {
                        var frozenHeart = new Buffs.FrozenHeart(enemy, owner);
                        enemy.GetBuff(frozenHeart);
                    }
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
            owner.EntityUpdateEvent -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}