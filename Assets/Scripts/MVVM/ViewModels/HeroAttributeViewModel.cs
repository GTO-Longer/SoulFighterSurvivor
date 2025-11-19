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
        private event Action UnBindEvent;

        private void Start()
        {
            var textGroup = new Dictionary<string, TMP_Text>();
            var attributeList = transform.GetComponentsInChildren<TMP_Text>();
            var HPContent = transform.Find("MainStateBackground/HPBarBackground/HPContent").GetComponent<TMP_Text>();
            var MPContent = transform.Find("MainStateBackground/MPBarBackground/MPContent").GetComponent<TMP_Text>();
            
            foreach (var _attribute in attributeList)
            {
                textGroup[_attribute.gameObject.name] = _attribute;
            }
            
            UnBindEvent += Binder.BindTextGroup(textGroup, HeroManager.hero);
            UnBindEvent += Binder.BindActive(transform.Find("AttributesAndHexes").gameObject, HeroManager.hero.showAttributes);
            UnBindEvent += Binder.BindDoubleText(HPContent, HeroManager.hero.healthPoint, HeroManager.hero.maxHealthPoint);
            UnBindEvent += Binder.BindDoubleText(MPContent, HeroManager.hero.magicPoint, HeroManager.hero.maxMagicPoint);
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
