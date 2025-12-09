using System;
using System.Collections.Generic;
using Classes;
using DataManagement;
using Managers.EntityManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MVVM.ViewModels
{
    public class EquipmentInfoViewModel : MonoBehaviour
    {
        private Image equipmentIcon;
        private TMP_Text equipmentName;
        private TMP_Text usageDescription;
        private TMP_Text equipmentCost;
        private Button purchaseButton;
        public GameObject entryPrefab;
        public List<GameObject> entryList;
        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            equipmentIcon = transform.Find("EquipmentTitle/EquipmentIcon").GetComponent<Image>();
            equipmentName = transform.Find("EquipmentTitle/NameAndDescription/EquipmentName").GetComponent<TMP_Text>();
            usageDescription = transform.Find("EquipmentTitle/NameAndDescription/UsageDescription").GetComponent<TMP_Text>();
            equipmentCost = transform.Find("EquipmentTitle/EquipmentCost").GetComponent<TMP_Text>();
            purchaseButton = transform.Find("PurchaseButton")?.GetComponent<Button>();

            entryPrefab.SetActive(false);
            HideEquipmentInfo();
        }

        public void ShowEquipmentInfo(Equipment equipment)
        {
            if (equipment != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                ClearEntryList();

                equipmentName.text = equipment.equipmentName;
                usageDescription.text = equipment._usageDescription;
                equipmentCost.text = $"<sprite=\"Coin\" index=0>{equipment._cost}";
                equipmentIcon.sprite = equipment.equipmentIcon;

                if (purchaseButton != null)
                {
                    purchaseButton.onClick.RemoveAllListeners();
                    
                    var uniqueCheck = HeroManager.hero.equipmentList.Find(equip =>
                        equip.Value != null && equip.Value._uniqueEffect == equipment._uniqueEffect);

                    if (!equipment.canPurchase)
                    {
                        purchaseButton.transform.Find("PurchaseContent").GetComponent<TMP_Text>().text =
                            "该装备不可购买";
                        purchaseButton.interactable = false;
                    }else if ((equipment._uniqueEffect != EquipmentUniqueEffect.None && uniqueCheck == null) ||
                        equipment._uniqueEffect == EquipmentUniqueEffect.None || uniqueCheck.Value == equipment)
                    {
                        if (HeroManager.hero.equipmentList.Find(equip => equip.Value == equipment) == null)
                        {
                            purchaseButton.transform.Find("PurchaseContent").GetComponent<TMP_Text>().text =
                                $"以{equipment._cost:D}金币购买 " + equipment.equipmentName;
                            purchaseButton.onClick.AddListener(() =>
                            {
                                HeroManager.hero.PurchaseEquipment(equipment);
                                HideEquipmentInfo();
                                ShowEquipmentInfo(equipment);
                            });
                            purchaseButton.interactable = true;
                        }
                        else
                        {
                            purchaseButton.transform.Find("PurchaseContent").GetComponent<TMP_Text>().text =
                                $"以{(int)(equipment._cost * 0.7f):D}金币售出 " + equipment.equipmentName;
                            purchaseButton.onClick.AddListener(() =>
                            {
                                HeroManager.hero.SellEquipment(equipment);
                                HideEquipmentInfo();
                                ShowEquipmentInfo(equipment);
                            });
                            purchaseButton.interactable = true;
                        }
                    }
                    else
                    {
                        purchaseButton.transform.Find("PurchaseContent").GetComponent<TMP_Text>().text =
                        $"与装备{uniqueCheck.Value.equipmentName}独一性冲突";
                        purchaseButton.interactable = false;
                    }
                }

                foreach (var kv in equipment.equipmentAttributes)
                {
                    entryList.Add(Instantiate(entryPrefab, entryPrefab.transform.parent));

                    var entryContent = "";
                    // 获取装备属性对应的属性描述
                    switch (kv.Key)
                    {
                        case EquipmentAttributeType.None:
                            Debug.LogError("未找到对应属性！");
                            break;
                        case EquipmentAttributeType.maxHealthPoint:
                            entryContent = $"<color=#0ACD02>+{kv.Value:F0}最大生命值</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MaxHealthPointIcon");
                            break;
                        case EquipmentAttributeType.percentageMaxHealthPoint:
                            entryContent = $"<color=#0ACD02>+{kv.Value:P0}最大生命值</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MaxHealthPointIcon");
                            break;
                        case EquipmentAttributeType.maxMagicPoint:
                            entryContent = $"<color=#1DA1FF>+{kv.Value:F0}最大法力值</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MaxMagicPointIcon");
                            break;
                        case EquipmentAttributeType.percentageMaxMagicPoint:
                            entryContent = $"<color=#1DA1FF>+{kv.Value:P0}最大法力值</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MaxMagicPointIcon");
                            break;
                        case EquipmentAttributeType.attackSpeed:
                            entryContent = $"<color=#E0C300>+{kv.Value:P0}攻击速度</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackSpeedIcon");
                            break;
                        case EquipmentAttributeType.percentageAttackSpeed:
                            entryContent = $"<color=#E0C300>+{kv.Value:P0}总攻击速度加成</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackSpeedIcon");
                            break;
                        case EquipmentAttributeType.attackDamage:
                            entryContent = $"<color=#FF6161>+{kv.Value:F0}攻击力</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackDamageIcon");
                            break;
                        case EquipmentAttributeType.percentageAttackDamage:
                            entryContent = $"<color=#FF6161>+{kv.Value:P0}攻击力</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackDamageIcon");
                            break;
                        case EquipmentAttributeType.abilityPower:
                            entryContent = $"<color=#C97BFF>+{kv.Value:F0}法术强度</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AbilityPowerIcon");
                            break;
                        case EquipmentAttributeType.percentageAbilityPower:
                            entryContent = $"<color=#C97BFF>+{kv.Value:P0}法术强度</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AbilityPowerIcon");
                            break;
                        case EquipmentAttributeType.abilityHaste:
                            entryContent = $"<color=#FFE761>+{kv.Value:F0}技能急速</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AbilityHasteIcon");
                            break;
                        case EquipmentAttributeType.percentageAbilityHaste:
                            entryContent = $"<color=#FFE761>+{kv.Value:P0}技能急速</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AbilityHasteIcon");
                            break;
                        case EquipmentAttributeType.attackDefense:
                            entryContent = $"<color=#FFD700>+{kv.Value:F0}物理防御</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackDefenseIcon");
                            break;
                        case EquipmentAttributeType.percentageAttackDefense:
                            entryContent = $"<color=#FFD700>+{kv.Value:P0}物理防御</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackDefenseIcon");
                            break;
                        case EquipmentAttributeType.magicDefense:
                            entryContent = $"<color=#6AA2FF>+{kv.Value:F0}法术防御</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MagicDefenseIcon");
                            break;
                        case EquipmentAttributeType.percentageMagicDefense:
                            entryContent = $"<color=#6AA2FF>+{kv.Value:P0}法术防御</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MagicDefenseIcon");
                            break;
                        case EquipmentAttributeType.attackPenetration:
                            entryContent = $"<color=#FF6A6A>+{kv.Value:F0}物理穿透</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackPenetrationIcon");
                            break;
                        case EquipmentAttributeType.percentageAttackPenetration:
                            entryContent = $"<color=#FF6A6A>+{kv.Value:P0}物理穿透</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "AttackPenetrationIcon");
                            break;
                        case EquipmentAttributeType.magicPenetration:
                            entryContent = $"<color=#9E4EFF>+{kv.Value:F0}法术穿透</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MagicPenetrationIcon");
                            break;
                        case EquipmentAttributeType.percentageMagicPenetration:
                            entryContent = $"<color=#9E4EFF>+{kv.Value:P0}法术穿透</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MagicPenetrationIcon");
                            break;
                        case EquipmentAttributeType.criticalRate:
                            entryContent = $"<color=#FFB74D>+{kv.Value:P0}暴击率</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "CriticalRateIcon");
                            break;
                        case EquipmentAttributeType.criticalDamage:
                            entryContent = $"<color=#FFA347>+{kv.Value:P0}暴击伤害</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "CriticalDamageIcon");
                            break;
                        case EquipmentAttributeType.movementSpeed:
                            entryContent = $"<color=#FFFFFF>+{kv.Value:F0}移动速度</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MovementSpeedIcon");
                            break;
                        case EquipmentAttributeType.percentageMovementSpeed:
                            entryContent = $"<color=#FFFFFF>+{kv.Value:P0}移动速度</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MovementSpeedIcon");
                            break;
                        case EquipmentAttributeType.percentageHealthRegeneration:
                            entryContent = $"<color=#20FF20>+{kv.Value:P0}生命值回复</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MaxHealthPointIcon");
                            break;
                        case EquipmentAttributeType.percentageMagicRegeneration:
                            entryContent = $"<color=#80D8FF>+{kv.Value:P0}法力值回复</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "MaxMagicPointIcon");
                            break;
                        case EquipmentAttributeType.omnivamp:
                            entryContent = $"<color=#6E1BA8>+{kv.Value:P0}全能吸血</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "OmniVampIcon");
                            break;
                        case EquipmentAttributeType.lifeSteal:
                            entryContent = $"<color=#FF6666>+{kv.Value:P0}生命偷取</color>";
                            entryList[^1].transform.Find("EntryIcon").GetComponent<Image>().sprite =
                                ResourceReader.ReadIcon("Attributes", "LifeStealIcon");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    entryList[^1].transform.Find("EntryContent").GetComponent<TMP_Text>().text = entryContent;
                    entryList[^1].SetActive(true);
                }

                if (equipment.GetPassiveSkillDescription(out var passiveContent))
                {
                    entryList.Add(Instantiate(entryPrefab, entryPrefab.transform.parent));
                    entryList[^1].transform.Find("EntryContent").GetComponent<TMP_Text>().text = passiveContent;
                    entryList[^1].SetActive(true);
                }

                if (equipment.GetActiveSkillDescription(out var activeContent))
                {
                    entryList.Add(Instantiate(entryPrefab, entryPrefab.transform.parent));
                    entryList[^1].transform.Find("EntryContent").GetComponent<TMP_Text>().text = activeContent;
                    entryList[^1].SetActive(true);
                }

                // 更新UI布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(entryPrefab.transform.parent.GetComponent<RectTransform>());
            }
        }

        public void HideEquipmentInfo()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            ClearEntryList();
        }

        public void ClearEntryList()
        {
            foreach (var entry in entryList)
            {
                entry.gameObject.SetActive(false);
            }
            entryList.Clear();
        }
    }
}