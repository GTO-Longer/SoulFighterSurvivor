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
                HUDUIRoot.Instance.buffInfo.chosenBuff.Value = GetComponent<BuffData>().buff;
            }
            else
            {
                HUDUIRoot.Instance.targetBuffInfo.chosenBuff.Value = GetComponent<BuffData>().buff;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (team == Team.Hero)
            {
                HUDUIRoot.Instance.buffInfo.chosenBuff.Value = null;
            }
            else
            {
                HUDUIRoot.Instance.targetBuffInfo.chosenBuff.Value = null;
            }
        }
    }
}