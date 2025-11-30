using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace RVO
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class RVOObstacle : MonoBehaviour
    {
        // 内缩偏移量
        private const float inwardOffset = 20f;

        // 用于绘制 Gizmo
        private List<float2> originalVerts;   // 原始顶点（世界坐标）
        private List<float2> offsetVerts;     // 缩进去后的顶点（世界坐标）

        private void Start()
        {
            // 获取顶点+缩进去
            offsetVerts = GetOffsetVertices(out originalVerts);

            if (offsetVerts == null || offsetVerts.Count < 3)
            {
                Debug.LogWarning($"[RVOObstacle] Invalid polygon on {name}");
                return;
            }

            // 注册到 RVO
            var sim = RVOManager.Instance.simulator;
            sim.AddObstacle(offsetVerts);
        }

        private List<float2> GetOffsetVertices(out List<float2> original)
        {
            var poly = GetComponent<PolygonCollider2D>();
            original = new List<float2>();

            foreach (var p in poly.points)
            {
                Vector2 wp = transform.TransformPoint(p);
                original.Add(new float2(wp.x, wp.y));
            }

            // 向内缩
            var offsetPolygon = OffsetPolygonInward(original, inwardOffset);

            // 修正为逆时针
            if (!IsCounterClockwise(offsetPolygon))
                offsetPolygon.Reverse();

            return offsetPolygon;
        }

        private bool IsCounterClockwise(List<float2> verts)
        {
            float sum = 0f;
            for (int i = 0; i < verts.Count; i++)
            {
                float2 a = verts[i];
                float2 b = verts[(i + 1) % verts.Count];
                sum += (b.x - a.x) * (b.y + a.y);
            }
            return sum < 0f;
        }

        private List<float2> OffsetPolygonInward(List<float2> verts, float offset)
        {
            int n = verts.Count;
            var result = new List<float2>(n);

            for (int i = 0; i < n; i++)
            {
                float2 prev = verts[(i - 1 + n) % n];
                float2 curr = verts[i];
                float2 next = verts[(i + 1) % n];

                float2 dirA = math.normalize(curr - prev);
                float2 dirB = math.normalize(next - curr);

                float2 normalA = new float2(-dirA.y, dirA.x);
                float2 normalB = new float2(-dirB.y, dirB.x);

                float2 bisector = math.normalize(normalA + normalB);

                float dot = math.dot(bisector, normalA);
                if (math.abs(dot) < 0.01f)
                    dot = 0.01f;

                float move = offset / dot;

                result.Add(curr + bisector * move);
            }

            return result;
        }

        private void OnDrawGizmos()
        {
            var poly = GetComponent<PolygonCollider2D>();
            if (poly == null) return;

            // 1. 绘制原始多边形（白色）
            Gizmos.color = Color.white;
            DrawPolygonGizmo(poly, false);

            // 2. 绘制偏移后多边形（黄色）
            Gizmos.color = Color.yellow;
            DrawOffsetPolygonGizmo();
        }

        private void DrawPolygonGizmo(PolygonCollider2D poly, bool isOffset)
        {
            var points = poly.points;
            int n = points.Length;

            for (int i = 0; i < n; i++)
            {
                Vector2 a = transform.TransformPoint(points[i]);
                Vector2 b = transform.TransformPoint(points[(i + 1) % n]);
                Gizmos.DrawLine(a, b);
            }
        }

        private void DrawOffsetPolygonGizmo()
        {
            if (offsetVerts == null || offsetVerts.Count < 2)
                return;

            for (int i = 0; i < offsetVerts.Count; i++)
            {
                float2 a = offsetVerts[i];
                float2 b = offsetVerts[(i + 1) % offsetVerts.Count];
                Gizmos.DrawLine(new Vector3(a.x, a.y, 0), new Vector3(b.x, b.y, 0));
            }
        }
    }
}
