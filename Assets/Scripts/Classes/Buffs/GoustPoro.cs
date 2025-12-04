using System;

namespace Classes.Buffs
{
    public class GoustPoro : Buff
    {
        public GoustPoro(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "幽灵疾步", "获得4秒的40%移动速度加成，并且使自身处于幽灵状态", 0, 4)
        {
            OnBuffGet = () =>
            {
                owner._percentageMovementSpeedBonus.Value += 0.4f;
                owner.agent.isGoust = true;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageMovementSpeedBonus.Value -= 0.4f;
                owner.agent.isGoust = false;
            };

            isUnique = true;
        }
    }
}