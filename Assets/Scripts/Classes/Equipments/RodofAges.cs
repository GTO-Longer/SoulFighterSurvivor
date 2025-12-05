using System;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class RodofAges : Equipment
    {
        private Action<Entity, Entity, float, Skill> equipmentEffect;
        private Action<Entity, Entity> OnKill;
        private Action<Entity, Entity, float> OnHurt;
        private int killCount;
        private bool completed;
        
        public RodofAges() : base("RodofAges")
        {
            killCount = 0;
            completed = false;
            
            equipmentEffect = (attacker, _, _, skill) =>
            {
                attacker.TakeHeal(skill.actualSkillCost * 0.25f);
            };

            OnHurt = (self, _, damageCount) =>
            {
                self.TakeMagicRecover(damageCount * 0.15f);
            };
            
            OnKill = (_, _) =>
            {
                killCount += 1;

                if (killCount >= 50 && !completed)
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
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.AbilityEffect += equipmentEffect;
            owner.OnHurt += OnHurt;
            owner.OnKillEntity += OnKill;
        }

        public override void OnEquipmentRemove()
        {
            owner.AbilityEffect -= equipmentEffect;
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