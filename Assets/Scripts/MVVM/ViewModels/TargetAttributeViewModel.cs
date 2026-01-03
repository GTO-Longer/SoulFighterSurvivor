using System;
using System.Collections.Generic;
using Classes;
using Components.UI;
using DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class TargetAttributeViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;
        public Property<Entity> checkTarget = new Property<Entity>();

        public void Initialize()
        {
            // 配置敌方buff信息显示
            var buffInfo = HUDUIRoot.Instance.targetBuffInfo;
            var buffInfoUI = buffInfo.GetComponent<RectTransform>();
            buffInfoUI.transform.SetParent(transform);
            buffInfoUI.anchoredPosition = new Vector2(250, -375);
            buffInfoUI.pivot = new Vector2(0.5f, 1);
            buffInfoUI.sizeDelta = new Vector2(500, 0);
            buffInfoUI.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(500, 0);
            buffInfoUI.anchorMin = new Vector2(0, 1);
            buffInfoUI.anchorMax = new Vector2(0, 1);
            
            var textGroup = new Dictionary<string, TMP_Text>();
            var attributeList = transform.GetComponentsInChildren<TMP_Text>();
            var HPContent = transform.Find("Background/HPBarBackground/HPContent").GetComponent<TMP_Text>();
            var HPBar = transform.Find("Background/HPBarBackground/HPBar").GetComponent<Image>();
            
            foreach (var _attribute in attributeList)
            {
                textGroup[_attribute.gameObject.name] = _attribute;
            }
            
            UnBindEvent += Binder.BindActive(gameObject, checkTarget);
            UnBindEvent += Binder.BindTextGroup(textGroup, checkTarget);
            UnBindEvent += Binder.BindHPGroup(HPContent, HPBar, checkTarget, "{0:F0} / {1:F0}");
            
            checkTarget.PropertyChanged += (_, _) =>
            {
                buffInfo.DeleteAllBuffUI();
                if (checkTarget.Value != null)
                {
                    foreach (var buff in checkTarget.Value.buffList)
                    {
                        buffInfo.CreateBuffUI(buff);
                    }
                }
            };
        }
        
        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
