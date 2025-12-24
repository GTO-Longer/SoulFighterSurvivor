using System;
using Managers;
using Managers.EntityManagers;
using UnityEngine;

namespace Classes.Equipments
{
    public class Archangel_sStaff : Equipment
    {
        private float addCount => 0.03f * HeroManager.hero.maxMagicPoint.Value;
        private Action<Entity, Entity> OnKill;
        public Archangel_sStaff() : base("Archangel_sStaff")
        {
            canPurchase = true;
            chargeCount.Value = 0;
            maxChargeCount.Value = 50;
            
            OnKill = (_, _) =>
            {
                if (chargeCount.Value < maxChargeCount.Value)
                {
                    chargeCount.Value += 1;
                }

                if (chargeCount.Value >= maxChargeCount.Value)
                {
                    var selfIndex = HeroManager.hero.equipmentList.FindIndex(equip => equip.Value.equipmentName == equipmentName);
                    HeroManager.hero.equipmentList[selfIndex].Value.OnEquipmentRemove();
                    HeroManager.hero.equipmentList[selfIndex].Value = EquipmentManager.Instance.GetEquipment("Seraph_sEmbrace");
                    HeroManager.hero.equipmentList[selfIndex].Value.OnEquipmentGet(HeroManager.hero);
                    canPurchase = false;
                    HeroManager.hero.equipmentList[selfIndex].Value.canPurchase = true;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnKillEntity += OnKill;
            owner._MPToAP_ConversionEfficiency.Value = new Vector2(owner._MPToAP_ConversionEfficiency.Value.x, owner._MPToAP_ConversionEfficiency.Value.y + 0.03f);
        }

        public override void OnEquipmentRemove()
        {
            owner.OnKillEntity -= OnKill;
            owner._MPToAP_ConversionEfficiency.Value = new Vector2(owner._MPToAP_ConversionEfficiency.Value.x, owner._MPToAP_ConversionEfficiency.Value.y - 0.03f);
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, addCount);
            return true;
        }
    }
}