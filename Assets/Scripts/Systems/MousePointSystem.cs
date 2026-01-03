using Components.UI;
using Managers;
using UnityEngine;

namespace Systems
{
    public class MousePointSystem : MonoBehaviour
    {
        public RectTransform _rectTransform;
        private Canvas _canvas;

        public void Initialize()
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
            MouseClickEffectControl();
        }

        // 跟随鼠标指针
        private void FollowMouseOnScreen()
        {
            if (_rectTransform == null || _canvas == null)
                return;

            Vector2 mousePos = Input.mousePosition;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    mousePos,
                    null, 
                    out var localPoint))
            {
                _rectTransform.anchoredPosition = localPoint;
            }
        }
        
        // 鼠标指示器控制
        private void MouseClickEffectControl()
        {
            // 获取鼠标位置
            var mouseWorld = (Vector2)CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            if(PanelUIRoot.Instance.isPanelOpen) return;

            if (Input.GetMouseButtonDown(1))
            {
                var effect = EffectManager.Instance.CreateEffect("MouseClick", false);
                effect.effect.transform.position = mouseWorld;
                
                var main = effect.effect.GetComponent<ParticleSystem>().main;
                main.startColor = Input.GetKey(KeyCode.LeftShift) ? Color.red : Color.green;
                
                effect.effect.SetActive(true);
            }
        }
    }
}