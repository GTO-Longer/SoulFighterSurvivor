using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class EssenceTheft : Skill
    {
        private float healCount => 35 + owner.level.Value / 3 * 10 + 0.2f * owner.abilityPower.Value;
        
        public EssenceTheft() : base("EssenceTheft")
        {
            _skillLevel = 0;
            _maxSkillLevel = 0;
            maxSkillChargeCount = 5;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                healCount);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ":Skill effective");
            owner.OnKillEntity += (_, _) => skillChargeCount++;
            owner.EntityUpdateEvent += (_) =>
            {
                if (skillChargeCount >= maxSkillChargeCount)
                {
                    skillChargeCount = 0;
                    owner.TakeHeal(healCount);
                    owner.TakeMagicRecover(healCount);
                    if (owner.skillList[(int)SkillType.RSkill].specialTimer > 0 && 
                        owner.skillList[(int)SkillType.RSkill].skillChargeCount < owner.skillList[(int)SkillType.RSkill].maxSkillChargeCount)
                    {
                        owner.skillList[(int)SkillType.RSkill].skillChargeCount += 1;
                    }
                }
            };
        }
    }
}