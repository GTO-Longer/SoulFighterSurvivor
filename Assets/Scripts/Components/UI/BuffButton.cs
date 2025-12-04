using DataManagement;
using MVVM.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Components.UI
{
    public class BuffButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            BuffViewModel.chosenBuff.Value = GetComponent<BuffData>().buff;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            BuffViewModel.chosenBuff.Value = null;
        }
    }
}