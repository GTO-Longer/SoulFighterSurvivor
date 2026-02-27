using System;
using Classes.Entities;

namespace Classes.Hexes
{
    public class PracticeLeg : Hex
    {
        private Action<Entity> practiceLeg;
        public PracticeLeg() : base("PracticeLeg")
        {
            practiceLeg = (_) =>
            {
                if (owner.agent != null)
                {
                    owner.agent.isGhost = true;
                }
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            owner._movementSpeedBonus.Value += 50;
            owner.EntityUpdateEvent += practiceLeg;
        }

        public override void OnHexRemove()
        {
            owner._movementSpeedBonus.Value -= 50;
            owner.EntityUpdateEvent -= practiceLeg;
            owner.agent.isGhost = false;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}