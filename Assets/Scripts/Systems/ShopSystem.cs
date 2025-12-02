using System;
using System.Collections.Generic;
using DataManagement;
using Managers;
using MVVM.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Systems
{
    public class ShopSystem : MonoBehaviour
    {
        public Transform starterEquipmentArea;
        public Transform legendEquipmentArea;
        public GameObject equipmentSlotPrefab;
        public List<GameObject> equipmentSlots = new();
        
        private void Start()
        {
            equipmentSlotPrefab.SetActive(false);
            gameObject.SetActive(false);
            
            foreach (var equipment in EquipmentManager.Instance.equipmentList)
            {
                switch (equipment._equipmentType)
                {
                    case EquipmentType.None:
                        Debug.LogError(equipment.equipmentName + " is an unknown equipment type");
                        break;
                    case EquipmentType.Starter:
                        equipmentSlots.Add(Instantiate(equipmentSlotPrefab, starterEquipmentArea));
                        break;
                    case EquipmentType.Legend:
                        equipmentSlots.Add(Instantiate(equipmentSlotPrefab, legendEquipmentArea));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var equipmentSlot = equipmentSlots[^1];
                equipmentSlot.GetComponent<EquipmentData>().equipment = equipment;
                equipmentSlot.GetComponent<Button>().onClick.AddListener(() => EquipmentInfoViewModel.Instance.ShowEquipmentInfo(equipment));
                equipmentSlot.SetActive(true);
            }
        }

        /// <summary>
        /// 关闭商店面板
        /// </summary>
        public void CloseShopPanel()
        {
            gameObject.SetActive(false);
            EquipmentInfoViewModel.Instance.HideEquipmentInfo();
        }

        /// <summary>
        /// 打开商店面板
        /// </summary>
        public void OpenShopPanel()
        {
            gameObject.SetActive(true);
        }
    }
}