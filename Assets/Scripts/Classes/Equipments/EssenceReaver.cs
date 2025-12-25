using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class EssenceReaver : Equipment
    {
        private float recoverCount => 0.1f * HeroManager.hero.maxMagicPoint;

        private Action<Entity, Entity, float, float> equipmentEffect;
        
        public EssenceReaver() : base("EssenceReaver")
        {
            equipmentEffect = (attacker, _, _, ratio) =>
            {
                attacker.TakeMagicRecover(recoverCount * ratio);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.AttackEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.AttackEffect -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, recoverCount);
            return true;
        }
    }
}