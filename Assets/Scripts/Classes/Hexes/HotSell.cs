using System;
using Classes.Entities;
using Managers;

namespace Classes.Hexes
{
    public class HotSell : Hex
    {
        public HotSell() : base("HotSell")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            var hero = owner as Hero;
            if (hero == null) return;
            
            foreach (var property in hero.equipmentList)
            {
                if (property.Value != null)
                {
                    AudioManager.Instance.Play("Sell_Equipment", "Sell_Equipment");
                    hero.coins.Value += property.Value._cost;
                    
                    property.Value.OnEquipmentRemove();
                    property.Value = null;
                }
            }

            if (hero.tempEquipmentList.Count > 0)
            {
                var count = hero.tempEquipmentList.Count;
                for (var i = 0; i < count; i++)
                {
                    foreach (var property in hero.equipmentList)
                    {
                        if (property.Value == null)
                        {
                            property.Value = hero.tempEquipmentList[0];
                            hero.tempEquipmentList[0].OnEquipmentGet(owner);
                            hero.tempEquipmentList.RemoveAt(0);
                            break;
                        }
                    }
                }
            }
        }

        public override void OnHexRemove()
        {
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}