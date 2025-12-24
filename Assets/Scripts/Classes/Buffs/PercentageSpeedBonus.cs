using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class PercentageSpeedBonus : Buff
    {
        public PercentageSpeedBonus(Entity ownerEntity, Entity sourceEntity, float duration, float reduceRatio) : base(ownerEntity, sourceEntity, "加速", $"增加{reduceRatio:P0}移动速度", 0, duration)
        {
            OnBuffGet = () =>
            {
                owner._percentageMovementSpeedBonus.Value += reduceRatio;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageMovementSpeedBonus.Value -= reduceRatio;
            };
        }
    }
}