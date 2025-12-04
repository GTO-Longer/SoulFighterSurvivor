using MVVM;
using MVVM.ViewModels;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class Flash : Skill
    {
        public Flash() : base("Flash")
        {
            _skillLevel = 1;
            _maxSkillLevel = 1;
            base.skillType = skillType;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ": Skill effective");
            if (skillType == SkillType.DSkill)
            {
                owner.OnDSkillRelease += (_, _) => { HeroFlash(); };
            }
            else
            {
                owner.OnFSkillRelease += (_, _) => { HeroFlash(); };
            }
        }

        private void HeroFlash()
        {
                
            if (actualSkillCoolDown > coolDownTimer)
            {
                Binder.ShowText(SkillViewModel.Instance.skillTips, "技能正在冷却", 1);
                return;
            }

            if (owner.canFlash)
            {
                var direction = (owner._mousePosition - (Vector2)owner.gameObject.transform.position).normalized;
                var distance = Mathf.Min(Vector2.Distance(owner._mousePosition, owner.gameObject.transform.position),
                    _destinationDistance);
                owner.Flash(direction, distance);
                coolDownTimer = 0;
            }
        }
    }
}
