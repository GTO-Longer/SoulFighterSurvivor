using System;
using UnityEngine;
using Utilities;

namespace Classes.Buffs
{
    public class Torment : Buff
    {
        private float damageCount => owner.maxHealthPoint * 0.01f;
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        private Action<Entity> BuffEffect;
        public Torment(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "折磨", "", 8, 6)
        {
            BuffEffect = (_) =>
            {
                timer += Time.deltaTime;
                buffDescription = $"每秒造成{1 * buffCount:0.#}%最大生命值魔法伤害";
                if (timer >= 0.5f)
                {
                    var damage = owner.CalculateAPDamage(sourceEntity, damageCount * buffCount / 2f);
                    owner.TakeDamage(damage, DamageType.AP, sourceEntity);
                    timer = 0;
                }
            };
            
            OnBuffGet = () =>
            {
                if (buffCount == 0)
                {
                    owner.EntityUpdateEvent += BuffEffect;
                }
                
                if (buffCount < buffMaxCount)
                {
                    buffCount.Value += 1;
                }

                timer = 0;
            };
            
            OnBuffRunOut = () =>
            {
                buffCount.Value = 0;
                owner.EntityUpdateEvent -= BuffEffect;
            };

            isUnique = true;
        }
    }
}