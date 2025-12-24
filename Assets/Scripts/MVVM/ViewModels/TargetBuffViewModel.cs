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
    public class TargetBuffViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;
        public static TargetBuffViewModel Instance;
        public static Property<Buff> chosenBuff = new Property<Buff>();
        public GameObject buffPrefab;
        private Transform buffBar;
        private Dictionary<string, Action> BuffUnbindEvent = new();

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
            
            var buffMask = newBuffUI.transform.Find("BuffMask").GetComponent<Image>();
            BuffUnbindEvent.Add(buff.buffName + "Mask", Binder.BindFillAmountImmediate(buffMask, buff.buffLeftDuration));
            
            var buffCount = newBuffUI.transform.Find("BuffCount").GetComponent<TMP_Text>();
            if (buff.buffMaxCount > 0)
            {
                buffCount.enabled = true;
                BuffUnbindEvent.TryAdd(buff.buffName + "Count", Binder.BindText(buffCount, buff.buffCount));
            }
            else
            {
                buffCount.enabled = false;
            }
            
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
                        if (BuffUnbindEvent.ContainsKey(buff.buffName + "Mask"))
                        {
                            BuffUnbindEvent[buff.buffName + "Mask"].Invoke();
                            BuffUnbindEvent.Remove(buff.buffName + "Mask");
                        }
                        
                        if (BuffUnbindEvent.ContainsKey(buff.buffName + "Count"))
                        {
                            BuffUnbindEvent[buff.buffName + "Count"].Invoke();
                            BuffUnbindEvent.Remove(buff.buffName + "Count");
                        }
                        
                        buffBar.GetChild(index).GetComponent<BuffButton>().OnPointerExit(null);
                        Destroy(buffBar.GetChild(index).gameObject);
                    }
                }
            }
        }

        public void ClearAllBuffUI()
        {
            for (var index = 0; index < buffBar.childCount; index++)
            {
                var buff = buffBar.GetChild(index).GetComponent<BuffData>().buff;
                DeleteBuffUI(buff);
            }
        }
    }
}