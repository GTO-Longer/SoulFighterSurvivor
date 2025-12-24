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
        private Transform itemView;
        private Transform starterEquipmentArea;
        private Transform anvilEquipmentArea;
        private Transform legendEquipmentArea;
        private Transform prismaticEquipmentArea;
        private Transform taskEquipmentArea;
        private GameObject equipmentSlotPrefab;
        private Button purchaseButton;
        private List<GameObject> equipmentSlots = new();
        private EquipmentInfoViewModel equipmentInfoViewModel;
        private CanvasGroup canvasGroup;
        private UsageType selectedUsage;
        private Transform selectors;

        public void Initialize()
        {
            Instance = this;
            selectedUsage = UsageType.None;
            
            selectors = transform.Find("UsageSelector");
            equipmentInfoViewModel = transform.Find("EquipmentInfo").GetComponent<EquipmentInfoViewModel>();
            purchaseButton = transform.Find("EquipmentInfo/PurchaseButton").GetComponent<Button>();
            itemView = transform.Find("ItemView/Viewport/Content");
            starterEquipmentArea = itemView.Find("Starter");
            anvilEquipmentArea = itemView.Find("Anvil");
            legendEquipmentArea = itemView.Find("Legend");
            prismaticEquipmentArea = itemView.Find("Prismatic");
            taskEquipmentArea = itemView.Find("Task");
            equipmentSlotPrefab = starterEquipmentArea.Find("EquipmentSlotPrefab").gameObject;
            canvasGroup = GetComponent<CanvasGroup>();
            
            equipmentInfoViewModel.Initialize();
            
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
                    case EquipmentType.Anvil:
                        equipmentSlots.Add(Instantiate(equipmentSlotPrefab, anvilEquipmentArea));
                        break;
                    case EquipmentType.Legend:
                        equipmentSlots.Add(Instantiate(equipmentSlotPrefab, legendEquipmentArea));
                        break;
                    case EquipmentType.Prismatic:
                        equipmentSlots.Add(Instantiate(equipmentSlotPrefab, prismaticEquipmentArea));
                        break;
                    case EquipmentType.Task:
                        equipmentSlots.Add(Instantiate(equipmentSlotPrefab, taskEquipmentArea));
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
            }
            
            equipmentSlotPrefab.SetActive(false);
                
            selectors.Find("AllButton").GetComponent<Button>().onClick.AddListener(() => UsageSelect(UsageType.None));
            selectors.Find("FighterButton").GetComponent<Button>().onClick.AddListener(() => UsageSelect(UsageType.Fighter));
            selectors.Find("WizardButton").GetComponent<Button>().onClick.AddListener(() => UsageSelect(UsageType.Wizard));
            selectors.Find("ShooterButton").GetComponent<Button>().onClick.AddListener(() => UsageSelect(UsageType.Shooter));
            selectors.Find("TankButton").GetComponent<Button>().onClick.AddListener(() => UsageSelect(UsageType.Tank));
            selectors.Find("AssassinButton").GetComponent<Button>().onClick.AddListener(() => UsageSelect(UsageType.Assassin));
            
            // 更新UI布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemView.GetComponent<RectTransform>());
            CloseShopPanel();
        }

        /// <summary>
        /// 关闭商店面板
        /// </summary>
        public void CloseShopPanel()
        {
            selectedUsage = UsageType.None;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            equipmentInfoViewModel.HideEquipmentInfo();
            PanelUIRoot.Instance.isShopOpen = false;
        }

        /// <summary>
        /// 打开商店面板
        /// </summary>
        public void OpenShopPanel()
        {
            itemView.parent.parent.GetComponent<ScrollRect>().verticalScrollbar.value = 1f;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            PanelUIRoot.Instance.isShopOpen = true;
        }

        public void UsageSelect(UsageType usageType)
        {
            selectedUsage = usageType;

            foreach (var equipmentSlot in equipmentSlots)
            {
                if (selectedUsage == UsageType.None)
                {
                    equipmentSlot.SetActive(true);
                    continue;
                }
                
                equipmentSlot.SetActive(equipmentSlot.GetComponent<EquipmentData>().equipment.usageTypes.Contains(selectedUsage));
            }
            
            // 更新UI布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemView.GetComponent<RectTransform>());
        }
    }
}