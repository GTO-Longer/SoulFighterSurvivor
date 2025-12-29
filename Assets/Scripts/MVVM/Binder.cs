using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Classes;
using TMPro;
using System.Reflection;
using DataManagement;
using DG.Tweening;
using MVVM.ViewModels;
using Unity.VisualScripting;
using Utilities;

namespace MVVM
{
    public static class Binder
    {
        #region 单数据-Text绑定
        
        public static Action BindText(TMP_Text text, Property<string> source)
        {
            if (text == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.SetText(source.Value);
            }

            text.SetText(source.Value);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindText(TMP_Text text, Property<int> source, string format = null)
        {
            if (text == null || source == null)
                return () => { };

            format ??= source.dataType switch
            {
                DataType.Int => "{0:D}",
                DataType.Percentage => "{0:P}",
                _ => "{0}"
            };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.SetText(string.Format(format, source.Value));
            }

            text.SetText(string.Format(format, source.Value));
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindText(TMP_Text text, Property<float> source, string format = null)
        {
            if (text == null || source == null)
                return () => { };

            format ??= source.dataType switch
            {
                DataType.Float => "{0:F2}",
                DataType.Int => "{0:F0}",
                DataType.Percentage => "{0:P0}",
                _ => "{0}"
            };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.SetText(string.Format(format, source.Value));
            }

            text.SetText(string.Format(format, source.Value));
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }
        
        #endregion
        
        # region 双数据-Text/图片绑定
        public static Action BindText(TMP_Text text, Property<float> source1, Property<float> source2, string format = "{0} / {1}"){
            if (text == null || source1 == null || source2 == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.SetText(string.Format(format, source1.Value, source2.Value));
            }

            text.SetText(string.Format(format, source1.Value, source2.Value));
            source1.PropertyChanged += OnChanged;
            source2.PropertyChanged += OnChanged;
            return () =>
            {
                source1.PropertyChanged -= OnChanged;
                source2.PropertyChanged -= OnChanged;
            };
        }
        
        public static Action BindFillAmountSmooth(Image image, float duration, Property<float> source){
            if (image == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (image == null) return;
                Async.SetFillAmountAsync(image, source.Value, duration);
            }

            Async.SetFillAmountAsync(image, source.Value, duration);
            source.PropertyChanged += OnChanged;
            return () =>
            {
                source.PropertyChanged -= OnChanged;
            };
        }
        
        public static Action BindFillAmountImmediate(Image image, Property<float> source){
            if (image == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (image == null) return;
                image.fillAmount = source.Value;
            }

            image.fillAmount = source.Value;
            source.PropertyChanged += OnChanged;
            return () =>
            {
                source.PropertyChanged -= OnChanged;
            };
        }

        #endregion

        #region Group绑定

        /// <summary>
        /// 绑定文字组合（实体属性）
        /// </summary>
        public static Action BindTextGroup(Dictionary<string, TMP_Text> textGroup, Property<Entity> entitySource)
        {
            if (textGroup == null || entitySource == null)
                return () => { };

            var unbindCallbacks = new List<Action>();

            void Rebind()
            {
                // 解绑旧的
                foreach (var unbind in unbindCallbacks)
                    unbind();
                unbindCallbacks.Clear();

                var entity = entitySource.Value;
                if (entity == null)
                {
                    foreach (var (_, text) in textGroup)
                    {
                        if (text != null) text.SetText("0");
                    }
                    return;
                }

                var fields = entity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.FieldType != typeof(Property<float>)) continue;

                    string fieldName = field.Name;
                    if (!textGroup.TryGetValue(fieldName, out TMP_Text targetText) || targetText == null)
                        continue;

                    var bindable = (Property<float>)field.GetValue(entity);
                    if (bindable == null)
                    {
                        targetText.SetText("0");
                        continue;
                    }

                    var unbind = BindText(targetText, bindable);
                    unbindCallbacks.Add(unbind);
                }
            }

            Rebind();
            entitySource.PropertyChanged += (_, _) => Rebind();

            return () =>
            {
                entitySource.PropertyChanged -= (_, _) => Rebind();
                foreach (var unbind in unbindCallbacks)
                    unbind();
                unbindCallbacks.Clear();
            };
        }

        public static Action BindTextGroup(Dictionary<string, TMP_Text> textGroup, Entity entity)
        {
            if (textGroup == null || entity == null)
                return () => { };

            var unbindCallbacks = new List<Action>();

            var fields = entity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(Property<float>)) continue;

                var fieldName = field.Name;
                if (!textGroup.TryGetValue(fieldName, out TMP_Text targetText) || targetText == null)
                    continue;

                var bindable = (Property<float>)field.GetValue(entity);
                if (bindable == null)
                {
                    targetText.SetText("0");
                    continue;
                }

                var unbind = BindText(targetText, bindable);
                unbindCallbacks.Add(unbind);
            }

            return () =>
            {
                foreach (var unbind in unbindCallbacks)
                    unbind();
                unbindCallbacks.Clear();
            };
        }
        
        /// <summary>
        /// 绑定敌人血条相关UI
        /// </summary>
        public static Action BindHPGroup(TMP_Text text,Image image, Property<Entity> source, string format = "{0} / {1}"){
            if (text == null || image == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null || image == null || source.Value == null) return;
                BindText(text, source.Value.healthPoint, source.Value.maxHealthPoint, "{0:F0} / {1:F0}");
                BindFillAmountImmediate(image, source.Value.healthPointProportion);
            }

            source.PropertyChanged += OnChanged;
            source.PropertyChanged += OnChanged;
            return () =>
            {
                source.PropertyChanged -= OnChanged;
                source.PropertyChanged -= OnChanged;
            };
        }
        
        /// <summary>
        /// 绑定技能
        /// </summary>
        public static Action BindSkill(Image skillIcon, GameObject background, TMP_Text skillName, TMP_Text skillLevel, TMP_Text skillCoolDown, TMP_Text skillCost, TMP_Text skillDescription, Property<Skill> skillSource)
        {
            void OnChanged(object sender, EventArgs e)
            {
                if (skillSource.Value == null)
                {
                    background.SetActive(false);
                    return;
                }

                background.SetActive(true);
                skillIcon.sprite = skillSource.Value.skillIcon.sprite;
                skillName.text = skillSource.Value.skillName;
                skillLevel.text = skillSource.Value.skillLevel == 0 ? "无等级" :$"技能等级{skillSource.Value.skillLevel:F0}";
                skillCoolDown.text = skillSource.Value.actualSkillCoolDown == 0 ? "无冷却" : $"{skillSource.Value.actualSkillCoolDown:0.#}秒";
                skillCost.text = skillSource.Value.actualSkillCost == 0 ? "无消耗" : $"{skillSource.Value.actualSkillCost:F0}法力值";
                skillDescription.text = skillSource.Value.GetDescription();
                LayoutRebuilder.ForceRebuildLayoutImmediate(background.GetComponent<RectTransform>());
            }

            OnChanged(null, null);
            skillSource.PropertyChanged += OnChanged;
            return () =>
            {
                skillSource.PropertyChanged -= OnChanged;
            };
        }
        
        /// <summary>
        /// 绑定Buff
        /// </summary>
        public static Action BindBuff(Image buffIcon, GameObject background, TMP_Text buffName, TMP_Text buffDescription, Property<Buff> buffSource)
        {
            void OnChanged(object sender, EventArgs e)
            {
                if (buffSource.Value == null)
                {
                    background.SetActive(false);
                    return;
                }

                background.SetActive(true);
                buffIcon.sprite = buffSource.Value.buffIcon;
                buffName.text = buffSource.Value.buffName;
                buffDescription.text = buffSource.Value.buffDescription;
                LayoutRebuilder.ForceRebuildLayoutImmediate(background.GetComponent<RectTransform>());
            }

            OnChanged(null, null);
            buffSource.PropertyChanged += OnChanged;
            return () =>
            {
                buffSource.PropertyChanged -= OnChanged;
            };
        }

        /// <summary>
        /// 绑定属性
        /// </summary>
        public static Action BindAttribute(GameObject background, TMP_Text attrName, TMP_Text attrDescription, TMP_Text attrAmount, AttributeType attrType)
        {
            var dependencies = new List<Property<float>>();
            void OnChanged(object sender, EventArgs e)
            {
                if (attrType == AttributeType.None)
                {
                    background.SetActive(false);
                    AttributeViewModel.Instance.UnBindEvent?.Invoke();
                    AttributeViewModel.Instance.UnBindEvent = null;
                    return;
                }
                
                if (!(AttributeViewModel.Instance.attributeDependenciesSettings.TryGetValue(attrType, out dependencies) &&
                      dependencies != null))
                {
                    return;
                }
                
                background.SetActive(true);
                attrName.text = AttributeViewModel.Instance.attributeDescriptionSettings[attrType][0].Invoke();
                attrDescription.text = AttributeViewModel.Instance.attributeDescriptionSettings[attrType][1].Invoke();
                attrAmount.text = AttributeViewModel.Instance.attributeDescriptionSettings[attrType][2].Invoke();
                LayoutRebuilder.ForceRebuildLayoutImmediate(background.GetComponent<RectTransform>());
            }

            OnChanged(null, null);
            foreach (var dependence in dependencies)
            {
                dependence.PropertyChanged += OnChanged;
            }
            
            return () =>
            {
                foreach (var dependence in dependencies)
                {
                    dependence.PropertyChanged -= OnChanged;
                }
            };
        }
        
        /// <summary>
        /// 绑定装备UI
        /// </summary>
        public static Action BindEquipment(Image icon, Image CDMask, TMP_Text CDText, TMP_Text chargeCount, Property<Equipment> source)
        {
            if (icon == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (source.Value == null)
                {
                    icon.sprite = null;
                    CDMask.enabled = false;
                    CDText.enabled = false;
                    chargeCount.enabled = false;
                }
                else
                {
                    icon.sprite = source.Value.equipmentIcon;
                    if (source.Value.haveActiveSkillCD || source.Value.havePassiveSkillCD)
                    {
                        CDMask.enabled = true;
                        CDText.enabled = true;
                        source.Value._passiveSkillCDProportion = new Property<float>();
                        source.Value._activeSkillCDProportion = new Property<float>();
                        source.Value._passiveSkillCDDif = new Property<string>();
                        source.Value._activeSkillCDDif = new Property<string>();
                        if (source.Value.haveActiveSkillCD)
                        {
                            BindFillAmountImmediate(CDMask, source.Value._activeSkillCDProportion);
                            BindText(CDText, source.Value._activeSkillCDDif);
                        }
                        else if(source.Value.havePassiveSkillCD)
                        {
                            BindFillAmountImmediate(CDMask, source.Value._passiveSkillCDProportion);
                            BindText(CDText, source.Value._passiveSkillCDDif);
                        }
                    }

                    if (source.Value.chargeCount >= 0 && source.Value.maxChargeCount > 0)
                    {
                        chargeCount.enabled = true;
                        BindText(chargeCount, source.Value.chargeCount);
                    }
                    else
                    {
                        chargeCount.enabled = false;
                    }
                }
            }

            OnChanged(null, null);
            
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }
        
        #endregion

        #region Boolean-UI显隐绑定

        public static Action BindActive(GameObject target, Property<bool> source, bool canInteract = true)
        {
            if (target == null || source == null || target.GetComponent<CanvasGroup>() == null)
            {
                Debug.LogWarning($"[Binder] {target.name}显隐绑定失败");
                return () => { };
            }

            void OnChanged(object sender, EventArgs e)
            {
                if (target == null) return;
                
                if (source.Value)
                {
                    target.GetComponent<CanvasGroup>().alpha = 1;
                    target.GetComponent<CanvasGroup>().blocksRaycasts = canInteract;
                }
                else
                {
                    target.GetComponent<CanvasGroup>().alpha = 0;
                    target.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }

            OnChanged(null, null);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindActive<T>(GameObject target, Property<T> source, bool canInteract = true)
        {
            if (target == null || source == null || target.GetComponent<CanvasGroup>() == null)
            {
                Debug.LogWarning($"[Binder] {target.name}显隐绑定失败");
                return () => { };
            }

            void OnChanged(object sender, EventArgs e)
            {
                if (target == null) return;
                
                if (source.Value.IsUnityNull())
                {
                    target.GetComponent<CanvasGroup>().alpha = 0;
                    target.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else
                {
                    target.GetComponent<CanvasGroup>().alpha = 1;
                    target.GetComponent<CanvasGroup>().blocksRaycasts = canInteract;
                }
            }

            OnChanged(null, null);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindActive<T>(TMP_Text target, Property<T> source)
        {
            if (target == null || source == null)
            {
                Debug.LogWarning($"[Binder] {target.name}显隐绑定失败");
                return () => { };
            }

            void OnChanged(object sender, EventArgs e)
            {
                if (target == null) return;
                
                if (source.Value.IsUnityNull())
                {
                    target.color = new Color(target.color.r, target.color.g, target.color.b, 0);
                }
                else
                {
                    target.color = new Color(target.color.r, target.color.g, target.color.b, 1);
                }
            }

            OnChanged(null, null);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }
        
        public static Action BindActive<T>(Image target, Property<T> source)
        {
            if (target == null || source == null)
            {
                Debug.LogWarning($"[Binder] {target.name}显隐绑定失败");
                return () => { };
            }

            void OnChanged(object sender, EventArgs e)
            {
                if (target == null) return;
                
                if (source.Value.IsUnityNull())
                {
                    target.color = new Color(target.color.r, target.color.g, target.color.b, 0);
                }
                else
                {
                    target.color = new Color(target.color.r, target.color.g, target.color.b, 1);
                }
            }

            OnChanged(null, null);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        #endregion

        #region 事件-Button绑定

        public static Action BindButton(Button button, Action action)
        {
            if (button == null || action == null)
                return () => { };

            void OnClick() => action.Invoke();

            button.onClick.AddListener(OnClick);
            return () => button.onClick.RemoveListener(OnClick);
        }
        
        #endregion

        #region 普通UI显示
        
        public static Dictionary<TMP_Text, Tweener> Tweeners = new();
        public static void ShowText(TMP_Text text, string content, float duration = -1)
        {
            if (duration < 0)
            {
                text.text = content;
                text.gameObject.SetActive(true);
            }
            else
            {
                if (Tweeners.TryGetValue(text, out var tweener) && tweener != null)
                {
                    tweener.Kill();
                }
                
                Tweeners[text] = Async.SetAsync(duration - 0.5f, null, () =>
                {
                    text.gameObject.SetActive(true);
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
                    text.text = content;
                }, () =>
                {
                    text.DOFade(0, 0.5f).OnComplete(() =>
                    {
                        text.gameObject.SetActive(false);
                        Tweeners.Remove(text);
                    });
                });
            }
        }

        #endregion
    }
}