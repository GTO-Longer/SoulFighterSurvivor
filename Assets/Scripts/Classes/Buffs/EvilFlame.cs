using System;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Buffs
{
    public class EvilFlame : Buff
    {
        private float damageCount => 20 + 0.04f * HeroManager.hero.abilityPower;
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        public Action<Entity> BuffEffect;
        public EvilFlame(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "邪焰", "", 0, 3)
        {
            buffMaxCount = 3;

            BuffEffect = (_) =>
            {
                timer += Time.deltaTime;
                buffDescription = $"每秒造成{damageCount:F0}伤害";
                if (timer >= 0.5f)
                {
                    owner.TakeDamage(damageCount * buffCount / 2f, DamageType.AP, sourceEntity);
                    timer = 0;
                }
            };
            
            OnBuffGet = () =>
            {
                buffCount = 1;
                timer = 0;
                owner.EntityUpdateEvent += BuffEffect;
            };
            
            OnBuffRunOut = () =>
            {
                buffCount = 0;
                owner.EntityUpdateEvent -= BuffEffect;
            };

            isUnique = true;
        }
    }
}