namespace Classes.Buffs
{
    public class Tyrant : Buff
    {
        public Tyrant(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "暴行", "降低法术抗性", 4, 5)
        {
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    buffCount.Value += 1;
                    buffDescription = $"降低{0.075f * buffCount:P0}法术抗性";
                }

                owner._percentageMagicDefenseBonus.Value -= 0.075f;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageMagicDefenseBonus.Value += 0.075f * buffCount;
                buffCount.Value = 0;
            };

            isUnique = true;
        }
    }
}