using System;
using Managers.EntityManagers;
using UnityEngine;

namespace Classes.Equipments
{
    public class Worglet_sDeathcap : Equipment
    {
        private float addCount => 0.1f * HeroManager.hero.maxMagicPoint.Value;
        private float healCount => 300 + 0.2f * HeroManager.hero.maxMagicPoint.Value;

        private Action<Entity, Entity, float> equipmentEffect;
        public Worglet_sDeathcap() : base("Worglet_sDeathcap")
        {
            canPurchase = false;
            
            equipmentEffect = (_, _, _) =>
            {
                if (owner.healthPointProportion <= 0.3f && _passiveSkillActive)
                {
                    _passiveSkillCDTimer = 0;
                    owner.TakeHeal(healCount);
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner._MPToAP_ConversionEfficiency.Value = new Vector2(owner._MPToAP_ConversionEfficiency.Value.x, owner._MPToAP_ConversionEfficiency.Value.y + 0.1f);
            owner._percentageAbilityPowerBonus.Value += 0.5f;
            owner.OnHurt += equipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner._MPToAP_ConversionEfficiency.Value = new Vector2(owner._MPToAP_ConversionEfficiency.Value.x, owner._MPToAP_ConversionEfficiency.Value.y - 0.1f);
            owner._percentageAbilityPowerBonus.Value -= 0.5f;
            owner.OnHurt -= equipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, addCount, healCount);
            return true;
        }
    }
}