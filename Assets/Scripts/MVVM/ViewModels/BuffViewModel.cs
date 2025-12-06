using System;
using Classes;
using Components.UI;
using DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVM.ViewModels
{
    public class BuffViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;
        public static BuffViewModel Instance;
        public static Property<Buff> chosenBuff = new Property<Buff>();
        public GameObject buffPrefab;
        private Transform buffBar;

        private void Start()
        {
            Instance = this;
            buffPrefab.SetActive(false);
            buffBar = buffPrefab.transform.parent;
            
            var buffIcon = transform.Find("Background/TitleBanner/BuffIcon").GetComponent<Image>();
            var buffName = transform.Find("Background/TitleBanner/BuffName").GetComponent<TMP_Text>();
            var buffDescription = transform.Find("Background/BuffDescription").GetComponent<TMP_Text>();
            var background = transform.Find("Background").gameObject;
            
            // 绑定技能介绍面板
            UnBindEvent += Binder.BindBuff(buffIcon, background, buffName, buffDescription, chosenBuff);
        }

        // 物体销毁时触发注销对应事件
        private void OnDestroy()
        {
            UnBindEvent?.Invoke();
        }
        
        public Transform CreateBuffUI(Buff buff)
        {
            var newBuffUI = Instantiate(buffPrefab, buffBar);
            newBuffUI.GetComponent<BuffData>().buff = buff;
            newBuffUI.SetActive(true);
            return newBuffUI.transform;
        }

        public void DeleteBuffUI(Buff buff)
        {
            for (var index = 0; index < buffBar.childCount; index++)
            {
                var buffData = buffBar.GetChild(index).GetComponent<BuffData>();
                if (buffData != null && buffData.buff != null)
                {
                    if (buffData.buff == buff)
                    {
                        buffBar.GetChild(index).GetComponent<BuffButton>().OnPointerExit(null);
                        Destroy(buffBar.GetChild(index).gameObject);
                    }
                }
            }
        }
    }
}