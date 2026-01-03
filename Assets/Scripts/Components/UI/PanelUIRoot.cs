using DataManagement;
using Systems;
using UnityEngine;

namespace Components.UI
{
    public class PanelUIRoot : MonoBehaviour
    {
        public static PanelUIRoot Instance;
        
        public bool isShopOpen;
        public bool isChoiceOpen;
        public bool isPauseOpen;
        
        public bool isPanelOpen => isShopOpen || isChoiceOpen || isPauseOpen;
        private float formerTimeScale;
        
        [HideInInspector] public ShopSystem shopSystem;
        [HideInInspector] public ChoiceSystem choiceSystem;
        [HideInInspector] public PauseSystem pauseSystem;
        [HideInInspector] public MousePointSystem mousePointer;

        private void Awake()
        {
            Instance = this;
            
            shopSystem = ResourceReader.LoadPrefab("UI/ShopPanel", transform).GetComponent<ShopSystem>();
            choiceSystem = ResourceReader.LoadPrefab("UI/ChoicePanel", transform).GetComponent<ChoiceSystem>();
            pauseSystem = ResourceReader.LoadPrefab("UI/PausePanel", transform).GetComponent<PauseSystem>();
            mousePointer = ResourceReader.LoadPrefab("UI/MousePointer", transform).GetComponent<MousePointSystem>();
        }

        private void Start()
        {
            shopSystem.Initialize();
            choiceSystem.Initialize();
            pauseSystem.Initialize();
            mousePointer.Initialize();
            
            mousePointer.transform.SetSiblingIndex(0);
            shopSystem.transform.SetSiblingIndex(0);
            choiceSystem.transform.SetSiblingIndex(0);
            pauseSystem.transform.SetSiblingIndex(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !isPanelOpen)
            {
                shopSystem.OpenShopPanel();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isShopOpen)
            {
                shopSystem.CloseShopPanel();
            }else if (Input.GetKeyDown(KeyCode.Escape) && !isPanelOpen)
            {
                pauseSystem.PauseGame();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isPauseOpen)
            {
                pauseSystem.ContinueGame();
            }

            if (isPanelOpen)
            {
                if (formerTimeScale == 0)
                {
                    formerTimeScale = Time.timeScale;
                    Time.timeScale = 0;
                }
            }
            else
            {
                if (formerTimeScale != 0)
                {
                    Time.timeScale = formerTimeScale;
                    formerTimeScale = 0;
                }
            }
        }
    }
}