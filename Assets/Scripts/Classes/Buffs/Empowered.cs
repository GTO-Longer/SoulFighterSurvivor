using System;
using UnityEngine;

namespace Classes.Buffs
{
    public class Empowered : Buff
    {
        private float moveDistance = 0;
        public Empowered(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "盈能", "盈能满层时攻击可以触发盈能射击", 100, -1)
        {
            isUnique = true;

            OnBuffGet = () =>
            {
                owner.OnAttackHit += (_, _) =>
                {
                    buffCount.Value += 10;
                    buffCount.Value = Mathf.Min(buffCount.Value, buffMaxCount);
                };

                owner.EntityUpdateEvent += (_) =>
                {
                    if (!owner.agent.isStopped)
                    {
                        moveDistance += owner.movementSpeed.Value * Time.deltaTime;
                    }

                    if (moveDistance >= 62.5f)
                    {
                        moveDistance -= 62.5f;
                        
                        buffCount.Value += 5;
                        buffCount.Value = Mathf.Min(buffCount.Value, buffMaxCount);
                    }
                };
            };
        }
    }
}