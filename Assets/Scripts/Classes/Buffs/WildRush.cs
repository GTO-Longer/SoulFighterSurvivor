using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class WildRush : Buff
    {
        public WildRush(Entity ownerEntity, Entity sourceEntity, float bonus) : base(ownerEntity, sourceEntity, "狂飙", $"增加{bonus:P0}攻击速度", 0, 5)
        {
            OnBuffGet = () =>
            {
                owner._attackSpeedBonus.Value += bonus;
            };
            
            OnBuffRunOut = () => 
            {
                owner._attackSpeedBonus.Value -= bonus;
            };

            isUnique = true;
        }
    }
}