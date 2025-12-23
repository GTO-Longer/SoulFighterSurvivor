using System;

namespace Classes.Buffs
{
    public class GhostPoro : Buff
    {
        private Action<Entity> Ghost;
        public GhostPoro(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "幽灵疾步", "获得4秒的40%移动速度加成，并且使自身处于幽灵状态", 0, 4)
        {
            Ghost = (_) =>
            {
                if (owner != null)
                {
                    owner.agent.isGoust = true;
                }
            };
            
            OnBuffGet = () =>
            {
                owner._percentageMovementSpeedBonus.Value += 0.4f;
                owner.EntityUpdateEvent += Ghost;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageMovementSpeedBonus.Value -= 0.4f;
                owner.EntityUpdateEvent -= Ghost;
                owner.agent.isGoust = false;
            };

            isUnique = true;
        }
    }
}