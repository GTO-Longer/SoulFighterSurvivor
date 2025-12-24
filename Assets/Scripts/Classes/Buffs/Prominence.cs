using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class Prominence : Buff
    {
        public Prominence(Entity ownerEntity, Entity sourceEntity, int chargeCount) : base(ownerEntity, sourceEntity, "盛名", "", 9999, 20)
        {
            isUnique = true;
            
            OnBuffGet = () =>
            {
                if (buffCount == 0)
                {
                    buffCount.Value = chargeCount;
                    owner._attackDamageBonus.Value += 15f + buffCount * 0.2f;
                }
                else
                {
                    buffCount.Value += 1;
                    owner._attackDamageBonus.Value += 0.2f;
                }
                
                buffDescription = $"增加{15f + buffCount * 0.2f:0.#}攻击力";
            };

            OnBuffRunOut = () =>
            {
                owner._attackDamageBonus.Value -= 15f + buffCount * 0.2f;
                buffCount.Value = 0;
            };
        }
    }
}