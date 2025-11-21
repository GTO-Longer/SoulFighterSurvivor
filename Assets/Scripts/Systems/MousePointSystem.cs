using UnityEngine;

namespace Systems
{
    public class MousePointSystem : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Canvas _canvas;

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
        }
    
        void FollowMouseOnScreen()
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
    }
}