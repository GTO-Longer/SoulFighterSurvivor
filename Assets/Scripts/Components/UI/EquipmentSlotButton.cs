using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Components.UI
{
    public class EquipmentSlotButton : Button
    {
        public Action leftClick = () => { };
        public Action rightClick = () => { };


        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                leftClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                rightClick.Invoke();
        }
    }
}