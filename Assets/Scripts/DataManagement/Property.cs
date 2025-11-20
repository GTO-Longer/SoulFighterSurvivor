using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DataManagement
{
    public sealed class Property<T> : INotifyPropertyChanged, IDisposable
    {
        private T _value;
        private Func<T> _computeFunc;
        private readonly List<INotifyPropertyChanged> _dependencies = new();
        private readonly bool _isComputed;

        // 普通属性
        public Property(T initialValue = default)
        {
            _value = initialValue;
            _isComputed = false;
        }

        // 依赖属性
        public Property(Func<T> computeFunc, params Property<float>[] dependencies)
        {
            _computeFunc = computeFunc;
            _isComputed = true;
            SetupDependencies(dependencies);
        }

        private void SetupDependencies(Property<float>[] dependencies)
        {
            foreach (var dep in dependencies)
            {
                if (dep != null)
                {
                    dep.PropertyChanged += OnDependencyChanged;
                    _dependencies.Add(dep);
                }
            }
        }

        private void OnDependencyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 依赖变化 → 触发自身 PropertyChanged（值在 getter 中计算）
            OnPropertyChanged(nameof(Value));
        }

        public T Value
        {
            get
            {
                if (_isComputed)
                {
                    return _computeFunc();
                }
                return _value;
            }
            set
            {
                if (_isComputed)
                {
                    Debug.LogError("ComputableProperty cannot be defined.");
                }

                if (!Equals(_value, value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public static implicit operator T(Property<T> property) => property.Value;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            foreach (var dep in _dependencies)
            {
                dep.PropertyChanged -= OnDependencyChanged;
            }
            _dependencies.Clear();
        }
    }
}