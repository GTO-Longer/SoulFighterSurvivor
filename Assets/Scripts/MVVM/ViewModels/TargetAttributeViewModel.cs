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
        
        private Transform _attributeList;
        
        void Start()
        {
            _attributeList = transform.Find("Background/AttributeList");
            Dictionary<string, TMP_Text> textGroup = new Dictionary<string, TMP_Text>();
            for (int index = 0; index < _attributeList.childCount;index++)
            {
                textGroup[_attributeList.GetChild(index).name] = _attributeList.GetChild(index).Find("Content").GetComponent<TextMeshProUGUI>();
            }
            
            Binder.BindActive(gameObject, HeroManager.hero.target);
            Binder.BindTextGroup(textGroup, HeroManager.hero.target);
        }

        void Update()
        {
            
        }
    }
}
