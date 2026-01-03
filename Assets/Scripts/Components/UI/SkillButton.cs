using Managers.EntityManagers;
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
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (skillType != SkillType.None)
            {
                HUDUIRoot.Instance.skillInfo.chosenSkill.Value = HeroManager.hero.skillList[(int)skillType];
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HUDUIRoot.Instance.skillInfo.chosenSkill.Value = null;
        }

        public void OnButtonClick()
        {
            // 触发技能
        }
    }
}