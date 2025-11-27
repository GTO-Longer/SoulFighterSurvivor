using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Factories
{
    public class ScreenTextFactory : MonoBehaviour
    {
        public GameObject screenTextPrefab;
        public static ScreenTextFactory Instance { get; private set; }
        private ObjectPool<RectTransform> _screenTextPool;
        private List<RectTransform> _activeTexts = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                screenTextPrefab.SetActive(false);

                _screenTextPool = new ObjectPool<RectTransform>(
                    createFunc: () =>
                    {
                        var instance = Instantiate(screenTextPrefab, screenTextPrefab.transform.parent).GetComponent<RectTransform>();
                        instance.gameObject.SetActive(false);
                        return instance;
                    },
                    actionOnGet: text => text.gameObject.SetActive(true),
                    actionOnRelease: text => text.gameObject.SetActive(false),
                    actionOnDestroy: text => Destroy(text.gameObject),
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
        /// 在指定位置创建ScreenText
        /// </summary>
        public void Spawn(Vector2 position, string content, float duration, float moveRange, Color? color = null)
        {
            var actualColor = color ??= Color.red;
            var text = _screenTextPool.Get();
            _activeTexts.Add(text);
            text.GetComponent<TMP_Text>().text = content;
            text.GetComponent<TMP_Text>().color = actualColor;
            
            text.anchoredPosition = position;
            text.DOMoveX(gameObject.transform.position.x + (Random.Range(0, 1) * 2 - 1) * moveRange, duration);
            text.DOMoveY(gameObject.transform.position.y + moveRange, duration / 2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                text.DOMoveY(gameObject.transform.position.y, duration / 2f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    Despawn(text);
                });
            });
        }

        /// <summary>
        /// 回收所有ScreenText
        /// </summary>
        private void DespawnAll()
        {
            foreach (var text in _activeTexts.ToList())
            {
                Despawn(text);
            }
        }

        /// <summary>
        /// 回收对应ScreenText
        /// </summary>
        /// <param name="text"></param>
        private void Despawn(RectTransform text)
        {
            _screenTextPool.Release(text);
            _activeTexts.Remove(text);
        }

        private void OnDestroy()
        {
            DespawnAll();
            _screenTextPool?.Dispose();
        }
    }
}