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

        public static Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
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
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            Vector3 direction = Vector3.zero;

            if (mousePos.x < edgeBuffer) direction.x = -1;
            else if (mousePos.x > screenSize.x - edgeBuffer) direction.x = 1;

            if (mousePos.y < edgeBuffer) direction.y = -1;
            else if (mousePos.y > screenSize.y - edgeBuffer) direction.y = 1;

            if (direction != Vector3.zero)
            {
                _mainCamera.transform.position += direction.normalized * (cameraMoveSpeed * Time.deltaTime);
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

                Vector3 newCamPos = new Vector3(
                    focusTarget.position.x,
                    focusTarget.position.y,
                    _mainCamera.transform.position.z
                );
                _mainCamera.transform.position = newCamPos;
            }
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f && !PanelUIRoot.Instance.isPanelOpen)
            {
                float newSize = _mainCamera.orthographicSize - scroll * zoomSensitivity;
                newSize = Mathf.Clamp(newSize, minOrthographicSize, maxOrthographicSize);
                _mainCamera.orthographicSize = newSize;
            }
        }
    }
}