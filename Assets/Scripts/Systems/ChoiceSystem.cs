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
        
        private void Start()
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
            foreach (var choice in choices)
            {
                var newChoice = Instantiate(choicePrefab.gameObject, choicePrefab.parent);
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
                newChoice.transform.Find("ChoiceIcon").GetComponent<Image>().sprite = choice.choiceIcon;
                
                // 设置材质
                var material = choice.choiceQuality switch
                {
                    Quality.None => null,
                    Quality.Silver => null,
                    Quality.Gold => ResourceReader.ReadMaterial("Gold"),
                    Quality.Prismatic => ResourceReader.ReadMaterial("Prismatic"),
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (choice.choiceQuality == Quality.Prismatic)
                {
                    newChoice.transform.Find("ChoiceBorder").GetComponent<Image>().material = material;
                    newChoice.transform.Find("ChoiceIcon").GetComponent<Image>().material = material;
                }

                if (choice.choiceQuality == Quality.Prismatic)
                {
                    ShaderManager.Instance.AddMaterial(newChoice.transform.Find("ChoiceBorder").GetComponent<Image>().material);
                    ShaderManager.Instance.AddMaterial(newChoice.transform.Find("ChoiceIcon").GetComponent<Image>().material);
                }

                choiceDictionary.Add(choice, newChoice);
            }

            var index = 0;
            foreach (var kv in choiceDictionary)
            {
                kv.Value.transform.localScale = new Vector3(0, 0, 0);
                kv.Value.SetActive(true);
                var choiceIndex = index;
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

        private void ClearChoices()
        {
            gameObject.SetActive(false);
            foreach (var kv in choiceDictionary)
            {
                if (kv.Key.choiceQuality == Quality.Prismatic)
                {
                    ShaderManager.Instance.RemoveMaterial(kv.Value.transform.Find("ChoiceBorder").GetComponent<Image>().material);
                    ShaderManager.Instance.RemoveMaterial(kv.Value.transform.Find("ChoiceIcon").GetComponent<Image>().material);
                }

                Destroy(kv.Value);
            }
            choiceDictionary.Clear();
            PanelUIRoot.Instance.isChoiceOpen = false;
        }
    }
}