using MVVM;
using MVVM.ViewModels;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class GoustPoro : Skill
    {
        public GoustPoro() : base("GoustPoro")
        {
            _skillLevel = 1;
            _maxSkillLevel = 1;
            
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
                owner.OnDSkillRelease += (_, _) => { GoustPoroEffective(); };
            }
            else
            {
                owner.OnFSkillRelease += (_, _) => { GoustPoroEffective(); };
            }
        }

        private void GoustPoroEffective()
        {
            if (actualSkillCoolDown > coolDownTimer)
            {
                Binder.ShowText(SkillViewModel.Instance.skillTips, "技能正在冷却", 1);
                return;
            }

            var goustPoro = new Buffs.GoustPoro(owner, owner);
            owner.GetBuff(goustPoro);
            coolDownTimer = 0;
        }
    }
}
