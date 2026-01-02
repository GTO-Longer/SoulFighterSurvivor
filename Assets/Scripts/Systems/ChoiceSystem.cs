using System;
using System.Collections.Generic;
using System.Linq;
using Components.UI;
using DataManagement;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Systems
{
    public class ChoiceSystem : MonoBehaviour
    {
        public static ChoiceSystem Instance;
        
        private Transform choicePrefab;
        private Dictionary<Choice, GameObject> choiceDictionary = new();
        private List<(bool, Choice[])> CacheList = new();
        private CanvasGroup _canvasGroup;
        private List<GameObject> refreshButtons = new();
        
        public int choiceIndex;

        public int diceCount;

        public void Initialize()
        {
            Instance = this;
            
            diceCount = 8;
            choicePrefab = transform.Find("Choices/ChoicePrefab");
            choicePrefab.gameObject.SetActive(false);
            _canvasGroup = GetComponent<CanvasGroup>();
            
            refreshButtons.Add(transform.Find("RefreshButtons/RefreshButton_1").gameObject);
            refreshButtons.Add(transform.Find("RefreshButtons/RefreshButton_2").gameObject);
            refreshButtons.Add(transform.Find("RefreshButtons/RefreshButton_3").gameObject);
            
            ClearChoices();
        }

        private void Update()
        {
            if (CacheList.Count > 0 && !PanelUIRoot.Instance.isPanelOpen)
            {
                MakeChoice(CacheList[0].Item1, CacheList[0].Item2);
                CacheList.RemoveAt(0);
            }
        }

        public void MakeChoice(bool canRefresh, params Choice[] choices)
        {
            if(choices.Length <= 0) return;
            
            if (PanelUIRoot.Instance.isPanelOpen)
            {
                CacheList.Add((canRefresh, choices));
                return;
            }
            
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            PanelUIRoot.Instance.isChoiceOpen = true;

            choiceIndex = 0;
            choicePrefab.parent.GetComponent<HorizontalLayoutGroup>().padding.bottom = canRefresh ? 75 : 0;
            
            // 遍历选项并创建UI
            foreach (var choice in choices)
            {
                if (choiceIndex >= 3)
                {
                    break;
                }
                
                var newChoice = Instantiate(choicePrefab.gameObject, choicePrefab.parent);
                var choiceIcon = newChoice.transform.Find("ChoiceIcon").GetComponent<Image>();
                var choiceIconBorder = newChoice.transform.Find("ChoiceIconBorder").GetComponent<Image>();
                var choiceBorder = newChoice.transform.Find("ChoiceBorder").GetComponent<Image>();

                // 刷新功能
                var refreshButton = refreshButtons[choiceIndex];
                refreshButton.SetActive(canRefresh);
                
                if (canRefresh)
                {
                    var button = refreshButton.GetComponent<Button>();
                    button.interactable = diceCount > 0;

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                    {
                        if (newChoice.transform.localScale.x < 0.9f) return;
                        
                        if(diceCount <= 0) return;
                        
                        // 获取一个随机的不重复的选项
                        if (!ToolFunctions.GetRandomUniqueItems(
                                choices.ToList().FindAll(obj => !choiceDictionary.ContainsKey(obj)), 1,
                                out var newResults))
                        {
                            Debug.LogWarning("[ChoiceSystem] 剩余选项数量不够");
                            return;
                        }
                        
                        diceCount -= 1;
                        newChoice.transform.localScale = new Vector3(0, 0, 0);
                        
                        foreach (var RButton in refreshButtons)
                        {
                            RButton.GetComponent<Button>().interactable = diceCount > 0;
                            RButton.transform.Find("Content").GetComponent<TMP_Text>().text = $"{diceCount:D}";
                        }
                        
                        var newChoiceContent = newResults[0];
                        
                        // 将字典原本的键替换为新的选项
                        choiceDictionary.Remove(choiceDictionary.FirstOrDefault(x => x.Value == newChoice).Key);
                        choiceDictionary.Add(newChoiceContent, newChoice);
                        
                        newChoice.GetComponent<Button>().onClick.RemoveAllListeners();
                        newChoice.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (newChoice.transform.localScale.x >= 0.9f)
                            {
                                newChoiceContent.OnSelected?.Invoke();
                                ClearChoices();
                            }
                        });
                        
                        newChoice.transform.Find("ChoiceTitle").GetComponent<TMP_Text>().text = newChoiceContent.choiceTitle;
                        newChoice.transform.Find("ChoiceContent").GetComponent<TMP_Text>().text = newChoiceContent.choiceContent;
                        choiceIcon.sprite = newChoiceContent.choiceIcon;
                        choiceIconBorder.sprite = newChoiceContent.choiceIconBorder;

                        ShowChoice(newChoice);
                    });
                }
                
                newChoice.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (newChoice.transform.localScale.x >= 0.9f)
                    {
                        choice.OnSelected?.Invoke();
                        ClearChoices();
                    }
                });
                newChoice.transform.Find("ChoiceTitle").GetComponent<TMP_Text>().text = choice.choiceTitle;
                newChoice.transform.Find("ChoiceContent").GetComponent<TMP_Text>().text = choice.choiceContent;
                choiceIcon.sprite = choice.choiceIcon;
                choiceIconBorder.sprite = choice.choiceIconBorder;
                
                // 设置材质
                var material = choice.choiceQuality switch
                {
                    Quality.None => null,
                    Quality.Silver => null,
                    Quality.Gold => ResourceReader.LoadMaterial("Gold"),
                    Quality.Prismatic => ResourceReader.LoadMaterial("Prismatic"),
                    _ => throw new ArgumentOutOfRangeException()
                };

                choiceIcon.material = !choice.rawColor ? material : null;
                choiceBorder.material = material;

                // 设置图标边框颜色
                switch (choice.choiceQuality)
                {
                    case Quality.None: choiceIconBorder.gameObject.SetActive(false); break;
                    case Quality.Silver: choiceIconBorder.color = Colors.GetColor(Colors.SilverBorder, 0.25f); break;
                    case Quality.Gold: choiceIconBorder.color = Colors.GetColor(Colors.GoldBorder, 0.25f); break;
                    case Quality.Prismatic: choiceIconBorder.color = Colors.GetColor(Colors.PrismaticBorder, 0.25f); break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // 将有材质的素材加入材质更新
                if (material != null)
                {
                    if (!choice.rawColor)
                    {
                        ShaderManager.Instance.AddMaterial(choiceIcon.material);
                    }

                    ShaderManager.Instance.AddMaterial(choiceBorder.material);
                }

                choiceDictionary.Add(choice, newChoice);
                choiceIndex++;
            }

            ShowChoices();
        }

        private void ShowChoices()
        {
            var index = 0;
            foreach (var kv in choiceDictionary)
            {
                kv.Value.transform.localScale = new Vector3(0, 0, 0);
                kv.Value.SetActive(true);
                var tempIndex = index;
                
                // 设置选项依次显示
                Async.SetAsync(0f, null, null, 
                () =>
                {
                    Async.SetAsync(0.2f * tempIndex, null, null, 
                    () =>
                    {
                        kv.Value.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);
                    }).SetUpdate(true);
                }).SetUpdate(true);
                index++;
            }
        }

        private void ShowChoice(GameObject choiceObj)
        {
            choiceObj.transform.localScale = new Vector3(0, 0, 0);
            choiceObj.SetActive(true);
                
            // 设置选项依次显示
            Async.SetAsync(0.2f , null, null, 
            () =>
            {
                choiceObj.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);
            }).SetUpdate(true);
        }

        private void ClearChoices()
        {
            foreach (var kv in choiceDictionary)
            {
                if (kv.Key.choiceQuality is Quality.Prismatic or Quality.Gold)
                {
                    if (!kv.Key.rawColor)
                    {
                        ShaderManager.Instance.RemoveMaterial(kv.Value.transform.Find("ChoiceIcon").GetComponent<Image>().material);
                    }

                    ShaderManager.Instance.RemoveMaterial(kv.Value.transform.Find("ChoiceBorder").GetComponent<Image>().material);
                }

                Destroy(kv.Value);
            }
            
            choiceDictionary.Clear();
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            PanelUIRoot.Instance.isChoiceOpen = false;
        }
    }
}