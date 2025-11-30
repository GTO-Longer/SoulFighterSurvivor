using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using MVVM;
using Classes;
using UnityEngine.UI;
using Utilities;

namespace Factories
{
    public class StateBarFactory : MonoBehaviour
    {
        public GameObject stateBarPrefab;
        public static StateBarFactory Instance { get; private set; }

        private ObjectPool<RectTransform> _stateBarPool;
        private Dictionary<Entity, RectTransform> _activeBars = new ();
        private Dictionary<Entity, Action> _unbindEvent = new ();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                stateBarPrefab.SetActive(false);

                _stateBarPool = new ObjectPool<RectTransform>(
                    createFunc: () =>
                    {
                        var instance = Instantiate(stateBarPrefab, stateBarPrefab.transform.parent).GetComponent<RectTransform>();
                        instance.gameObject.SetActive(false);
                        return instance;
                    },
                    actionOnGet: bar => bar.gameObject.SetActive(true),
                    actionOnRelease: bar => bar.gameObject.SetActive(false),
                    actionOnDestroy: bar => Destroy(bar.gameObject),
                    collectionCheck: true,
                    defaultCapacity: 10,
                    maxSize: 1000
                );
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 为指定Entity创建StateBar
        /// </summary>
        public void Spawn(Entity entity)
        {
            if (entity == null || entity.gameObject == null) return;

            if (_activeBars.TryGetValue(entity, out var existingBar))
            {
                existingBar.gameObject.SetActive(true);
                return;
            }

            var bar = _stateBarPool.Get();
            _unbindEvent.Add(entity, Binder.BindStateBar(bar, entity));
            _activeBars[entity] = bar;
        }

        /// <summary>
        /// 隐藏并回收指定Entity的StateBar
        /// </summary>
        public void Despawn(Entity entity)
        {
            if (entity == null) return;

            if (_activeBars.TryGetValue(entity, out var bar))
            {
                _unbindEvent[entity].Invoke();
                bar.gameObject.SetActive(false);
                _stateBarPool.Release(bar);
                _activeBars.Remove(entity);
            }
        }

        /// <summary>
        /// 回收所有StateBar
        /// </summary>
        private void DespawnAll()
        {
            foreach (var bar in _activeBars.Select(kvp => kvp.Value))
            {
                if (bar == null) continue;
                bar.gameObject.SetActive(false);
                _stateBarPool.Release(bar);
            }

            _activeBars.Clear();
        }

        /// <summary>
        /// 更新所有StateBar
        /// </summary>
        private void LateUpdate()
        {
            if (_activeBars.Count == 0) return;

            foreach (var (entity, bar) in _activeBars)
            {
                if (entity?.gameObject == null || bar == null) continue;
                bar.anchoredPosition = new Vector2(entity.gameObject.transform.position.x - 100, entity.gameObject.transform.position.y + 200);
                bar.Find("HPBarBackground/HPBar").GetComponent<Image>().color = entity.team == Team.Hero ? Color.green : Color.red;
            }
        }

        private void OnDestroy()
        {
            DespawnAll();
            _stateBarPool?.Dispose();
        }
    }
}