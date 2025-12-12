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
        private ShopSystem shopSystem;
        public bool playerCanInteractGame => !isShopOpen;
        private float formerTimeScale = 1;

        private void Start()
        {
            Instance = this;
            isShopOpen = false;
            shopSystem = transform.Find("ShopPanel").GetComponent<ShopSystem>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P) && !isShopOpen)
            {
                shopSystem.OpenShopPanel();
            }

            if (Input.GetKeyDown(KeyCode.Escape) && isShopOpen)
            {
                shopSystem.CloseShopPanel();
            }

            if (isShopOpen)
            {
                formerTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = formerTimeScale;
            }
        }
    }
}