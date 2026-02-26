using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class BlackCleavers : Buff
    {
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        public Action<Entity> BuffEffect;
        
        public BlackCleavers(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "切割", "降低护甲", 5, 6)
        {
            isUnique = true;
            
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    buffCount.Value += 1;
                    buffDescription = $"降低{0.06f * buffCount:P0}护甲";
                }

                owner._percentageAttackDefenseBonus.Value -= 0.06f;
            };
            
            OnBuffRunOut = () =>
            {
                owner._percentageAttackDefenseBonus.Value += 0.06f * buffCount;
                buffCount.Value = 0;
            };
        }
    }
}