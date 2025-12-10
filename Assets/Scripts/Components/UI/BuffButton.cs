using DataManagement;
using MVVM.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Components.UI
{
    public class BuffButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Team team;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (team == Team.Hero)
            {
                BuffViewModel.chosenBuff.Value = GetComponent<BuffData>().buff;
            }
            else
            {
                TargetBuffViewModel.chosenBuff.Value = GetComponent<BuffData>().buff;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (team == Team.Hero)
            {
                BuffViewModel.chosenBuff.Value = null;
            }
            else
            {
                TargetBuffViewModel.chosenBuff.Value = null;
            }
        }
    }
}