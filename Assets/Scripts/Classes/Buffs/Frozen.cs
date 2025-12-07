using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class Frozen : Buff
    {
        public Frozen(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "结霜", "降低30%移动速度", 0, 1)
        {
            OnBuffGet = () =>
            {
                owner._percentageMovementSpeedBonus.Value -= 0.3f;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageMovementSpeedBonus.Value += 0.3f;
            };

            isUnique = true;
        }
    }
}