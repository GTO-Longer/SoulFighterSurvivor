using System;
using System.Collections.Generic;
using Components.UI;
using DataManagement;
using Managers.EntityManagers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class EquipmentListViewModel : MonoBehaviour
    {
        public static EquipmentListViewModel Instance;
        private List<Transform> equipmentSlotList;
        private Action UnBindEvent;
        private EquipmentInfoViewModel equipmentInfoViewModel;
        private EquipmentInfoViewModel playerEquipmentInfoShow;

        private void Start()
        {
            Instance = this;
            equipmentSlotList = new List<Transform>();
            equipmentInfoViewModel = PanelUIRoot.Instance.shopSystem.transform.Find("EquipmentInfo").GetComponent<EquipmentInfoViewModel>();
            playerEquipmentInfoShow = transform.parent.Find("EquipmentInfo").GetComponent<EquipmentInfoViewModel>();

            // 绑定装备CD显示
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

                var equipmentCDMask = slotTransform.Find("CDMask").GetComponent<Image>();
                var equipmentCDText = slotTransform.Find("EquipmentCD").GetComponent<TMP_Text>();
                var chargeCount = slotTransform.Find("ChargeCount").GetComponent<TMP_Text>();
                UnBindEvent += Binder.BindEquipment(slotTransform.GetComponent<Image>(), equipmentCDMask, equipmentCDText, chargeCount, equipmentProp);

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
                            playerEquipmentInfoShow.gameObject.SetActive(true);
                            playerEquipmentInfoShow.ShowEquipmentInfo(current);
                        }
                    };

                    button.onPointerExit = () =>
                    {
                        playerEquipmentInfoShow.gameObject.SetActive(false);
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
            equipmentSlotList[from].GetComponent<EquipmentData>().equipment = list[from].Value;
            equipmentSlotList[to].GetComponent<EquipmentData>().equipment = list[to].Value;
        }

        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
