using System;

namespace Classes.Buffs
{
    public class VoidInfection : Buff
    {
        private float percentageAPCount = 0.02f;
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        public Action<Entity> BuffEffect;
        private bool fullCharged;
        public VoidInfection(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "虚空侵染", "提升法术强度", 5, 6)
        {
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    fullCharged = false;
                    buffCount.Value += 1;
                    buffDescription = $"提升{percentageAPCount * buffCount:P0}法术强度";
                }
                
                if (buffCount >= buffMaxCount && !fullCharged)
                {
                    fullCharged = true;
                    owner.omnivamp.Value += 0.15f;
                    buffDescription = $"提升{percentageAPCount * buffCount:P0}法术强度，获得15%全能吸血";
                }

                owner._percentageAbilityPowerBonus.Value += percentageAPCount;
            };
            
            OnBuffRunOut = () =>
            {
                owner._percentageAbilityPowerBonus.Value -= percentageAPCount * buffCount;
                if (fullCharged)
                {
                    owner.omnivamp.Value -= 0.15f;
                }
                fullCharged = false;
                buffCount.Value = 0;
            };

            isUnique = true;
        }
    }
}