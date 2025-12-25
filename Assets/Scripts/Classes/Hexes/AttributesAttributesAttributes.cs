using System;
using Classes.Entities;
using Managers;
using MVVM.ViewModels;

namespace Classes.Hexes
{
    public class AttributesAttributesAttributes : Hex
    {
        public AttributesAttributesAttributes() : base("AttributesAttributesAttributes")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            EquipmentManager.Instance.GetEquipment("AttributeAnvil").OnActiveSkillEffective();
            EquipmentManager.Instance.GetEquipment("AttributeAnvil").OnActiveSkillEffective();
            EquipmentManager.Instance.GetEquipment("AttributeAnvil").OnActiveSkillEffective();
            EquipmentManager.Instance.GetEquipment("AttributeAnvil").OnActiveSkillEffective();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}