// UguiBinder.cs
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MVVM
{
    /// <summary>
    /// 轻量级 UGUI MVVM 绑定工具（纯静态，无 MonoBehaviour）
    /// 支持：Text 数据绑定、Button 事件绑定、事件到事件绑定
    /// </summary>
    public static class Binder
    {
        // ===========================================
        // 1. Text 绑定：支持 string / int / float
        // ===========================================
        public static void BindText(Text text, BindableProperty<string> source)
        {
            if (text == null || source == null) return;
            text.text = source.Value;
            source.PropertyChanged += (_, _) => text.text = source.Value;
        }

        public static void BindText(Text text, BindableProperty<int> source, string format = "{0}")
        {
            if (text == null || source == null) return;
            text.text = string.Format(format, source.Value);
            source.PropertyChanged += (_, _) => text.text = string.Format(format, source.Value);
        }

        public static void BindText(Text text, BindableProperty<float> source, string format = "{0}")
        {
            if (text == null || source == null) return;
            text.text = string.Format(format, source.Value);
            source.PropertyChanged += (_, _) => text.text = string.Format(format, source.Value);
        }

        // ===========================================
        // 2. Button 绑定：点击 → Action
        // ===========================================
        public static void BindButton(Button button, Action action)
        {
            if (button == null || action == null) return;
            button.onClick.AddListener(action.Invoke);
        }

        // ===========================================
        // 3. 事件 → 事件绑定：当 sourceEvent 触发时，自动调用 targetAction
        // ===========================================
        public static Action BindEvent(Action sourcePublisher, Action targetSubscriber)
        {
            // 订阅
            sourcePublisher += targetSubscriber;
            
            // 返回取消订阅的委托（用于清理，小项目可忽略）
            return () => sourcePublisher -= targetSubscriber;
        }
    }
}