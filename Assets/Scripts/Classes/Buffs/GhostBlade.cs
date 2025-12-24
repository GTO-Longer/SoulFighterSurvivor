using System;

namespace Classes.Buffs
{
    public class GhostBlade : Buff
    {
        private Action<Entity> Ghost;
        public GhostBlade(Entity ownerEntity, Entity sourceEntity) : base(ownerEntity, sourceEntity, "幽梦之灵", "移动速度增加30%，进入幽灵状态。", 0, 6)
        {
            Ghost = (_) =>
            {
                if (owner != null)
                {
                    owner.agent.isGhost = true;
                }
            };
            
            OnBuffGet = () =>
            {
                owner._percentageMovementSpeedBonus.Value += 0.3f;
                owner.EntityUpdateEvent += Ghost;
            };
            
            OnBuffRunOut = () => 
            {
                owner._percentageMovementSpeedBonus.Value -= 0.3f;
                owner.EntityUpdateEvent -= Ghost;
                owner.agent.isGhost = false;
            };

            isUnique = true;
        }
    }
}