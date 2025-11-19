// UguiBinder.cs
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Classes;
using TMPro;
using System.Reflection;
using Unity.VisualScripting;

namespace MVVM
{
    /// <summary>
    /// 轻量级 UGUI MVVM 绑定工具（纯静态，无 MonoBehaviour）
    /// 所有绑定方法返回 Action 用于解绑，避免 MissingReferenceException 和内存泄漏。
    /// </summary>
    public static class Binder
    {
        // ===========================================
        // 1. Text 绑定：支持 string / int / float
        // ===========================================

        public static Action BindText(TMP_Text text, BindableProperty<string> source)
        {
            if (text == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.text = source.Value;
            }

            text.text = source.Value;
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindText(TMP_Text text, BindableProperty<int> source, string format = "{0}")
        {
            if (text == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.text = string.Format(format, source.Value);
            }

            text.text = string.Format(format, source.Value);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        public static Action BindText(TMP_Text text, BindableProperty<float> source, string format = "{0}")
        {
            if (text == null || source == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.text = string.Format(format, source.Value);
            }

            text.text = string.Format(format, source.Value);
            source.PropertyChanged += OnChanged;
            return () => source.PropertyChanged -= OnChanged;
        }

        // ===========================================
        // 2. TextGroup 绑定（动态 Entity 切换）
        // ===========================================

        public static Action BindTextGroup(Dictionary<string, TMP_Text> textGroup, BindableProperty<Entity> entitySource, string format = "{0}")
        {
            if (textGroup == null || entitySource == null)
                return () => { };

            var unbindCallbacks = new List<Action>();

            void Rebind()
            {
                // 清理解绑旧的
                foreach (var unbind in unbindCallbacks)
                    unbind();
                unbindCallbacks.Clear();

                var entity = entitySource.Value;
                if (entity == null)
                {
                    foreach (var (_, text) in textGroup)
                    {
                        if (text != null) text.text = "0";
                    }
                    return;
                }

                var fields = entity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.FieldType != typeof(BindableProperty<float>)) continue;

                    string fieldName = field.Name;
                    if (!textGroup.TryGetValue(fieldName, out TMP_Text targetText) || targetText == null)
                        continue;

                    var bindable = (BindableProperty<float>)field.GetValue(entity);
                    if (bindable == null)
                    {
                        targetText.text = "0";
                        continue;
                    }

                    var unbind = BindText(targetText, bindable, format);
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

        // ===========================================
        // 3. TextGroup 绑定（固定 Entity）
        // ===========================================

        public static Action BindTextGroup(Dictionary<string, TMP_Text> textGroup, Entity entity, string format = "{0}")
        {
            if (textGroup == null || entity == null)
                return () => { };

            var unbindCallbacks = new List<Action>();

            var fields = entity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(BindableProperty<float>)) continue;

                string fieldName = field.Name;
                if (!textGroup.TryGetValue(fieldName, out TMP_Text targetText) || targetText == null)
                    continue;

                var bindable = (BindableProperty<float>)field.GetValue(entity);
                if (bindable == null)
                {
                    targetText.text = "0";
                    continue;
                }

                var unbind = BindText(targetText, bindable, format);
                unbindCallbacks.Add(unbind);
            }

            return () =>
            {
                foreach (var unbind in unbindCallbacks)
                    unbind();
                unbindCallbacks.Clear();
            };
        }

        // ===========================================
        // 4. Boolean 绑定：控制 GameObject 显示/隐藏
        // ===========================================

        public static Action BindActive(GameObject target, BindableProperty<bool> source)
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

        public static Action BindActive(GameObject target, BindableProperty<Entity> source)
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

        public static Action BindActive(Component target, BindableProperty<bool> source)
        {
            if (target == null || source == null)
                return () => { };

            return BindActive(target.gameObject, source);
        }

        // ===========================================
        // 5. Button 绑定：点击 → Action
        // ===========================================

        public static Action BindButton(Button button, Action action)
        {
            if (button == null || action == null)
                return () => { };

            void OnClick() => action.Invoke();

            button.onClick.AddListener(OnClick);
            return () => button.onClick.RemoveListener(OnClick);
        }

        // ===========================================
        // 6. 事件 → 事件绑定（保留原逻辑）
        // ===========================================

        public static Action BindEvent(Action sourcePublisher, Action targetSubscriber)
        {
            if (sourcePublisher == null || targetSubscriber == null)
                return () => { };

            sourcePublisher += targetSubscriber;
            return () => sourcePublisher -= targetSubscriber;
        }
    }
}