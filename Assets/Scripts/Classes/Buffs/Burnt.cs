using System;
using Classes.Hexes;
using UnityEngine;
using Utilities;

namespace Classes.Buffs
{
    public class Burnt : Buff
    {
        private float damagePercent;
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        private Action<Entity> BuffEffect;
        private int burnCounter;
        
        public Burnt(Entity ownerEntity, Entity sourceEntity, float percent) : base(ownerEntity, sourceEntity, "烧焦", "", 0, 4)
        {
            isBurn = true;
            isUnique = true;
            damagePercent = percent;
            burnCounter = 0;
            
            BuffEffect = (_) =>
            {
                timer += Time.deltaTime;
                buffDescription = $"每秒造成{damagePercent:P0}当前生命值的物理伤害";
                if (timer >= 0.2f)
                {
                    Burn?.Invoke(owner);
                    timer = 0;
                }
            };

            Burn = (_) =>
            {
                if (owner is not { isAlive: true }) return;
                
                var damage = owner.CalculateADDamage(sourceEntity, damagePercent * owner.healthPoint / 5f);
                owner.TakeDamage(damage, DamageType.AD, sourceEntity);

                burnCounter += 1;

                if (HellfireConduit.HellfireConduitEffective && burnCounter >= 5)
                {
                    HellFire.CDRecover?.Invoke();
                    burnCounter = 0;
                }
            };
            
            OnBuffGet = () =>
            {
                owner.EntityUpdateEvent += BuffEffect;
            };
            
            OnBuffRunOut = () =>
            {
                owner.EntityUpdateEvent -= BuffEffect;
            };
        }
    }
}