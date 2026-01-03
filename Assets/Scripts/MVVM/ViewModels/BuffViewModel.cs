using System;
using System.Collections.Generic;
using Classes;
using Components.UI;
using DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MVVM.ViewModels
{
    public class BuffViewModel : MonoBehaviour
    {
        private event Action UnBindEvent;
        public Property<Buff> chosenBuff = new Property<Buff>();
        private Transform buffBar;
        private Dictionary<Buff, Action> BuffUnbindEvent = new();
        private Team currTeam;

        public void Initialize(Team team)
        {
            currTeam = team;
            if (currTeam == Team.Hero)
            {
                buffBar = HUDUIRoot.Instance.heroAttributes.transform.Find("MainStateBackground/BuffBar");
            }
            else
            {
                buffBar = HUDUIRoot.Instance.targetAttributes.transform.Find("BuffBar");
            }
            
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
            var newBuffUI = ResourceReader.LoadPrefab("UI/Buff");
            newBuffUI.transform.SetParent(buffBar);
            newBuffUI.GetComponent<BuffData>().buff = buff;
            newBuffUI.GetComponent<BuffButton>().team = currTeam;
            BuffUnbindEvent.TryAdd(buff, null);
            
            var buffMask = newBuffUI.transform.Find("BuffMask").GetComponent<Image>();
            BuffUnbindEvent[buff] += Binder.BindFillAmountImmediate(buffMask, buff.buffLeftDuration);
            
            var buffCount = newBuffUI.transform.Find("BuffCount").GetComponent<TMP_Text>();
            if (buff.buffMaxCount > 0)
            {
                buffCount.enabled = true;
                BuffUnbindEvent[buff] += Binder.BindText(buffCount, buff.buffCount);
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
                        if (BuffUnbindEvent.ContainsKey(buff))
                        {
                            BuffUnbindEvent[buff].Invoke();
                            BuffUnbindEvent.Remove(buff);
                        }

                        buffBar.GetChild(index).GetComponent<BuffButton>().OnPointerExit(null);
                        Destroy(buffBar.GetChild(index).gameObject);
                    }
                }
            }
        }

        public void DeleteAllBuffUI()
        {
            for (var index = 0; index < buffBar.childCount; index++)
            {
                var buff = buffBar.GetChild(index).GetComponent<BuffData>().buff;
                DeleteBuffUI(buff);
            }
        }
    }
}