using System.Collections;
using System.Collections.Generic;
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
        private ShopSystem shopSystem;
        private float formerTimeScale = 0;

        private void Start()
        {
            Instance = this;
            isShopOpen = false;
            isChoiceOpen = false;
            shopSystem = transform.Find("ShopPanel").GetComponent<ShopSystem>();
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