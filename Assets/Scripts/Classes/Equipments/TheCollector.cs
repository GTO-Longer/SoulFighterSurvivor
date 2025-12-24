using System;
using Factories;
using Managers.EntityManagers;
using UnityEngine;

namespace Classes.Equipments
{
    public class TheCollector : Equipment
    {
        private Action<Entity, Entity, float> equipmentEffect;
        
        public TheCollector() : base("TheCollector")
        {
            equipmentEffect = (attacker, target, _) =>
            {
                if (target.healthPoint.Value <= target.maxHealthPoint.Value * 0.05f && target.isAlive)
                {
                    target.healthPoint.Value = 0;
                    attacker.KillEntity(target);
                    target.Die(attacker);
                    ScreenTextFactory.Instance.Spawn(target.gameObject.transform.position, $"-<sprite=\"Attributes\" name=\"CriticalRateIcon\" tint=1> 999", 0.8f,
                        200, 100, Color.white);
                    
                    HeroManager.hero.coins.Value += 20;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnDamage += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.OnDamage -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}