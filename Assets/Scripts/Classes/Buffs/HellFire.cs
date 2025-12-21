using System;
using Classes.Hexes;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Buffs
{
    public class HellFire : Buff
    {
        private float damageCount => 6 + 0.012f * HeroManager.hero.abilityPower + 0.028f * HeroManager.hero.attackDamage;
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        private Action<Entity> BuffEffect;
        /// <summary>
        /// 恢复玩家CD
        /// </summary>
        public static Action CDRecover;
        
        public HellFire(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "地狱之火", "", 9999, 5)
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
                    CDRecover?.Invoke();
                }
            };

            CDRecover = () =>
            {
                HeroManager.hero.skillList[(int)SkillType.QSkill].coolDownTimer += 0.5f;
                HeroManager.hero.skillList[(int)SkillType.WSkill].coolDownTimer += 0.5f;
                HeroManager.hero.skillList[(int)SkillType.ESkill].coolDownTimer += 0.5f;
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