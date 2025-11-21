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
                text.text = source.Value;
            }

            text.text = source.Value;
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
                text.text = string.Format(format, source.Value);
            }

            text.text = string.Format(format, source.Value);
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
                text.text = string.Format(format, source.Value);
            }

            text.text = string.Format(format, source.Value);
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
        
        public static Action BindLength(Transform image, Property<float> source1, Property<float> source2){
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

        #endregion

        #region TextGroup绑定

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
                        if (text != null) text.text = "0";
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
                        targetText.text = "0";
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

                string fieldName = field.Name;
                if (!textGroup.TryGetValue(fieldName, out TMP_Text targetText) || targetText == null)
                    continue;

                var bindable = (Property<float>)field.GetValue(entity);
                if (bindable == null)
                {
                    targetText.text = "0";
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

        public static Action BindActive(GameObject target, Property<Entity> source)
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