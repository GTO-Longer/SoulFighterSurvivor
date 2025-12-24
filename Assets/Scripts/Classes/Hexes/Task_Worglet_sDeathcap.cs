using System;
using Classes.Entities;
using Classes.Equipments;
using Managers;
using Managers.EntityManagers;
using UnityEngine;

namespace Classes.Hexes
{
    public class Task_Worglet_sDeathcap : Hex
    {
        private Action<Entity> HexEffect;
        public Task_Worglet_sDeathcap() : base("Task_Worglet_sDeathcap")
        {
            HexEffect = (_) =>
            {
                if (owner is not Hero hero) return;
                
                var capIndex = hero.equipmentList.FindIndex(equip =>
                    equip.Value == EquipmentManager.Instance.GetEquipment("Rabadons_Deathcap"));
                var seraphIndex = hero.equipmentList.FindIndex(equip =>
                    equip.Value == EquipmentManager.Instance.GetEquipment("Seraph_sEmbrace"));

                if (seraphIndex == -1 || capIndex == -1)
                {
                    return;
                }

                hero.equipmentList[capIndex].Value.canPurchase = false;
                hero.equipmentList[capIndex].Value.OnEquipmentRemove();
                hero.equipmentList[seraphIndex].Value.canPurchase = false;
                hero.equipmentList[seraphIndex].Value.OnEquipmentRemove();
                hero.equipmentList[Mathf.Max(capIndex, seraphIndex)].Value = null;
                hero.equipmentList[Mathf.Min(capIndex, seraphIndex)].Value = EquipmentManager.Instance.GetEquipment("Worglet_sDeathcap");
                hero.equipmentList[Mathf.Min(capIndex, seraphIndex)].Value.OnEquipmentGet(hero);
                hero.equipmentList[Mathf.Min(capIndex, seraphIndex)].Value.canPurchase = true;
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.EntityUpdateEvent += HexEffect;
            if (!hexGetEventTriggered)
            {
                HeroManager.hero.coins.Value += 1250;
                hexGetEventTriggered = true;
            }
        }

        public override void OnHexRemove()
        {
            
            owner.EntityUpdateEvent -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}