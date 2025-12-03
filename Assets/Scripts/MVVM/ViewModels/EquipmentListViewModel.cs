using System;
using System.Collections.Generic;
using Managers.EntityManagers;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class EquipmentListViewModel : MonoBehaviour
    {
        private List<Transform> equipmentSlotList;
        private Action UnBindEvent;

        private void Start()
        {
            equipmentSlotList = new List<Transform>();

            for (var index = 0; index < 6; index++)
            {
                // 直接捕获 Property<Equipment> 的局部副本
                var equipmentProp = HeroManager.hero.equipmentList[index];
                
                var slotTransform = transform.GetChild(index);
                equipmentSlotList.Add(slotTransform);

                var data = slotTransform.GetComponent<DataManagement.EquipmentData>();
                data.equipment = equipmentProp.Value;

                UnBindEvent += Binder.BindEquipmentImage(slotTransform.GetComponent<Image>(), equipmentProp);

                var button = slotTransform.GetComponent<Components.UI.EquipmentSlotButton>();
                if (button != null)
                {
                    button.leftClick = () =>
                    {
                        var current = equipmentProp.Value;
                        if (current != null)
                        {
                            EquipmentInfoViewModel.Instance.ShowEquipmentInfo(current);
                        }
                    };

                    button.rightClick = () =>
                    {
                        var current = equipmentProp.Value;
                        if (current != null)
                        {
                            HeroManager.hero.SellEquipment(current);
                        }
                    };
                }
            }
        }

        private void OnDestroy()
        {
            // 安全注销
            UnBindEvent?.Invoke();
        }
    }
}
