using System;
using Managers.EntityManagers;

namespace Classes.Equipments
{
    public class BlackFireTorch : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private float damageCount => 20 + 0.04f * HeroManager.hero.abilityPower;
        
        public BlackFireTorch() : base("BlackFireTorch")
        {
            equipmentEffect = (attacker, target, _, _) =>
            {
                var evilFlame = new Buffs.EvilFlame(target, attacker);
                evilFlame.buffIcon = equipmentIcon;
                target.GetBuff(evilFlame);
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.AbilityEffect += equipmentEffect;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription, damageCount);
            return true;
        }
    }
}