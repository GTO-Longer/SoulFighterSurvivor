using System;
using System.Collections.Generic;
using Components.UI;
using DataManagement;
using Managers.EntityManagers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class EquipmentListViewModel : MonoBehaviour
    {
        private List<Transform> equipmentSlotList;
        private Action UnBindEvent;
        public EquipmentInfoViewModel equipmentInfoViewModel;
        public EquipmentInfoViewModel localEquipmentInfoShow;

        private void Start()
        {
            equipmentSlotList = new List<Transform>();

            for (var index = 0; index < 6; index++)
            {
                var slotIndex = index;
                var equipmentProp = HeroManager.hero.equipmentList[slotIndex];
                var slotTransform = transform.GetChild(slotIndex);
                equipmentSlotList.Add(slotTransform);

                void OnChange(object sender, EventArgs e)
                {
                    slotTransform.GetComponent<EquipmentData>().equipment = equipmentProp.Value;
                }

                equipmentProp.PropertyChanged += OnChange;

                UnBindEvent += Binder.BindEquipmentImage(slotTransform.GetComponent<Image>(), equipmentProp);

                var button = slotTransform.GetComponent<EquipmentSlotButton>();
                if (button != null)
                {
                    button.canDrag = true;
                    button.leftClick = () =>
                    {
                        var current = button.GetComponent<EquipmentData>().equipment;
                        if (current != null)
                        {
                            if (PanelUIRoot.Instance.isShopOpen)
                            {
                                equipmentInfoViewModel.ShowEquipmentInfo(current);
                            }
                        }
                    };

                    button.rightClick = () =>
                    {
                        var current = button.GetComponent<EquipmentData>().equipment;
                        if (current != null)
                        {
                            if (PanelUIRoot.Instance.isShopOpen)
                            {
                                equipmentInfoViewModel.HideEquipmentInfo();
                                HeroManager.hero.SellEquipment(current);
                                equipmentInfoViewModel.ShowEquipmentInfo(current);
                            }
                        }
                    };

                    // 拖拽交换
                    button.onDragEnd = () => SwapSlot(slotIndex);
                    
                    button.onPointerEnter = () =>
                    {
                        var current = equipmentProp.Value;
                        if (current != null && !PanelUIRoot.Instance.isShopOpen)
                        {
                            localEquipmentInfoShow.gameObject.SetActive(true);
                            localEquipmentInfoShow.ShowEquipmentInfo(current);
                        }
                    };

                    button.onPointerExit = () =>
                    {
                        localEquipmentInfoShow.gameObject.SetActive(false);
                    };
                }
            }
        }

        // 拖拽槽位交换
        private void SwapSlot(int from)
        {
            var to = -1;

            Vector2 mousePos = Input.mousePosition;

            var results = new List<RaycastResult>();
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = mousePos
            };
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var r in results)
            {
                var btn = r.gameObject.GetComponentInParent<EquipmentSlotButton>();
                if (btn != null && btn.canDrag && !btn.isDragging)
                {
                    to = btn.transform.GetSiblingIndex();
                    break;
                }
            }

            if (to == -1 || to == from) return;

            var list = HeroManager.hero.equipmentList;

            (list[from].Value, list[to].Value) = (list[to].Value, list[from].Value);

            // 更新 UI 数据
            equipmentSlotList[from].GetComponent<DataManagement.EquipmentData>().equipment = list[from].Value;
            equipmentSlotList[to].GetComponent<DataManagement.EquipmentData>().equipment = list[to].Value;
        }


        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
