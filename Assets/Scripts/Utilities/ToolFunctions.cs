using System.Collections.Generic;
using System.Linq;
using Classes;
using DataManagement;
using Systems;
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
            
            var _overlapColliders = new Collider2D[20];

            Vector2 center = collider.bounds.center;
            
            if (radius == 0)
            {
                radius = collider.radius * collider.transform.lossyScale.x;
            }

            // 获取所有重叠的碰撞箱
            var count = Physics2D.OverlapCircleNonAlloc(center, radius, _overlapColliders);

            Entity targetEntity = null;
            var nearestDistanceSqr = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var col = _overlapColliders[i];
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

        public static Entity[] IsOverlappingOtherTagAll(GameObject obj, float radius = 0)
        {
            var overlappingEntities = new List<(Entity entity, float distanceSqr)>();
            var excludeTag = obj.tag;
            var collider = obj.GetComponent<CircleCollider2D>();
            if (collider == null) return null;

            var _overlapColliders = new Collider2D[20];

            Vector2 center = collider.bounds.center;

            if (radius == 0)
            {
                radius = collider.radius * collider.transform.lossyScale.x;
            }

            // 获取所有重叠的碰撞箱
            var count = Physics2D.OverlapCircleNonAlloc(center, radius, _overlapColliders);

            for (var i = 0; i < count; i++)
            {
                var col = _overlapColliders[i];
                if (col == null) continue;

                var otherGo = col.gameObject;

                // 跳过没有Tag的对象
                if (otherGo.CompareTag("Untagged")) continue;

                // 跳过相同Tag的对象
                if (otherGo.CompareTag(excludeTag)) continue;

                // 获取EntityData，没有则跳过
                var entityData = otherGo.GetComponent<EntityData>();
                if (entityData == null) continue;
                
                var distSqr = (otherGo.transform.position - (Vector3)center).sqrMagnitude;
                overlappingEntities.Add((entityData.entity, distSqr));
            }

            if (overlappingEntities.Count == 0)
            {
                return null;
            }
            
            // 按距离平方升序排序
            overlappingEntities.Sort((a, b) => a.distanceSqr.CompareTo(b.distanceSqr));

            // 提取 Entity 数组并返回
            return overlappingEntities.Select(item => item.entity).ToArray();
        }

        /// <summary>
        /// 鼠标处是否有物体
        /// </summary>
        public static bool IsObjectAtMousePoint(out List<GameObject> objects, string tag = null, bool sameTag = false)
        {
            objects = new List<GameObject>();  
            Vector2 _mousePosition = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var results = new RaycastHit2D[10];
            var size = Physics2D.RaycastNonAlloc(_mousePosition, Vector2.zero, results);
            if (size <= 0)
            {
                return false;
            }
            
            foreach (var result in results)
            {
                if (result.collider == null) continue;
                if (tag == null)
                {
                    objects.Add(result.collider.gameObject);
                }
                else
                {
                    var obj = result.collider.gameObject;
                    if (obj.CompareTag(tag) == sameTag)
                    {
                        objects.Add(obj);
                    }
                }
            }
            return true;
        }
    }
}