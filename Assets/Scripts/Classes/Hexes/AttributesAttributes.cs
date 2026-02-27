using System;
using Classes.Entities;
using Managers;
using MVVM.ViewModels;

namespace Classes.Hexes
{
    public class AttributesAttributes : Hex
    {
        public AttributesAttributes() : base("AttributesAttributes")
        {

        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
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