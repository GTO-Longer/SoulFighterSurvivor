namespace Classes.Buffs
{
    public class LastBreath : Buff
    {
        public LastBreath(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "狂风绝息斩", "获得60%额外穿透", 0, 15)
        {
            OnBuffGet = () =>
            {
                owner.percentageAttackPenetration.Value += 0.6f;
            };
            
            OnBuffRunOut = () => 
            {
                owner.percentageAttackPenetration.Value -= 0.6f;
            };

            isUnique = true;
        }
    }
}