using Classes;
using DataManagement;
using UnityEngine;

namespace Utilities
{
    public static class ToolFunctions
    {
        /// <summary>
        /// 检测圆形范围内是否有与指定tag不同的其他碰撞体，并返回最近的一个Entity
        /// </summary>
        public static Entity IsOverlappingOtherTag(GameObject obj, float radius = 0)
        {
            var excludeTag = obj.tag;
            var collider = obj.GetComponent<CircleCollider2D>();
            if (collider == null) return null;
            
            var _overlapColloders = new Collider2D[20];

            Vector2 center = collider.bounds.center;
            
            if (radius == 0)
            {
                radius = collider.radius * collider.transform.lossyScale.x;
            }

            // 获取所有重叠的碰撞箱
            var count = Physics2D.OverlapCircleNonAlloc(center, radius, _overlapColloders);

            Entity targetEntity = null;
            var nearestDistanceSqr = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var col = _overlapColloders[i];
                if (col == null) continue;

                var otherGo = col.gameObject;
                
                // 跳过没有Tag的对象
                if (otherGo.CompareTag("Untagged")) continue;

                // 跳过相同Tag的对象
                if (otherGo.CompareTag(excludeTag)) continue;

                // 获取EntityData，没有则跳过
                var entityData = otherGo.GetComponent<EntityData>();
                if (entityData == null) continue;

                // 计算距离平方
                var distSqr = (otherGo.transform.position - (Vector3)center).sqrMagnitude;
                if (distSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distSqr;
                    targetEntity = entityData.entity;
                }
            }

            return targetEntity;
        }
    }
}