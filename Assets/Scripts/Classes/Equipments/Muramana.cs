using System;
using Managers;
using Managers.EntityManagers;
using MVVM.ViewModels;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class Muramana : Equipment
    {
        private int killCount;
        private float addCount => 0.05f * HeroManager.hero.maxMagicPoint.Value;
        private Action<Entity, Entity> OnKill;
        public Muramana() : base("Muramana")
        {
            canPurchase = true;
            killCount = 0;
            
            OnKill = (_, _) =>
            {
                killCount += 1;

                if (killCount >= 50)
                {
                    var selfIndex = HeroManager.hero.equipmentList.FindIndex(equip => equip.Value.equipmentName == equipmentName);
                    HeroManager.hero.equipmentList[selfIndex].Value.OnEquipmentRemove();
                    HeroManager.hero.equipmentList[selfIndex].Value = EquipmentManager.Instance.GetEquipment("Manamune");
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