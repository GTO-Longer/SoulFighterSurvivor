using System;
using Classes;
using DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MVVM.ViewModels
{
    public class HexInfoViewModel : MonoBehaviour
    {
        public static HexInfoViewModel Instance;
        
        private TMP_Text hexName;
        private TMP_Text hexDescription;
        private Image hexIcon;

        private void Start()
        {
            Instance = this;
            hexName = transform.Find("Background/TitleBanner/HexName").GetComponent<TMP_Text>();
            hexDescription = transform.Find("Background/HexDescription").GetComponent<TMP_Text>();
            hexIcon = transform.Find("Background/TitleBanner/HexIcon").GetComponent<Image>();
            HideHexInfo();
        }

        public void ShowHexInfo(Hex hex)
        {
            gameObject.SetActive(true);

            hexName.text = hex.hexName;
            hexDescription.text = hex.GetHexDetail(out var detail) ? detail : "";
            hexIcon.sprite = hex.hexIcon;
            hexIcon.material = hex.hexQuality switch
            {
                Quality.None => null,
                Quality.Silver => null,
                Quality.Gold => ResourceReader.LoadMaterial("Gold"),
                Quality.Prismatic => ResourceReader.LoadMaterial("Prismatic"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void HideHexInfo()
        {
            gameObject.SetActive(false);
        }
    }
}
