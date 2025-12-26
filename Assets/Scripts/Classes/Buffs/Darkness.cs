using System;

namespace Classes.Buffs
{
    public class Darkness : Buff
    {
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        public Action<Entity> BuffEffect;
        
        public Darkness(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "黑暗", "提升双穿", 4, 5)
        {
            isUnique = true;
            
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    buffCount.Value += 1;
                    buffDescription = $"提升{0.08f * buffCount:P0}双抗";
                }

                owner.percentageAttackPenetration.Value += 0.08f;
                owner.percentageMagicPenetration.Value += 0.08f;
            };
            
            OnBuffRunOut = () =>
            {
                owner.percentageAttackPenetration.Value -= 0.08f * buffCount;
                owner.percentageMagicPenetration.Value -= 0.08f * buffCount;
                buffCount.Value = 0;
            };
        }
    }
}