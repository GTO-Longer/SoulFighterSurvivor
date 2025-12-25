using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class TapDance : Buff
    {
        public TapDance(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "踢踏舞", "", 99999, 5)
        {
            isUnique = true;
            
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    owner._movementSpeedBonus.Value += 5;
                    buffCount.Value += 1;
                    buffDescription = $"提升{5 * buffCount:F0}移动速度";
                }
            };
            
            OnBuffRunOut = () => 
            {
                owner._movementSpeedBonus.Value -= 5 * buffCount;
                buffCount.Value = 0;
            };
        }
    }
}