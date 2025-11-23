using UnityEngine;

namespace Classes.Skills
{
    public class EssenceTheft : Skill
    {
        private int soulPieces;
        private float healCount => 35 + owner.level.Value / 3 * 10 + 0.2f * owner.abilityPower.Value;
        
        public EssenceTheft()
        {
            ReadSkillConfig("EssenceTheft");
            
            _skillLevel = 0;
            _maxSkillLevel = 0;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                healCount);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ":Skill effective");
            owner.OnKillEntity += (_, _) => soulPieces++;
            owner.EntityUpdateEvent += (_, _) =>
            {
                if (soulPieces >= 9)
                {
                    soulPieces = 0;
                    owner.TakeHeal(healCount);
                }
            };
        }
    }
}