namespace Classes.Buffs
{
    public class Anger : Buff
    {
        public Anger(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "怒火", "增加暴击率", 10, 6)
        {
            OnBuffGet = () =>
            {
                if (buffCount < buffMaxCount)
                {
                    buffCount.Value += 1;
                    buffDescription = $"增加{0.025f * buffCount:P0}暴击率";
                }

                owner._criticalRateBonus.Value += 0.025f;
            };
            
            OnBuffRunOut = () => 
            {
                owner._criticalRateBonus.Value -= 0.025f * buffCount;
                buffCount.Value = 0;
            };

            isUnique = true;
        }
    }
}