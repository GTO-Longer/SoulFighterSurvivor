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
        /// 检测obj范围内是否有与指定tag不同的其他碰撞体，并返回最近的一个Entity
        /// </summary>
        public static bool IsOverlappingOtherTag(GameObject obj, string tag, out Entity entity , float radius = 0)
        {
            var collider = obj.GetComponent<CircleCollider2D>();
            entity = null;
            if (collider == null) return false;
            
            var _overlapColliders = new Collider2D[20];

            Vector2 center = collider.bounds.center;
            
            if (radius == 0)
            {
                radius = collider.radius * collider.transform.lossyScale.x;
            }

            // 获取所有重叠的碰撞箱
            var count = Physics2D.OverlapCircleNonAlloc(center, radius, _overlapColliders);
            var nearestDistanceSqr = float.MaxValue;
            for (var i = 0; i < count; i++)
            {
                var col = _overlapColliders[i];
                if (col == null) continue;

                var otherGo = col.gameObject;
                
                // 跳过没有Tag的对象
                if (otherGo.CompareTag("Untagged")) continue;

                // 跳过相同Tag的对象
                if (otherGo.CompareTag(tag)) continue;

                // 获取EntityData，没有则跳过
                var entityData = otherGo.GetComponent<EntityData>();
                if (entityData == null) continue;

                // 计算距离平方
                var distSqr = (otherGo.transform.position - (Vector3)center).sqrMagnitude;
                if (distSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distSqr;
                    entity = entityData.entity;
                }
            }

            return entity != null;
        }
        
        /// <summary>
        /// 检测圆形范围内是否目标Entity
        /// </summary>
        public static bool IsOverlappingTarget(GameObject obj, GameObject target, float radius = 0)
        {
            var collider = obj.GetComponent<CircleCollider2D>();
            if (collider == null) return false;
            
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

                // 获取EntityData，没有则跳过
                var entityData = otherGo.GetComponent<EntityData>();
                if (entityData == null) continue;

                // 判断实例id是否相同
                if (otherGo.GetInstanceID() == target.GetInstanceID())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取范围内的所有Entity
        /// </summary>
        public static Entity[] IsOverlappingAll(GameObject obj, float radius = 0)
        {
            var overlappingEntities = new List<(Entity entity, float distanceSqr)>();
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
        /// 获取范围内tag不同的所有Entity
        /// </summary>
        public static Entity[] IsOverlappingWithOtherTagAll(GameObject obj, float radius = 0, string tag = "")
        {
            var overlappingEntities = new List<(Entity entity, float distanceSqr)>();
            if(tag == "")
            {
                tag = obj.tag;
            }
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
                if (otherGo.CompareTag(tag)) continue;

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
        /// 获取范围内tag相同的所有Entity
        /// </summary>
        public static Entity[] IsOverlappingWithTagAll(GameObject obj, float radius = 0, string tag = "")
        {
            var overlappingEntities = new List<(Entity entity, float distanceSqr)>();
            if(tag == "")
            {
                tag = obj.tag;
            }
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

                // 跳过不同Tag的对象
                if (!otherGo.CompareTag(tag)) continue;

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
                if (result.collider.gameObject.CompareTag("Untagged")) continue;
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
            return objects.Count > 0;
        }
        
        /// <summary>
        /// 从列表中选出n个不同的元素
        /// </summary>
        public static bool GetRandomUniqueItems<T>(List<T> list, int count, out List<T> result)
        {
            result = new List<T>();
            if (list == null || list.Count < count) 
                return false;

            count = Mathf.Min(count, list.Count);

            var tempList = new List<T>(list);

            for (var i = 0; i < count; i++)
            {
                var index = Random.Range(0, tempList.Count);
                result.Add(tempList[index]);
                tempList.RemoveAt(index);
            }

            return true;
        }

        /// <summary>
        /// 从字典中选出n个不同的键值对
        /// </summary>
        public static bool GetRandomUniqueItems<TKey, TValue>(Dictionary<TKey, TValue> source, int count, out Dictionary<TKey, TValue> result)
        {
            result = new Dictionary<TKey, TValue>();
            if (source == null)
                return false;

            if (count <= 0)
                return false;

            if (count > source.Count)
                return false;

            var rng = new System.Random();

            // 随机打乱键并取前count个
            var selectedKeys = source.Keys.OrderBy(_ => rng.Next()).Take(count);

            result = new Dictionary<TKey, TValue>();
            foreach (var key in selectedKeys)
            {
                result.Add(key, source[key]);
            }

            return true;
        }
        
        /// <summary>
        /// 判断敌人是否在扇形范围
        /// </summary>
        public static bool IsOverlappingInSector(float sectorAngle, float sectorRadius, Vector2 direction, GameObject obj, GameObject target)
        {
            var targetDirection = (target.transform.position - obj.transform.position).normalized;
            // 点乘积结果
            var dot = Vector3.Dot(targetDirection, direction);
            // 反余弦计算角度
            var offsetAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            
            return offsetAngle < sectorAngle * .5f && Vector2.Distance(obj.transform.position, target.transform.position) < sectorRadius;
        }
        
        /// <summary>
        /// 判断扇形范围内是否有敌人
        /// </summary>
        public static bool IsOverlappingInSectorAll(float sectorAngle, float sectorRadius, Vector2 direction, GameObject obj, out List<Entity> targets)
        {
            targets = new List<Entity>();
            var rawTargets = IsOverlappingWithOtherTagAll(obj, sectorRadius);
            if (rawTargets == null) return false;

            foreach (var target in rawTargets)
            {
                if (IsOverlappingInSector(sectorAngle, sectorRadius, direction, obj, target.gameObject))
                {
                    targets.Add(target);
                }
            }
            
            return targets.Count > 0;
        }

        /// <summary>
        /// 判断盒状碰撞箱内是否有敌人
        /// </summary>
        public static bool IsOverlappingInBoxColliderAll(GameObject obj, out List<Entity> targets)
        {
            targets = new List<Entity>();
            var filter = new ContactFilter2D();
            filter.useTriggers = true;
            var results = new Collider2D[10];
            var count = obj.GetComponent<BoxCollider2D>().OverlapCollider(filter, results);
            if (count <= 0) return false;
            
            for (var index = 0; index < count; index ++)
            {
                var result = results[index];
                var entityData = result.GetComponent<EntityData>();
                if (entityData == null) continue;
                
                targets.Add(entityData.entity);
            }
            
            return targets.Count > 0;
        }
    }
}