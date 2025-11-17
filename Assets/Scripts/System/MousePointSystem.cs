using UnityEngine;

public class MousePointSystem : MonoBehaviour
{
    public Transform focusTarget;

    [Header("摄像机边缘拖动设置")]
    public float cameraMoveSpeed = 10f;
    public float edgeBuffer = 50f;

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        FollowMouseWithinScreen();
        PanCameraAtEdges();
        HandleInstantFocus();
    }

    void FollowMouseWithinScreen()
    {
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, _mainCamera.nearClipPlane)
        );
        mouseWorldPos.z = transform.position.z; // 保持原有 Z

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
}