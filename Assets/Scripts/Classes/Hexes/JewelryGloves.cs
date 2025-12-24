using System;
using Classes.Entities;
using UnityEngine;

namespace Classes.Hexes
{
    public class JewelryGloves : Hex
    {
        private float addAmount => 0.25f + 0.00045f * owner.abilityPower;
        
        public JewelryGloves() : base("JewelryGloves")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner.canSkillCritical = true;
            owner._APToCR_ConversionEfficiency.Value += new Vector2(0, 0.00045f);
            owner._criticalRateBonus.Value += 0.25f;
        }

        public override void OnHexRemove()
        {
            owner.canSkillCritical = false;
            owner._criticalRateBonus.Value -= 0.25f;
            owner._APToCR_ConversionEfficiency.Value -= new Vector2(0, 0.00045f);
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, addAmount);
            return true;
        }
    }
}