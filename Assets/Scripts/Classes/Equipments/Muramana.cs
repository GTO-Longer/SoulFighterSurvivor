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
        private float baseAttackBonus;
        private float addCount => 0.05f * HeroManager.hero.maxMagicPoint.Value;
        private Action<Entity> equipmentEffect;
        private Action<Entity, Entity> OnKill;
        public Muramana() : base("Muramana")
        {
            canPurchase = true;
            killCount = 0;
            baseAttackBonus = equipmentAttributes[EquipmentAttributeType.attackDamage];
            equipmentEffect = (hero) =>
            {
                var formerValue = equipmentAttributes[EquipmentAttributeType.attackDamage];
                if (Mathf.Abs(baseAttackBonus + addCount - formerValue) > 0.1f)
                {
                    equipmentAttributes[EquipmentAttributeType.attackDamage] = baseAttackBonus + addCount;
                    hero._attackDamageBonus.Value += equipmentAttributes[EquipmentAttributeType.attackDamage] - formerValue;
                }
            };
            
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

            owner.EntityUpdateEvent += equipmentEffect;
            owner.OnKillEntity += OnKill;
        }

        public override void OnEquipmentRemove()
        {
            owner.EntityUpdateEvent -= equipmentEffect;
            owner.OnKillEntity -= OnKill;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, addCount);
            return true;
        }
    }
}