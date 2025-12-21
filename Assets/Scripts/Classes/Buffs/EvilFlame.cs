using System;
using Classes.Hexes;
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
        private Action<Entity> BuffEffect;
        
        public EvilFlame(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "邪焰", "", 3, 3)
        {
            isBurn = true;
            isUnique = true;
            
            BuffEffect = (_) =>
            {
                timer += Time.deltaTime;
                buffDescription = $"每秒造成{damageCount * buffCount:F0}魔法伤害";
                if (timer >= 1f)
                {
                    Burn?.Invoke(owner);
                    timer = 0;
                }
            };

            Burn = (_) =>
            {
                if (owner is not { isAlive: true }) return;
                
                var damage = owner.CalculateAPDamage(sourceEntity, damageCount * buffCount);
                owner.TakeDamage(damage, DamageType.AP, sourceEntity);

                if (HellfireConduit.HellfireConduitEffective)
                {
                    HellFire.CDRecover?.Invoke();
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
        }
    }
}