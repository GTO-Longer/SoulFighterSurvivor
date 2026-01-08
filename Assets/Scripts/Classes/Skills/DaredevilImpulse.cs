using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class DaredevilImpulse : Skill
    {
        private float damageCount => (1 + HeroManager.hero.level) + (0.035f + 0.07f / 17f * (HeroManager.hero.level - 1)) * HeroManager.hero.attackDamage;
        public float comboLevel;
        private SkillType formerHit;
        
        public DaredevilImpulse() : base("DaredevilImpulse")
        {
            _skillLevel = 0;
            _maxSkillLevel = 0;
            comboLevel = 0;
            formerHit = SkillType.None;

            PassiveAbilityEffective += () =>
            {
                owner.AttackEffect += (_, _, _, _) =>
                {
                    if (formerHit != SkillType.Attack)
                    {
                        formerHit = SkillType.Attack;
                        
                        var daredevilImpulse = new Buffs.DaredevilImpulse(owner, owner);
                        owner.GainBuff(daredevilImpulse);
                    }

                    var buff = owner.GetBuff("悍勇本色");
                    if (buff != null)
                    {
                        buff.buffDurationTimer = 0;
                    }
                };

                owner.AbilityEffect += (_, _, _, skill) =>
                {
                    if (formerHit != skill.skillType)
                    {
                        formerHit = skill.skillType;
                        
                        var daredevilImpulse = new Buffs.DaredevilImpulse(owner, owner);
                        owner.GainBuff(daredevilImpulse);
                    }
                    
                    var buff = owner.GetBuff("悍勇本色");
                    if (buff != null)
                    {
                        buff.buffDurationTimer = 0;
                    }
                };
            };
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, damageCount);
        }
    }
}