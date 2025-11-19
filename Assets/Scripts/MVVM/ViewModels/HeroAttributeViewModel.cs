using System;
using System.Collections.Generic;
using Entities.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            var HPBar = transform.Find("MainStateBackground/HPBarBackground/HPBar");
            var MPBar = transform.Find("MainStateBackground/MPBarBackground/MPBar");
            var otherAttributes = transform.Find("AttributesAndHexes/OtherAttributesBackground").gameObject;
            var hexes = transform.Find("AttributesAndHexes/HexesBackground").gameObject;
            
            foreach (var _attribute in attributeList)
            {
                textGroup[_attribute.gameObject.name] = _attribute;
            }
            
            UnBindEvent += Binder.BindTextGroup(textGroup, HeroManager.hero);
            UnBindEvent += Binder.BindActive(otherAttributes, HeroManager.hero.showAttributes);
            UnBindEvent += Binder.BindActive(hexes, HeroManager.hero.showAttributes);
            UnBindEvent += Binder.BindText(HPContent, HeroManager.hero.healthPoint, HeroManager.hero.maxHealthPoint);
            UnBindEvent += Binder.BindText(MPContent, HeroManager.hero.magicPoint, HeroManager.hero.maxMagicPoint);
            UnBindEvent += Binder.BindLength(HPBar, HeroManager.hero.healthPoint, HeroManager.hero.maxHealthPoint);
            UnBindEvent += Binder.BindLength(MPBar, HeroManager.hero.magicPoint, HeroManager.hero.maxMagicPoint);
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
