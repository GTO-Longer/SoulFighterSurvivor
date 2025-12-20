using System;
using System.Collections.Generic;
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
        private Transform choicePrefab;
        public static ChoiceSystem Instance;
        private Dictionary<Choice, GameObject> choiceDictionary = new();

        public void Initialize()
        {
            Instance = this;
            choicePrefab = transform.Find("Choices/ChoicePrefab");
            choicePrefab.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void MakeChoice(params Choice[] choices)
        {
            if(choices.Length <= 0) return;
            
            gameObject.SetActive(true);
            PanelUIRoot.Instance.isChoiceOpen = true;
            
            // 遍历选项并创建UI
            foreach (var choice in choices)
            {
                var newChoice = Instantiate(choicePrefab.gameObject, choicePrefab.parent);
                var choiceIcon = newChoice.transform.Find("ChoiceIcon").GetComponent<Image>();
                var choiceIconBorder = newChoice.transform.Find("ChoiceIconBorder").GetComponent<Image>();
                var choiceBorder = newChoice.transform.Find("ChoiceBorder").GetComponent<Image>();
                
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
                choiceIcon.material = material;
                choiceBorder.material = material;
                
                // 设置图标边框颜色
                switch (choice.choiceQuality)
                {
                    case Quality.None: choiceIconBorder.gameObject.SetActive(false); break;
                    case Quality.Silver: choiceIconBorder.color = Colors.GetColor(Colors.SilverBorder); break;
                    case Quality.Gold: choiceIconBorder.color = Colors.GetColor(Colors.GoldBorder); break;
                    case Quality.Prismatic: choiceIconBorder.color = Colors.GetColor(Colors.PrismaticBorder); break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // 将有材质的素材加入材质更新
                if (material != null)
                {
                    ShaderManager.Instance.AddMaterial(choiceIcon.material);
                    ShaderManager.Instance.AddMaterial(choiceBorder.material);
                }

                choiceDictionary.Add(choice, newChoice);
            }

            var index = 0;
            foreach (var kv in choiceDictionary)
            {
                kv.Value.transform.localScale = new Vector3(0, 0, 0);
                kv.Value.SetActive(true);
                var choiceIndex = index;
                
                // 设置选项依次显示
                Async.SetAsync(0f, null, null, 
                    () =>
                    {
                        Async.SetAsync(0.2f * choiceIndex, null, null, 
                            () =>
                            {
                                kv.Value.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuad)
                                    .SetUpdate(UpdateType.Normal, true);
                            }).SetUpdate(true);
                    }).SetUpdate(true);
                index++;
            }
        }

        public void ClearChoices()
        {
            foreach (var kv in choiceDictionary)
            {
                if (kv.Key.choiceQuality is Quality.Prismatic or Quality.Gold)
                {
                    ShaderManager.Instance.RemoveMaterial(kv.Value.transform.Find("ChoiceIcon").GetComponent<Image>().material);
                }

                Destroy(kv.Value);
            }
            choiceDictionary.Clear();
            gameObject.SetActive(false);
            PanelUIRoot.Instance.isChoiceOpen = false;
        }
    }
}