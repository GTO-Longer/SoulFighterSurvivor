using System;
using System.Collections;
using System.Collections.Generic;
using Hero;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace MVVM.ViewModels
{
    public class TargetAttributeViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;

        private void Start()
        {
            var textGroup = new Dictionary<string, TMP_Text>();
            var attributeList = transform.GetComponentsInChildren<TMP_Text>();
            
            foreach (var _attribute in attributeList)
            {
                textGroup[_attribute.gameObject.name] = _attribute;
                Debug.Log(_attribute.gameObject.name);
            }
            
            UnBindEvent += Binder.BindActive(gameObject, HeroManager.hero.target);
            UnBindEvent += Binder.BindTextGroup(textGroup, HeroManager.hero.target);
        }
        
        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
