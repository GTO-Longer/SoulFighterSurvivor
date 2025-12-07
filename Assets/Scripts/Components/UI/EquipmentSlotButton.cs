using System;
using DataManagement;
using MVVM.ViewModels;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Components.UI
{
    public class EquipmentSlotButton : Button, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Action leftClick = () => { };
        public Action rightClick = () => { };
        public Action onDragEnd;
        public Action onPointerEnter;
        public Action onPointerExit;

        public bool canDrag;
        public bool isDragging;
        private RectTransform rectTransform;
        private Vector2 originalPosition;
        private int originalSiblingIndex;
        private GameObject tempIcon;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (canDrag && !isDragging)
            {
                onPointerEnter?.Invoke();
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (canDrag && !isDragging)
            {
                onPointerExit?.Invoke();
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!isDragging)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                    leftClick.Invoke();
                else if (eventData.button == PointerEventData.InputButton.Right)
                    rightClick.Invoke();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canDrag || GetComponent<EquipmentData>().equipment == null) return;

            isDragging = true;
            originalPosition = rectTransform.position;
            tempIcon = Instantiate(gameObject, transform.parent);
            tempIcon.GetComponent<Image>().sprite = null;
            tempIcon.transform.Find("CDMask").GetComponent<Image>().enabled = false;
            tempIcon.transform.Find("EquipmentCD").GetComponent<TMP_Text>().enabled = false;
            tempIcon.transform.Find("ChargeCount").GetComponent<TMP_Text>().enabled = false;
            tempIcon.transform.SetSiblingIndex(originalSiblingIndex);
            originalSiblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag || GetComponent<EquipmentData>().equipment == null) return;
            rectTransform.position = MousePointSystem.Instance._rectTransform.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag || GetComponent<EquipmentData>().equipment == null) return;

            onDragEnd?.Invoke();

            isDragging = false;
            rectTransform.position = originalPosition;
            transform.SetSiblingIndex(originalSiblingIndex);
            Destroy(tempIcon);
        }
    }
}
