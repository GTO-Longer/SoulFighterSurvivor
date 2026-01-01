using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ParallelogramGenerator : MonoBehaviour
{
    private Vector2 baseStart = new(-0.5f, 0);
    private Vector2 baseEnd = new(0.5f, 0);

    private float sideLength = 1f;
    public float rotationAngle = 90f;

    private MeshFilter meshFilter;
    private Mesh mesh;

    void Start()
    {
        GenerateParallelogram();
    }

    void OnValidate()
    {
        if (meshFilter != null) GenerateParallelogram();
    }

    public void GenerateParallelogram()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = "Effect";
        meshRenderer.sortingOrder = 0;

        // 1. 计算四个顶点
        // p0: 底边起点, p1: 底边终点
        Vector3 p0 = baseStart;
        Vector3 p1 = baseEnd;

        // 计算底边向量和方向
        Vector2 baseDir = (baseEnd - baseStart).normalized;
        
        // 计算斜边的偏移向量
        // 使用极坐标转换：x = r * cos(theta), y = r * sin(theta)
        // 注意：这里的角度需要转为弧度
        float rad = rotationAngle * Mathf.Deg2Rad;
        Vector2 sideOffset = new Vector2(
            Mathf.Cos(rad) * sideLength,
            Mathf.Sin(rad) * sideLength
        );

        // 如果底边不是水平的，需要根据底边的旋转调整偏移（这里简化处理，假设角度是相对于世界坐标或底边起点的偏移）
        // 顶点顺序：左下 -> 右下 -> 左上 -> 右上
        Vector3 p2 = (Vector2)p0 + sideOffset; // 左上
        Vector3 p3 = (Vector2)p1 + sideOffset; // 右上

        // 2. 赋值顶点
        Vector3[] vertices = new Vector3[] { p0, p1, p2, p3 };

        // 3. 定义三角形（两个三角形组成一个平行四边形）
        // 索引顺序：0-2-1 和 2-3-1 (顺时针)
        int[] triangles = new int[] {
            0, 2, 1,
            2, 3, 1
        };

        // 4. 定义UV（为了让Shader正常显示，映射到0-1空间）
        Vector2[] uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        // 5. 更新网格
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals(); // 重新计算法线以支持光照
    }
}