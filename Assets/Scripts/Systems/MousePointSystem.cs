using System;
using UnityEngine;

namespace Systems
{
    [Obsolete("Obsolete")]
    public class MousePointSystem : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Canvas _canvas;
        public GameObject mousePointIndicator;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();

            // 隐藏光标
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            FollowMouseOnScreen();
            MousePointIndicatorControl();
        }

        // 跟随鼠标指针
        private void FollowMouseOnScreen()
        {
            if (_rectTransform == null || _canvas == null)
                return;

            Vector2 mousePos = Input.mousePosition;
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    mousePos,
                    null, 
                    out localPoint))
            {
                _rectTransform.anchoredPosition = localPoint;
            }
        }
        
        // 鼠标指示器控制
        private void MousePointIndicatorControl()
        {
            // 获取鼠标位置
            var mouseWorld = (Vector2)CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePointIndicator.transform.position = mouseWorld;

            if (Input.GetMouseButtonDown(1))
            {
                var obj = Instantiate(mousePointIndicator, mousePointIndicator.transform.parent).GetComponent<ParticleSystem>();
                obj.Play();
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    obj.startColor = Color.red;
                }
                else
                {
                    obj.startColor = Color.green;
                }
            }
        }
    }
}