using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public class Async : MonoBehaviour
    {
        public static Async Instance;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public static void SetAsync(float duration, Transform target = null, TweenCallback update = null, TweenCallback complete = null)
        {
            var transform = target == null ? Instance.transform : target;
            transform.DOMoveX(transform.position.x, duration).OnUpdate(update).OnComplete(complete).SetUpdate(UpdateType.Fixed);
        }
    }
}