namespace Classes.Buffs
{
    public class SpellDance : Buff
    {
        public SpellDance(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "咒舞", "移动速度增加60", 0, 4)
        {
            OnBuffGet = () =>
            {
                owner._movementSpeedBonus.Value += 60;
            };
            
            OnBuffRunOut = () => 
            {
                owner._movementSpeedBonus.Value -= 60;
            };

            isUnique = true;
        }
    }
}