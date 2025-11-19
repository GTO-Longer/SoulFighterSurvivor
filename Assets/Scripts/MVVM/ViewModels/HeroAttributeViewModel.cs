using System;
using System.Collections;
using System.Collections.Generic;
using Classes;
using Hero;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace MVVM.ViewModels
{
    public class HeroAttributeViewModel : MonoBehaviour
    {
        
        private Transform _attributeList;
        private event Action UnBindEvent; 
        
        void Start()
        {
            Dictionary<string, TMP_Text> textGroup = new Dictionary<string, TMP_Text>();
            
            _attributeList = transform.Find("BasicAttributesBackground/AttributeList");
            for (int index = 0; index < _attributeList.childCount;index++)
            {
                textGroup[_attributeList.GetChild(index).name] = _attributeList.GetChild(index).Find("Content").GetComponent<TextMeshProUGUI>();
            }
            _attributeList = transform.Find("OtherAttributesBackground/AttributeList");
            for (int index = 0; index < _attributeList.childCount;index++)
            {
                textGroup[_attributeList.GetChild(index).name] = _attributeList.GetChild(index).Find("Content").GetComponent<TextMeshProUGUI>();
            }
            
            UnBindEvent += Binder.BindTextGroup(textGroup, HeroManager.hero);
            UnBindEvent += Binder.BindActive(gameObject, HeroManager.hero.showAttributes);
        }

        void Update()
        {
            
        }

        // 物体销毁时触发注销对应事件
        void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
