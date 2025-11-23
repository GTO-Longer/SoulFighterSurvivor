using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public static class Async
    {
        public static void SetAsync(Transform transform, TweenCallback update, float duration)
        {
            transform.DOMoveX(transform.position.x, duration).OnUpdate(update);
        }
    }
}