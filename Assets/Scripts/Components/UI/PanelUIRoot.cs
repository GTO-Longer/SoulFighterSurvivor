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
        public bool isPanelOpen => isShopOpen || isChoiceOpen;
        private float formerTimeScale = 0;
        
        [HideInInspector]public ShopSystem shopSystem;
        [HideInInspector]public ChoiceSystem choiceSystem;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            shopSystem = ResourceReader.LoadPrefab("UI/ShopPanel", transform).GetComponent<ShopSystem>();
            choiceSystem = ResourceReader.LoadPrefab("UI/ChoicePanel", transform).GetComponent<ChoiceSystem>();

            shopSystem.Initialize();
            choiceSystem.Initialize();
            shopSystem.transform.SetSiblingIndex(0);
            choiceSystem.transform.SetSiblingIndex(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !isPanelOpen)
            {
                shopSystem.OpenShopPanel();
            }

            if (Input.GetKeyDown(KeyCode.Escape) && isShopOpen)
            {
                shopSystem.CloseShopPanel();
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