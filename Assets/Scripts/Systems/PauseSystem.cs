using Components.UI;
using Managers;
using UnityEngine;

namespace Systems
{
    public class PauseSystem : MonoBehaviour
    {
        public static PauseSystem Instance;
        private CanvasGroup _canvasGroup;

        public void Initialize()
        {
            Instance = this;
            _canvasGroup = GetComponent<CanvasGroup>();
            
            ContinueGame();
        }

        public void PauseGame()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            
            AudioManager.Instance.PauseAll();
            
            PanelUIRoot.Instance.isPauseOpen = true;
        }

        public void ContinueGame()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.UnPauseAll();
            }

            PanelUIRoot.Instance.isPauseOpen = false;
        }

        public void ExitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}