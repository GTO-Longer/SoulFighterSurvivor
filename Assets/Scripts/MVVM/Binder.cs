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
        // 2. 双数据绑定（两个变量之间的关系，任何一变量变化都要引起UI变化）
        // ===========================================
        public static Action BindText(TMP_Text text, BindableProperty<float> source1, BindableProperty<float> source2, string format = "{0} / {1}"){
            if (text == null || source1 == null || source2 == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (text == null) return;
                text.text = string.Format(format, source1.Value, source2.Value);
            }

            text.text = string.Format(format, source1.Value, source2.Value);
            source1.PropertyChanged += OnChanged;
            source2.PropertyChanged += OnChanged;
            return () =>
            {
                source1.PropertyChanged -= OnChanged;
                source2.PropertyChanged -= OnChanged;
            };
        }
        
        public static Action BindLength(Transform image, BindableProperty<float> source1, BindableProperty<float> source2){
            if (image == null || source1 == null || source2 == null)
                return () => { };

            void OnChanged(object sender, EventArgs e)
            {
                if (image == null) return;
                var scale = image.localScale;
                image.localScale = new(source2.Value == 0 ? 0 : (source1.Value / source2.Value), scale.y, scale.z);
            }
            
            var scale = image.localScale;
            image.localScale = new(source2.Value == 0 ? 0 : (source1.Value / source2.Value), scale.y, scale.z);
            source1.PropertyChanged += OnChanged;
            source2.PropertyChanged += OnChanged;
            return () =>
            {
                source1.PropertyChanged -= OnChanged;
                source2.PropertyChanged -= OnChanged;
            };
        }

        // ===========================================
        // 3. TextGroup 绑定（支持float）
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
        // 4. TextGroup 绑定（固定 Entity）
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
        // 5. Boolean 绑定：控制 GameObject 显示/隐藏
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
        // 6. Button 绑定：点击 → Action
        // ===========================================

        public static Action BindButton(Button button, Action action)
        {
            if (button == null || action == null)
                return () => { };

            void OnClick() => action.Invoke();

            button.onClick.AddListener(OnClick);
            return () => button.onClick.RemoveListener(OnClick);
        }
    }
}