using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class Async : MonoBehaviour
    {
        public static Async Instance;
        private static Dictionary<Image, TweenerCore<float, float, FloatOptions>> imageTweeners = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> SetAsync(float duration, Transform target = null, TweenCallback update = null, TweenCallback complete = null)
        {
            var transform = target == null ? Instance.transform : target;
            return transform.DOMoveX(transform.position.x, duration).OnUpdate(update).OnComplete(complete).SetUpdate(UpdateType.Fixed, true);
        }
        
        public static void SetFillAmountAsync(Image image, float targetValue, float duration, TweenCallback complete = null)
        {
            if (imageTweeners.TryGetValue(image, out var tweener) && tweener != null)
            {
                tweener.Kill();
            }

            imageTweeners[image] = DOTween
            .To(() => image.fillAmount, x => image.fillAmount = x, targetValue, duration)
            .SetUpdate(true)
            .SetLink(image.gameObject)
            .OnComplete(() =>
            {
                complete?.Invoke();
                imageTweeners.Remove(image);
            })
            .OnKill(() =>
            {
                imageTweeners.Remove(image);
            });
        }
    }
}