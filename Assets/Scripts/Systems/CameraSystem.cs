using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
    public class CameraSystem : MonoBehaviour
    {
        public Transform focusTarget;

        [Header("摄像机边缘拖动设置")] public float cameraMoveSpeed = 10f;
        public float edgeBuffer = 50f;

        [Header("摄像机缩放设置")] private const float zoomSensitivity = 1f;
        private const float minOrthographicSize = 5f;
        private const float maxOrthographicSize = 7.5f;

        private Camera _mainCamera;

        void Start()
        {
            _mainCamera = Camera.main;
        }

        void Update()
        {
            PanCameraAtEdges();
            HandleInstantFocus();
            HandleZoom();
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

                Vector3 newCamPos = new Vector3(
                    focusTarget.position.x,
                    focusTarget.position.y,
                    _mainCamera.transform.position.z
                );
                _mainCamera.transform.position = newCamPos;
            }
        }

        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                float newSize = _mainCamera.orthographicSize - scroll * zoomSensitivity;
                newSize = Mathf.Clamp(newSize, minOrthographicSize, maxOrthographicSize);
                _mainCamera.orthographicSize = newSize;
            }
        }
    }
}
