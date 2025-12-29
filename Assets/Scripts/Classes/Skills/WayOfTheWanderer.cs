using UnityEngine;

namespace Classes.Skills
{
    public class WayOfTheWanderer : Skill
    {
        private float healCount => 100 + owner.level.Value / 3 * 75;
        private float moveDistance;
        
        public WayOfTheWanderer() : base("WayOfTheWanderer")
        {
            _skillLevel = 0;
            _maxSkillLevel = 0;

            PassiveAbilityEffective += () =>
            {
                owner._CRToAD_ConversionEfficiency.Value += new Vector2(0, 0.5f);
                owner._percentageCriticalRateBonus.Value += 1;
                owner._criticalDamageBonus.Value -= 0.2f;
                
                owner.OnHurt += (_, _, _) =>
                {
                    if (owner.magicPoint >= owner.maxMagicPoint)
                    {
                        owner.TakeHeal(healCount);
                        owner.magicPoint.Value = 0;
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
                        
                        owner.magicPoint.Value += 5;
                        owner.magicPoint.Value = Mathf.Min(owner.magicPoint, owner.maxMagicPoint);
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