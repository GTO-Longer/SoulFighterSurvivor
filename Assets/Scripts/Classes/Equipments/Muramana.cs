using System;
using Managers;
using Managers.EntityManagers;
using UnityEngine;

namespace Classes.Equipments
{
    public class Muramana : Equipment
    {
        private float addCount => 0.05f * HeroManager.hero.maxMagicPoint.Value;
        private Action<Entity, Entity> OnKill;
        public Muramana() : base("Muramana")
        {
            canPurchase = true;
            maxChargeCount.Value = 50;
            chargeCount.Value = 0;
            
            OnKill = (_, _) =>
            {
                if (chargeCount.Value >= maxChargeCount.Value)
                {
                    var selfIndex = HeroManager.hero.equipmentList.FindIndex(equip => equip.Value.equipmentName == equipmentName);
                    HeroManager.hero.equipmentList[selfIndex].Value.OnEquipmentRemove();
                    HeroManager.hero.equipmentList[selfIndex].Value = EquipmentManager.Instance.GetEquipment("Manamune");
                    HeroManager.hero.equipmentList[selfIndex].Value.OnEquipmentGet(HeroManager.hero);
                    canPurchase = false;
                    HeroManager.hero.equipmentList[selfIndex].Value.canPurchase = true;
                }
                else
                {
                    chargeCount.Value += 1;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnKillEntity += OnKill;
            owner._MPToAD_ConversionEfficiency.Value = new Vector2(owner._MPToAD_ConversionEfficiency.Value.x, owner._MPToAD_ConversionEfficiency.Value.y + 0.05f);
        }

        public override void OnEquipmentRemove()
        {
            owner.OnKillEntity -= OnKill;
            owner._MPToAD_ConversionEfficiency.Value = new Vector2(owner._MPToAD_ConversionEfficiency.Value.x, owner._MPToAD_ConversionEfficiency.Value.y - 0.05f);
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, addCount);
            return true;
        }
    }
}