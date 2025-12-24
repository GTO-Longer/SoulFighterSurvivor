using System;
using Managers.EntityManagers;
using Utilities;

namespace Classes.Equipments
{
    public class RodofAges : Equipment
    {
        private Action<Skill> equipmentEffect;
        private Action<Entity, Entity> OnKill;
        private Action<Entity, Entity, float> OnHurt;
        private bool completed;
        
        public RodofAges() : base("RodofAges")
        {
            maxChargeCount.Value = 50;
            chargeCount.Value = 0;
            completed = false;
            
            equipmentEffect = (skill) =>
            {
                owner.TakeHeal(skill.actualSkillCost * 0.25f);
            };

            OnHurt = (self, _, damageCount) =>
            {
                self.TakeMagicRecover(damageCount * 0.15f);
            };
            
            OnKill = (_, _) =>
            {
                if (chargeCount.Value >= maxChargeCount.Value && !completed)
                {
                    equipmentAttributes[EquipmentAttributeType.abilityPower] += 50;
                    equipmentAttributes[EquipmentAttributeType.maxHealthPoint] += 400;
                    equipmentAttributes[EquipmentAttributeType.maxMagicPoint] += 300;

                    HeroManager.hero._abilityPowerBonus.Value += 50;
                    HeroManager.hero._maxHealthPointBonus.Value += 400;
                    HeroManager.hero._maxMagicPointBonus.Value += 300;
                    
                    HeroManager.hero.healthPoint.Value += 400;
                    HeroManager.hero.magicPoint.Value += 300;
                    
                    HeroManager.hero.LevelUp();
                    completed = true;
                }
                else
                {
                    chargeCount.Value += 1;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.OnSkillUsed += equipmentEffect;
            owner.OnHurt += OnHurt;
            owner.OnKillEntity += OnKill;
        }

        public override void OnEquipmentRemove()
        {
            owner.OnSkillUsed -= equipmentEffect;
            owner.OnHurt -= OnHurt;
            owner.OnKillEntity -= OnKill;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}