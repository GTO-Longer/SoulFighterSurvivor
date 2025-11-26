using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Classes;
using TMPro;
using System.Reflection;
using DataManagement;
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
        
        public static Action BindFillAmount(Image image, Property<float> source){
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
        /// 绑定实体与StateBar
        /// </summary>
        /// <param name="stateBar"></param>
        /// <param name="entity"></param>
        public static Action BindStateBar(RectTransform stateBar, Entity entity)
        {
            if (stateBar == null || entity == null)
                return () => { };

            Action UnBind = () => {};
            stateBar.gameObject.SetActive(true);
            
            UnBind += BindFillAmount(stateBar.Find("HPBarBackground/HPBar").GetComponent<Image>(), entity.healthPointProportion);
            UnBind += BindFillAmount(stateBar.Find("MPBarBackground/MPBar").GetComponent<Image>(), entity.magicPointProportion);
            UnBind += BindText(stateBar.Find("LevelBackground/Level").GetComponent<TMP_Text>(), entity.level, "{0:F0}");
            
            return UnBind;
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
                BindFillAmount(image, source.Value.healthPointProportion);
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
        public static Action BindSkill(Image skillIcon, GameObject background, TMP_Text skillName, TMP_Text skillCoolDown, TMP_Text skillCost, TMP_Text skillDescription, Property<Skill> skillSource)
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
                skillName.text = (skillSource.Value.skillName);
                skillCoolDown.text = skillSource.Value.actualSkillCoolDown == 0 ? "无冷却" : $"{skillSource.Value.actualSkillCoolDown:F2}秒";
                skillCost.text = skillSource.Value.actualSkillCost == 0 ? "无消耗" : $"{skillSource.Value.actualSkillCost:F0}法力值";
                skillDescription.text = (skillSource.Value.GetDescription());
                LayoutRebuilder.ForceRebuildLayoutImmediate(background.GetComponent<RectTransform>());
            }

            OnChanged(null, null);
            skillSource.PropertyChanged += OnChanged;
            return () =>
            {
                skillSource.PropertyChanged -= OnChanged;
            };
        }
        
        #endregion

        #region Boolean-GameObject显隐绑定

        public static Action BindActive(GameObject target, Property<bool> source)
        {
            if (target == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (target == null) return;
                target.SetActive(source.Value);
            }

            target.SetActive(source.Value);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindActive<T>(GameObject target, Property<T> source)
        {
            if (target == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (target == null) return;
                target.SetActive(!source.Value.IsUnityNull());
            }

            target.SetActive(!source.Value.IsUnityNull());
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindActive(Component target, Property<bool> source)
        {
            if (target == null || source == null)
                return () => { };

            return BindActive(target.gameObject, source);
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
    }
}