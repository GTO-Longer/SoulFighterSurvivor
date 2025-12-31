using DG.Tweening;
using UnityEngine;

namespace Classes.Skills
{
    public class Flash : Skill
    {
        public Flash() : base("Flash")
        {
            _skillLevel = 1;
            _maxSkillLevel = 1;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription);
        }

        public override bool SkillEffect()
        {
            if (owner.canFlash)
            {
                var direction = (owner._mousePosition - (Vector2)owner.gameObject.transform.position).normalized;
                var distance = Mathf.Min(Vector2.Distance(owner._mousePosition, owner.gameObject.transform.position), _destinationDistance);
                
                owner.Flash(direction, distance);
                coolDownTimer = 0;

                return true;
            }
            
            return false;
        }
    }
}
