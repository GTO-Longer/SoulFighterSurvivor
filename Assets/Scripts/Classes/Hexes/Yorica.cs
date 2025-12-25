using System;
using Classes.Entities;
using UnityEngine;

namespace Classes.Hexes
{
    public class Yorica : Hex
    {
        private float addAmount => 0.3f * owner.abilityPower;
        
        public Yorica() : base("Yorica")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._APToAH_ConversionEfficiency.Value += new Vector2(0, 0.3f);
        }

        public override void OnHexRemove()
        {
            owner._APToCR_ConversionEfficiency.Value -= new Vector2(0, 0.3f);
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, addAmount);
            return true;
        }
    }
}