using System;

namespace Classes.Buffs
{
    public class Rage : Buff
    {
        private float attackSpeedCount = 0.08f;
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        public Action<Entity> BuffEffect;
        private bool fullCharged;
        
        public Rage(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "狂暴", "提升攻速", 4, 4)
        {
            isUnique = true;
            
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    fullCharged = false;
                    buffCount.Value += 1;
                    buffDescription = $"提升{attackSpeedCount * buffCount:P0}攻速";
                }
                
                if (buffCount >= buffMaxCount && !fullCharged)
                {
                    fullCharged = true;
                    owner.attackEffectRatio += 0.33f;
                    buffDescription = $"提升{attackSpeedCount * buffCount:P0}攻速，所有攻击特效额外造成33%伤害";
                }

                owner._attackSpeedBonus.Value += attackSpeedCount;
            };
            
            OnBuffRunOut = () =>
            {
                owner._attackSpeedBonus.Value -= attackSpeedCount * buffCount;
                if (fullCharged)
                {
                    owner.attackEffectRatio -= 0.33f;
                }
                fullCharged = false;
                buffCount.Value = 0;
            };
        }
    }
}