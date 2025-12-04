using System;
using System.Collections.Generic;
using Components.UI;
using DataManagement;
using Managers;
using Managers.EntityManagers;
using MVVM.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Systems
{
    public class ShopSystem : MonoBehaviour
    {
        public static ShopSystem Instance;
        public Transform starterEquipmentArea;
        public Transform legendEquipmentArea;
        public GameObject equipmentSlotPrefab;
        private Button purchaseButton;
        private List<GameObject> equipmentSlots = new();
        private EquipmentInfoViewModel equipmentInfoViewModel;
        
        private void Start()
        {
            Instance = this;
            equipmentSlotPrefab.SetActive(false);
            gameObject.SetActive(false);
            equipmentInfoViewModel = transform.Find("EquipmentInfo").GetComponent<EquipmentInfoViewModel>();
            purchaseButton = transform.Find("EquipmentInfo/PurchaseButton").GetComponent<Button>();
            
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
                if (equipment.equipmentIcon != null)
                {
                    equipmentSlot.GetComponent<Image>().sprite = equipment.equipmentIcon;
                }
                equipmentSlot.GetComponent<EquipmentSlotButton>().leftClick += () => equipmentInfoViewModel.ShowEquipmentInfo(equipment);
                equipmentSlot.GetComponent<EquipmentSlotButton>().rightClick += () =>
                {
                    equipmentInfoViewModel.HideEquipmentInfo();
                    HeroManager.hero.PurchaseEquipment(equipment);
                    equipmentInfoViewModel.ShowEquipmentInfo(equipment);
                };
                equipmentSlot.SetActive(true);
            }
        }

        /// <summary>
        /// 关闭商店面板
        /// </summary>
        public void CloseShopPanel()
        {
            gameObject.SetActive(false);
            equipmentInfoViewModel.HideEquipmentInfo();
            PanelUIRoot.Instance.isShopOpen = false;
        }

        /// <summary>
        /// 打开商店面板
        /// </summary>
        public void OpenShopPanel()
        {
            gameObject.SetActive(true);
            PanelUIRoot.Instance.isShopOpen = true;
        }
    }
}