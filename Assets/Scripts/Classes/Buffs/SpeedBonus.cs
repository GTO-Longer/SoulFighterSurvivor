using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class SpeedBonus : Buff
    {
        public SpeedBonus(Entity ownerEntity, Entity sourceEntity, float duration, float amount) : base(ownerEntity, sourceEntity, "加速", $"增加{amount:F0}移动速度", 0, duration)
        {
            OnBuffGet = () =>
            {
                owner._movementSpeedBonus.Value += amount;
            };
            
            OnBuffRunOut = () => 
            {
                owner._movementSpeedBonus.Value -= amount;
            };
        }
    }
}