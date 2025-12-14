using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Components.UI
{
    public class HexButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(1.1f, 0.25f).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(1f, 0.25f).SetEase(Ease.OutQuad).SetUpdate(true);
        }
    }
}
