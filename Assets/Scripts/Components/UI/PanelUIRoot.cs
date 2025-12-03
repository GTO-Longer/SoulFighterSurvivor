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

        private void Start()
        {
            Instance = this;
            isShopOpen = false;
            shopSystem = transform.Find("ShopBackground").GetComponent<ShopSystem>();
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
        }
    }
}