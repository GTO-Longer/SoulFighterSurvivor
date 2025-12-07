using System;
using Managers.EntityManagers;

namespace Classes.Equipments
{
    public class Cryptbloom : Equipment
    {
        private Action<Entity, Entity> OnKill;
        private float healCount => 45 + 0.05f * HeroManager.hero.abilityPower;
        
        public Cryptbloom() : base("Cryptbloom")
        {
            OnKill = (_, _) =>
            {
                if (_passiveSkillActive)
                {
                    owner.TakeHeal(healCount);
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnKillEntity += OnKill;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.OnKillEntity -= OnKill;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, healCount);
            return true;
        }
    }
}