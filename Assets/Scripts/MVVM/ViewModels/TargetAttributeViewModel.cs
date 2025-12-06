using System;
using System.Collections.Generic;
using Classes;
using DataManagement;
using Managers.EntityManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class TargetAttributeViewModel : MonoBehaviour
    {
        public static TargetAttributeViewModel Instance;
        private event Action UnBindEvent;
        public Property<Entity> checkTarget = new Property<Entity>();

        private void Start()
        {
            Instance = this;
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
        }
        
        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
