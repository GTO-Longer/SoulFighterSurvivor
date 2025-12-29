using System;
using System.Collections.Generic;
using DataManagement;
using Managers.EntityManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MVVM.ViewModels
{
    public class HeroAttributeViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;
        public static HeroAttributeViewModel Instance;
        private Property<bool> notFullHealth;
        private Property<bool> notFullMagic;

        private void Start()
        {
            Instance = this;
            
            var textGroup = new Dictionary<string, TMP_Text>();
            var attributeList = transform.GetComponentsInChildren<TMP_Text>();
            var HPContent = transform.Find("MainStateBackground/HPBarBackground/HPContent").GetComponent<TMP_Text>();
            var MPContent = transform.Find("MainStateBackground/MPBarBackground/MPContent").GetComponent<TMP_Text>();
            var HPRegenerateContent = transform.Find("MainStateBackground/HPBarBackground/HPRegenerateContent").GetComponent<TMP_Text>();
            var MPRegenerateContent = transform.Find("MainStateBackground/MPBarBackground/MPRegenerateContent").GetComponent<TMP_Text>();
            var HPBar = transform.Find("MainStateBackground/HPBarBackground/HPBar").GetComponent<Image>();
            var HPBarSmooth = transform.Find("MainStateBackground/HPBarBackground/HPBarSmooth").GetComponent<Image>();
            var MPBar = transform.Find("MainStateBackground/MPBarBackground/MPBar").GetComponent<Image>();
            var otherAttributes = transform.Find("AttributesAndHexes/OtherAttributesBackground").gameObject;
            var hexes = transform.Find("AttributesAndHexes/HexesBackground").gameObject;
            var coinContent = transform.Find("Equipments/ShopButton/CoinContent").gameObject.GetComponent<TMP_Text>();

            notFullHealth = new Property<bool>(
                () => Mathf.Abs(HeroManager.hero.healthPoint.Value - HeroManager.hero.maxHealthPoint.Value) > 0.1f, DataType.None,
                HeroManager.hero.healthPoint, HeroManager.hero.maxHealthPoint);
            notFullMagic = new Property<bool>(
                () => Mathf.Abs(HeroManager.hero.magicPoint.Value - HeroManager.hero.maxMagicPoint.Value) > 0.1f, DataType.None,
                HeroManager.hero.magicPoint, HeroManager.hero.maxMagicPoint);
            
            foreach (var _attribute in attributeList)
            {
                textGroup[_attribute.gameObject.name] = _attribute;
            }
            
            UnBindEvent += Binder.BindTextGroup(textGroup, HeroManager.hero);
            UnBindEvent += Binder.BindActive(otherAttributes,  HeroManager.hero.showAttributes);
            UnBindEvent += Binder.BindActive(hexes, HeroManager.hero.showAttributes);
            UnBindEvent += Binder.BindText(HPContent, HeroManager.hero.healthPoint, HeroManager.hero.maxHealthPoint, "{0:F0} / {1:F0}");
            UnBindEvent += Binder.BindText(MPContent, HeroManager.hero.magicPoint, HeroManager.hero.maxMagicPoint, "{0:F0} / {1:F0}");
            UnBindEvent += Binder.BindText(HPRegenerateContent, HeroManager.hero.healthRegeneration, "+{0:F1}");
            UnBindEvent += Binder.BindText(MPRegenerateContent, HeroManager.hero.magicRegeneration, "+{0:F1}");
            UnBindEvent += Binder.BindActive(HPRegenerateContent, notFullHealth);
            UnBindEvent += Binder.BindActive(MPRegenerateContent, notFullMagic);
            UnBindEvent += Binder.BindFillAmountImmediate(HPBar, HeroManager.hero.healthPointProportion);
            UnBindEvent += Binder.BindFillAmountSmooth(HPBarSmooth, 0.2f, HeroManager.hero.healthPointProportion);
            UnBindEvent += Binder.BindFillAmountImmediate(MPBar, HeroManager.hero.magicPointProportion);
            UnBindEvent += Binder.BindText(coinContent, HeroManager.hero.coins);
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
    }
}
