using System;
using Classes;
using DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class SkillViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;
        public static SkillViewModel instance;
        public static Property<Skill> chosenSkill = new Property<Skill>();
        public TMP_Text skillTips;

        private void Start()
        {
            instance = this;
            
            var skillIcon = transform.Find("Background/TitleBanner/SkillIconContainer/SkillIcon").GetComponent<Image>();
            var skillName = transform.Find("Background/TitleBanner/SkillNameAndLevelContainer/SkillName").GetComponent<TMP_Text>();
            var skillLevel = transform.Find("Background/TitleBanner/SkillNameAndLevelContainer/SkillLevel").GetComponent<TMP_Text>();
            var skillCoolDown = transform.Find("Background/TitleBanner/CoolDownAndCostContainer/CoolDown").GetComponent<TMP_Text>();
            var skillCost = transform.Find("Background/TitleBanner/CoolDownAndCostContainer/Cost").GetComponent<TMP_Text>();
            var skillDescription = transform.Find("Background/Description").GetComponent<TMP_Text>();
            var background = transform.Find("Background").gameObject;
            skillTips = transform.Find("SkillTips").GetComponent<TMP_Text>();
            
            skillTips.gameObject.SetActive(false);
            
            UnBindEvent += Binder.BindSkill(skillIcon, background, skillName, skillLevel, skillCoolDown, skillCost, skillDescription, chosenSkill);
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
