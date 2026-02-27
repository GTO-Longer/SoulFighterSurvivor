using System;
using Classes.Entities;
using Classes.Equipments;
using Managers;
using Managers.EntityManagers;
using UnityEngine;

namespace Classes.Hexes
{
    public class Task_Warrior : Hex
    {
        private Action<Entity, Entity> HexEffect;
        private int killCount = 0;
        private bool completed = false;
        
        public Task_Warrior() : base("Task_Warrior")
        {
            HexEffect = (_, _) =>
            {
                if (killCount < 50)
                {
                    killCount += 1;
                }
                else
                {
                    if (!completed)
                    {
                        if (owner is not Hero hero) return;

                        completed = true;
                        var goldenShovel = EquipmentManager.Instance.GetEquipment("GoldenShovel");
                        goldenShovel.canPurchase = true;
                        hero.PurchaseEquipment(goldenShovel);
                    }
                }
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.OnKillEntity += HexEffect;
        }

        public override void OnHexRemove()
        {
            
            owner.OnKillEntity -= HexEffect;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, killCount);
            return true;
        }
    }
}