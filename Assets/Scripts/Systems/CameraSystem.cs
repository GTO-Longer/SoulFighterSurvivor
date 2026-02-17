using Components.UI;
using UnityEngine;

namespace Systems
{
    public class CameraSystem : MonoBehaviour
    {
        public Transform focusTarget;

        [Header("摄像机边缘拖动设置")]
        private const float cameraMoveSpeed = 1000f;
        private const float edgeBuffer = 50f;

        [Header("摄像机缩放设置")]
        private const float zoomSensitivity = 300f;
        private const float minOrthographicSize = 500f;
        private const float maxOrthographicSize = 1000f;

        [Header("跟随设置")]
        public bool isFollowing;
        public float followSpeed = 5f;

        [Header("摄像机边界设置")]
        public string boundaryObjectName = "CameraBoundary";
        private BoxCollider2D _cameraBoundary;

        public static Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;

            // 查找摄像机边界对象
            var boundaryObj = GameObject.Find(boundaryObjectName);
            if (boundaryObj != null)
            {
                _cameraBoundary = boundaryObj.GetComponent<BoxCollider2D>();
                if (_cameraBoundary == null)
                {
                    Debug.LogWarning($"CameraBoundary对象 '{boundaryObjectName}' 没有BoxCollider2D组件！");
                }
                else
                {
                    Debug.Log($"已找到摄像机边界: {boundaryObjectName}, 边界大小: {_cameraBoundary.bounds.size}");
                }
            }
            else
            {
                Debug.LogWarning($"未找到摄像机边界对象 '{boundaryObjectName}'！摄像机将不会受限。");
            }

            // 初始限制摄像机位置
            ClampCameraToBoundary();
        }

        private void Update()
        {
            HandleFollowToggle();
            PanCameraAtEdges();
            HandleInstantFocus();
            HandleZoom();
        }

        private void HandleFollowToggle()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                isFollowing = !isFollowing;
            }
        }

        private void PanCameraAtEdges()
        {
            if (isFollowing) return;

            Vector2 mousePos = Input.mousePosition;
            var screenSize = new Vector2(Screen.width, Screen.height);

            var direction = Vector3.zero;

            if (mousePos.x < edgeBuffer) direction.x = -1;
            else if (mousePos.x > screenSize.x - edgeBuffer) direction.x = 1;

            if (mousePos.y < edgeBuffer) direction.y = -1;
            else if (mousePos.y > screenSize.y - edgeBuffer) direction.y = 1;

            if (direction != Vector3.zero)
            {
                _mainCamera.transform.position += direction.normalized * (cameraMoveSpeed * Time.deltaTime);
                ClampCameraToBoundary();
            }
        }

        private void HandleInstantFocus()
        {
            if (Input.GetKey(KeyCode.Space) || isFollowing)
            {
                if (focusTarget == null)
                {
                    Debug.LogWarning("未设置 Focus Target，无法聚焦！");
                    return;
                }

                var newCamPos = new Vector3(
                    focusTarget.position.x,
                    focusTarget.position.y,
                    _mainCamera.transform.position.z
                );

                // 临时设置摄像机位置以计算边界限制
                _mainCamera.transform.position = newCamPos;
                ClampCameraToBoundary();
            }
        }

        private void HandleZoom()
        {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f && !PanelUIRoot.Instance.isPanelOpen)
            {
                var newSize = _mainCamera.orthographicSize - scroll * zoomSensitivity;
                newSize = Mathf.Clamp(newSize, minOrthographicSize, maxOrthographicSize);
                _mainCamera.orthographicSize = newSize;

                // 缩放后需要重新限制摄像机位置
                ClampCameraToBoundary();
            }
        }

        /// <summary>
        /// 将摄像机位置限制在边界内
        /// </summary>
        private void ClampCameraToBoundary()
        {
            if (_cameraBoundary == null || _mainCamera == null) return;

            // 获取摄像机视野的半高和半宽
            var orthographicSize = _mainCamera.orthographicSize;
            var aspectRatio = (float)Screen.width / Screen.height;
            var cameraHalfHeight = orthographicSize;
            var cameraHalfWidth = cameraHalfHeight * aspectRatio;

            // 获取边界范围
            var bounds = _cameraBoundary.bounds;
            var minX = bounds.min.x + cameraHalfWidth;
            var maxX = bounds.max.x - cameraHalfWidth;
            var minY = bounds.min.y + cameraHalfHeight;
            var maxY = bounds.max.y - cameraHalfHeight;

            // 如果边界太小无法容纳摄像机视野，则让摄像机居中
            if (minX > maxX)
            {
                minX = maxX = bounds.center.x;
            }
            if (minY > maxY)
            {
                minY = maxY = bounds.center.y;
            }

            // 限制摄像机位置
            var cameraPos = _mainCamera.transform.position;
            cameraPos.x = Mathf.Clamp(cameraPos.x, minX, maxX);
            cameraPos.y = Mathf.Clamp(cameraPos.y, minY, maxY);
            _mainCamera.transform.position = cameraPos;
        }
    }
}