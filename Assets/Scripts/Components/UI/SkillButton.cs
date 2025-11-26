using EntityManagers;
using MVVM.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Components.UI
{
    public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Skill Settings")]
        [SerializeField]
        private SkillType skillType = SkillType.None;

        private Button button;
        private Image skillIcon;

        private void Awake()
        {
            button ??= GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
            
            skillIcon??= GetComponentInChildren<Image>();
            // TODO:添加技能图标读取
            // skillIcon.sprite = 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (skillType != SkillType.None)
            {
                SkillViewModel.chosenSkill.Value = HeroManager.hero.skillList[(int)skillType];
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SkillViewModel.chosenSkill.Value = null;
        }

        public void OnButtonClick()
        {
            // 触发技能
        }
    }
}