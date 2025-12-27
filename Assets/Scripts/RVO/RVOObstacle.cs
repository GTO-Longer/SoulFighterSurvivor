using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace RVO
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class RVOObstacle : MonoBehaviour
    {
        private List<float2> verts;

        private void Start()
        {
            // 获取顶点
            verts = GetVertices();

            if (verts == null || verts.Count < 3)
            {
                Debug.LogWarning($"[RVOObstacle] Invalid polygon on {name}");
                return;
            }

            // 注册到 RVO
            var sim = RVOManager.Instance.simulator;
            sim.AddObstacle(verts);
        }

        private List<float2> GetVertices()
        {
            var poly = GetComponent<PolygonCollider2D>();
            var original = new List<float2>();

            foreach (var p in poly.points)
            {
                Vector2 wp = transform.TransformPoint(p);
                original.Add(new float2(wp.x, wp.y));
            }

            // 修正为逆时针
            if (!IsCounterClockwise(original))
            {
                original.Reverse();
            }

            return original;
        }

        private bool IsCounterClockwise(List<float2> verts)
        {
            var sum = 0f;
            for (var i = 0; i < verts.Count; i++)
            {
                var a = verts[i];
                var b = verts[(i + 1) % verts.Count];
                sum += (b.x - a.x) * (b.y + a.y);
            }
            return sum < 0f;
        }
    }
}
