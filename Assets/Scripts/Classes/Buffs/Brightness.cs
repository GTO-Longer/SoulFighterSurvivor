using System;

namespace Classes.Buffs
{
    public class Brightness : Buff
    {
        private float timer;
        /// <summary>
        /// buff效果
        /// </summary>
        public Action<Entity> BuffEffect;
        
        public Brightness(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "光明", "提升双抗", 4, 5)
        {
            isUnique = true;
            
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    buffCount.Value += 1;
                    buffDescription = $"提升{8 * buffCount:F0}双抗";
                }

                owner._attackDefenseBonus.Value += 8;
                owner._magicDefenseBonus.Value += 8;
            };
            
            OnBuffRunOut = () =>
            {
                owner._attackDefenseBonus.Value -= 8 * buffCount;
                owner._magicDefenseBonus.Value -= 8 * buffCount;
                buffCount.Value = 0;
            };
        }
    }
}