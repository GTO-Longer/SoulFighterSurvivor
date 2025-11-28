using System;
using EntityManagers;
using MVVM.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace Components.UI
{
    public class AttributeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Attribute Settings")]
        [SerializeField]
        private AttributeType attributeType = AttributeType.None;

        private Button button;

        private void Awake()
        {
            button ??= GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);

            if (attributeType == AttributeType.None)
            {
                attributeType = (AttributeType)Enum.Parse( typeof (AttributeType), gameObject.name);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (attributeType != AttributeType.None)
            {
                AttributeViewModel.chosenAttribute.Value = attributeType;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AttributeViewModel.chosenAttribute.Value = AttributeType.None;
        }

        public void OnButtonClick()
        {
            // 触发技能
        }
    }
}