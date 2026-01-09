using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class DaredevilImpulse : Skill
    {
        private float _damageCount => (1 + HeroManager.hero.level) + (0.035f + 0.07f / 17f * (HeroManager.hero.level - 1)) * HeroManager.hero.attackDamage;
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
                owner.AttackEffect += (_, target, _, _) =>
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

                    if (Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) <
                        200f + owner.scale + target.scale)
                    {
                        var damage = target.CalculateAPDamage(owner, _damageCount * (2 - target.healthPointProportion));
                        target.TakeDamage(damage, DamageType.AP, owner);
                    }
                };

                owner.AbilityEffect += (_, target, _, skill) =>
                {
                    if (skill.skillType == SkillType.RSkill) return;
                    
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

                    if (Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) <
                        200f + owner.scale + target.scale)
                    {
                        var damage = target.CalculateAPDamage(owner, _damageCount * (2 - target.healthPointProportion));
                        target.TakeDamage(damage, DamageType.AP, owner);
                    }
                };
            };
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damageCount);
        }
    }
}