using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class Hullbreaker : Buff
    {
        public Hullbreaker(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "过载", "获得50%总攻击速度和20%移动速度", 0, 10)
        {
            OnBuffGet = () =>
            {
                owner._percentageAttackSpeedBonus.Value += 0.5f;
                owner._percentageMovementSpeedBonus.Value += 0.2f;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageAttackSpeedBonus.Value -= 0.5f;
                owner._percentageMovementSpeedBonus.Value -= 0.2f;
            };

            isUnique = true;
        }
    }
}