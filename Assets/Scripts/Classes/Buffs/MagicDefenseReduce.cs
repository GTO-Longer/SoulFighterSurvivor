using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class MagicDefenseReduce : Buff
    {
        public MagicDefenseReduce(Entity ownerEntity, Entity sourceEntity, float duration, float reduceAmount) : base(ownerEntity, sourceEntity, "法抗降低", $"降低{reduceAmount:F0}法术抗性", 0, duration)
        {
            OnBuffGet = () =>
            {
                owner._magicDefenseBonus.Value -= reduceAmount;
            };
            
            OnBuffRunOut = () => 
            {
                owner._magicDefenseBonus.Value += reduceAmount;
            };
        }
    }
}