using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class SpeedReduce : Buff
    {
        public SpeedReduce(Entity ownerEntity, Entity sourceEntity, float duration, float reduceRatio) : base(ownerEntity, sourceEntity, "减速", $"降低{reduceRatio:P0}移动速度", 0, duration)
        {
            OnBuffGet = () =>
            {
                owner._percentageMovementSpeedBonus.Value -= reduceRatio;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageMovementSpeedBonus.Value += reduceRatio;
            };
        }
    }
}