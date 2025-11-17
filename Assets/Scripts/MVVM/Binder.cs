// UguiBinder.cs
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Classes;
using TMPro;
using Unity.VisualScripting;
using System.Reflection;
using System.Linq;
using Unity.Properties;

namespace MVVM
{
    /// <summary>
    /// 轻量级 UGUI MVVM 绑定工具（纯静态，无 MonoBehaviour）
    /// 支持：Text 数据绑定、Button 事件绑定、事件到事件绑定、Boolean 显示/隐藏绑定
    /// </summary>
    public static class Binder
    {
        // ===========================================
        // 1. Text 绑定：支持 string / int / float / Entity
        // ===========================================
        public static void BindText(TMP_Text text, BindableProperty<string> source)
        {
            if (text == null || source == null) return;
            text.text = source.Value;
            source.PropertyChanged += (_, _) => text.text = source.Value;
        }

        public static void BindText(TMP_Text text, BindableProperty<int> source, string format = "{0}")
        {
            if (text == null || source == null) return;
            text.text = string.Format(format, source.Value);
            source.PropertyChanged += (_, _) => text.text = string.Format(format, source.Value);
        }

        public static void BindText(TMP_Text text, BindableProperty<float> source, string format = "{0}")
        {
            if (text == null || source == null) return;
            text.text = string.Format(format, source.Value);
            source.PropertyChanged += (_, _) => text.text = string.Format(format, source.Value);
        }
        
        /// <summary>
        /// 自动将一组 TMP_Text 与 BindableProperty&lt;Entity&gt; 中当前 Entity 的同名 public BindableProperty&lt;float&gt; 字段进行绑定。
        /// 当 Entity 切换或为 null 时，自动重新绑定或清空文本。
        /// </summary>
        /// <param name="textGroup">要参与自动绑定的 TMP_Text 字典（key = 字段名）</param>
        /// <param name="entitySource">可观察的 Entity 数据源（BindableProperty&lt;Entity&gt;）</param>
        /// <param name="format">数值格式字符串，默认为 "F0"</param>
        public static void BindTextGroup(Dictionary<string, TMP_Text> textGroup, BindableProperty<Entity> entitySource, string format = "{0}")
        {
            if (textGroup == null || entitySource == null)
                return;

            // 初始绑定
            RebindEntity(textGroup, entitySource.Value, format);

            // 监听 Entity 切换
            entitySource.PropertyChanged += (_, _) =>
            {
                RebindEntity(textGroup, entitySource.Value, format);
            };
        }

        // 辅助方法：绑定单个 Entity，或清空
        private static void RebindEntity(Dictionary<string, TMP_Text> textGroup, Entity entity, string format)
        {
            if (entity == null)
            {
                foreach (var (_, text) in textGroup)
                {
                    if (text != null)
                        text.text = "0";
                }
                return;
            }

            var fields = entity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.FieldType != typeof(BindableProperty<float>))
                    continue;

                string fieldName = field.Name;
                if (!textGroup.TryGetValue(fieldName, out TMP_Text targetText) || targetText == null)
                    continue;

                var bindable = (BindableProperty<float>)field.GetValue(entity);
                if (bindable == null)
                {
                    targetText.text = "0";
                    continue;
                }

                BindText(targetText, bindable, format);
            }
        }

        // ===========================================
        // 2. Boolean 绑定：控制 GameObject 显示/隐藏
        // ===========================================
        /// <summary>
        /// 将布尔属性绑定到 GameObject 的激活状态（SetActive）
        /// </summary>
        /// <param name="target">要控制显示/隐藏的 GameObject（或任意 Component）</param>
        /// <param name="source">布尔数据源</param>
        public static void BindActive(GameObject target, BindableProperty<bool> source)
        {
            if (target == null || source == null) return;
            target.SetActive(source.Value);
            source.PropertyChanged += (_, _) => target.SetActive(source.Value);
        }
        
        /// <summary>
        /// 将游戏物体是否为空绑定到 GameObject 的激活状态（SetActive）
        /// </summary>
        /// <param name="target">要控制显示/隐藏的 GameObject（或任意 Component）</param>
        /// <param name="source">游戏物体数据源</param>
        public static void BindActive(GameObject target, BindableProperty<Entity> source)
        {
            if (target == null || source == null) return;
            target.SetActive(!source.Value.IsUnityNull());
            source.PropertyChanged += (_, _) => target.SetActive(!source.Value.IsUnityNull());
        }

        /// <summary>
        /// 重载：支持传入 Component（如 Image、Button 等），自动使用其 gameObject
        /// </summary>
        public static void BindActive(Component target, BindableProperty<bool> source)
        {
            if (target == null || source == null) return;
            BindActive(target.gameObject, source);
        }

        // ===========================================
        // 3. Button 绑定：点击 → Action
        // ===========================================
        public static void BindButton(Button button, Action action)
        {
            if (button == null || action == null) return;
            button.onClick.AddListener(action.Invoke);
        }

        // ===========================================
        // 4. 事件 → 事件绑定
        // ===========================================
        public static Action BindEvent(Action sourcePublisher, Action targetSubscriber)
        {
            sourcePublisher += targetSubscriber;
            return () => sourcePublisher -= targetSubscriber;
        }
    }
}