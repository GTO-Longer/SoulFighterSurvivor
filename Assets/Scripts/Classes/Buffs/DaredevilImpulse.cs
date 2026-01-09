using System;
using Classes.Entities;
using Utilities;

namespace Classes.Buffs
{
    public class DaredevilImpulse : Buff
    {
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        public Action<Entity> BuffEffect;
        
        public DaredevilImpulse(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "悍勇本色", "提升移动速度", 6, 6)
        {
            isUnique = true;
            
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    buffCount.Value += 1;
                    buffDescription = $"提升{3.5f * buffCount:0.#}%移动速度";
                }
                
                var hero = owner as Hero;
                var daredevilImpulse = hero?.skillList[(int)SkillType.PassiveSkill] as Skills.DaredevilImpulse;
                daredevilImpulse.comboLevel = buffCount.Value;

                owner._percentageMovementSpeedBonus.Value += 0.035f;
            };
            
            OnBuffRunOut = () =>
            {
                owner._percentageMovementSpeedBonus.Value -= 0.035f * buffCount;
                var hero = owner as Hero;
                var daredevilImpulse = hero?.skillList[(int)SkillType.PassiveSkill] as Skills.DaredevilImpulse;
                daredevilImpulse.comboLevel = 0;
                buffCount.Value = 0;
            };
        }
    }
}