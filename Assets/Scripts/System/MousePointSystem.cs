using UnityEngine;

public class MousePointSystem : MonoBehaviour
{
    public Transform focusTarget;

    [Header("摄像机边缘拖动设置")]
    public float cameraMoveSpeed = 10f;
    public float edgeBuffer = 50f;

    [Header("摄像机缩放设置")]
    public float zoomSensitivity = 1f;        // 滚轮灵敏度
    public float minOrthographicSize = 5f;    // 最小缩放（视野最大）
    public float maxOrthographicSize = 10f;   // 最大缩放（视野最小）

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;

        // 确保是正交相机
        if (!_mainCamera.orthographic)
        {
            Debug.LogWarning("MousePointSystem 要求 Camera 为 Orthographic 模式！");
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        FollowMouseWithinScreen();
        PanCameraAtEdges();
        HandleInstantFocus();
        HandleZoom(); // 新增：处理滚轮缩放
    }

    void FollowMouseWithinScreen()
    {
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCamera.nearClipPlane)
        );
        mouseWorldPos.z = transform.position.z;

        // 计算当前屏幕边界（世界坐标）
        float halfHeight = _mainCamera.orthographicSize;
        float halfWidth = halfHeight * _mainCamera.aspect;
        Vector3 camPos = _mainCamera.transform.position;

        float left = camPos.x - halfWidth;
        float right = camPos.x + halfWidth;
        float bottom = camPos.y - halfHeight;
        float top = camPos.y + halfHeight;

        // 限制自身在可视区域内
        transform.position = new Vector3(
            Mathf.Clamp(mouseWorldPos.x, left, right),
            Mathf.Clamp(mouseWorldPos.y, bottom, top),
            transform.position.z
        );
    }

    void PanCameraAtEdges()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        Vector3 direction = Vector3.zero;

        if (mousePos.x < edgeBuffer) direction.x = -1;
        else if (mousePos.x > screenSize.x - edgeBuffer) direction.x = 1;

        if (mousePos.y < edgeBuffer) direction.y = -1;
        else if (mousePos.y > screenSize.y - edgeBuffer) direction.y = 1;

        if (direction != Vector3.zero)
        {
            _mainCamera.transform.position += direction.normalized * cameraMoveSpeed * Time.deltaTime;
        }
    }

    void HandleInstantFocus()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (focusTarget == null)
            {
                Debug.LogWarning("未设置 Focus Target，无法聚焦！");
                return;
            }

            // 瞬间将摄像机移到 focusTarget 位置（保持原有 Z）
            Vector3 newCamPos = new Vector3(
                focusTarget.position.x,
                focusTarget.position.y,
                _mainCamera.transform.position.z
            );
            _mainCamera.transform.position = newCamPos;
        }
    }

    // 新增：处理鼠标滚轮缩放
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // 滚轮向上（scroll > 0）→ 缩小视野（减小 orthographicSize）
            // 滚轮向下（scroll < 0）→ 放大视野（增大 orthographicSize）
            float newSize = _mainCamera.orthographicSize - scroll * zoomSensitivity;

            // 限制在允许范围内
            newSize = Mathf.Clamp(newSize, minOrthographicSize, maxOrthographicSize);

            _mainCamera.orthographicSize = newSize;
        }
    }
}