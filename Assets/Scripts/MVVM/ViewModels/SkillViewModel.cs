using System;
using Classes;
using DataManagement;
using TMPro;
using UnityEngine;

namespace MVVM.ViewModels
{
    public class SkillViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;

        public static Property<Skill> chosenSkill = new Property<Skill>();

        private void Start()
        {
            var skillName = transform.Find("Background/TitleBanner/SkillName").GetComponent<TMP_Text>();
            var skillCoolDown = transform.Find("Background/TitleBanner/CoolDownAndCostContainer/CoolDown").GetComponent<TMP_Text>();
            var skillCost = transform.Find("Background/TitleBanner/CoolDownAndCostContainer/Cost").GetComponent<TMP_Text>();
            var skillDescription = transform.Find("Background/Description").GetComponent<TMP_Text>();
            var background = transform.Find("Background").gameObject;
            
            UnBindEvent += Binder.BindSkill(background, skillName, skillCoolDown, skillCost, skillDescription, chosenSkill);
            chosenSkill.Value = null;
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
