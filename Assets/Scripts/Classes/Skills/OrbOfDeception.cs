using System;
using UnityEngine;

namespace Classes.Skills
{
    public class OrbOfDeception : Skill
    {
        private float _APDamage => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        private float _RealDamage => _baseSkillValue[1][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        
        public OrbOfDeception()
        {
            ReadSkillConfig("OrbOfDeception");
            
            _skillLevel = 0;
            _maxSkillLevel = 5;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                _APDamage, _RealDamage);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ":Skill effective");
            
        }
    }
}