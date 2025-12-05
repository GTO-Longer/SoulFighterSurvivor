using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class FrozenHeart : Buff
    {
        public FrozenHeart(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "冰霜之心", "降低30%攻击速度", 0, 1)
        {
            OnBuffGet = () =>
            {
                owner._percentageAttackSpeedBonus.Value -= 0.3f;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageAttackSpeedBonus.Value += 0.3f;
            };

            isUnique = true;
        }
    }
}