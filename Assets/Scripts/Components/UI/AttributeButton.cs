using System;
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

            if (attributeType == AttributeType.None)
            {
                attributeType = (AttributeType)Enum.Parse( typeof (AttributeType), gameObject.name);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (attributeType != AttributeType.None)
            {
                HUDUIRoot.Instance.attributeInfo.BindAttributePanel(attributeType);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HUDUIRoot.Instance.attributeInfo.UnBindAttribute();
        }
    }
}