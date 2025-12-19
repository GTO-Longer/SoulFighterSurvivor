using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Components.UI
{
    public class HexSlotButton : Button
    {
        public Action leftClick = () => { };
        public Action rightClick = () => { };
        public Action onPointerEnter;
        public Action onPointerExit;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            onPointerEnter?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke();
        }
    }
}
