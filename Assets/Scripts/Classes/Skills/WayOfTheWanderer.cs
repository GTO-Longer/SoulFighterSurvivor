using UnityEngine;

namespace Classes.Skills
{
    public class WayOfTheWanderer : Skill
    {
        private float healCount => 50 + owner.level.Value / 3 * 40;
        private float moveDistance;
        
        public WayOfTheWanderer() : base("WayOfTheWanderer")
        {
            _skillLevel = 0;
            _maxSkillLevel = 0;

            PassiveAbilityEffective += () =>
            {
                owner._CRToAD_ConversionEfficiency.Value += new Vector2(0, 50f);
                owner._percentageCriticalRateBonus.Value += 1;
                owner._criticalDamageBonus.Value -= 0.2f;
                
                owner.maxEnergy.Value = 100;
                owner.energy.Value = owner.maxEnergy.Value;

                owner._percentageMaxMagicPointBonus.Value = 0;
                
                owner.OnHurt += (_, _, _) =>
                {
                    if (owner.energy >= owner.maxEnergy)
                    {
                        owner.TakeHeal(healCount);
                        owner.energy.Value = 0;
                    }
                };
                
                owner.EntityUpdateEvent += (_) =>
                {
                    if (!owner.agent.isStopped)
                    {
                        moveDistance += owner.movementSpeed.Value * Time.deltaTime;
                    }

                    if (moveDistance >= 62.5f)
                    {
                        moveDistance -= 62.5f;
                        
                        owner.energy.Value += 5;
                        owner.energy.Value = Mathf.Min(owner.energy, owner.maxEnergy);
                    }
                };
            };
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, healCount);
        }
    }
}