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
        private TMP_Text hexName;
        private TMP_Text hexDescription;
        private Image hexIcon;
        private Image hexIconBorder;

        public void Initialize()
        {
            hexName = transform.Find("Background/TitleBanner/HexName").GetComponent<TMP_Text>();
            hexDescription = transform.Find("Background/HexDescription").GetComponent<TMP_Text>();
            hexIcon = transform.Find("Background/TitleBanner/HexIcon").GetComponent<Image>();
            hexIconBorder = transform.Find("Background/TitleBanner/HexIconBorder").GetComponent<Image>();
            HideHexInfo();
        }

        public void ShowHexInfo(Hex hex)
        {
            gameObject.SetActive(true);

            hexName.text = hex.hexName;
            hexDescription.text = hex.GetHexDetail(out var detail) ? detail : "";
            
            // 设置图标图片和颜色
            hexIcon.sprite = hex.hexIcon;
            hexIcon.material = hex.hexQuality switch
            {
                Quality.None => null,
                Quality.Silver => null,
                Quality.Gold => ResourceReader.LoadMaterial("Gold"),
                Quality.Prismatic => ResourceReader.LoadMaterial("Prismatic"),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            // 设置图标边框图片和颜色
            hexIconBorder.sprite = hex.hexIconBorder;
            switch (hex.hexQuality)
            {
                case Quality.None: hexIconBorder.gameObject.SetActive(false); break;
                case Quality.Silver: hexIconBorder.color = Colors.GetColor(Colors.SilverBorder, 0.25f); break;
                case Quality.Gold: hexIconBorder.color = Colors.GetColor(Colors.GoldBorder, 0.25f); break;
                case Quality.Prismatic: hexIconBorder.color = Colors.GetColor(Colors.PrismaticBorder, 0.25f); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // UI重新布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.Find("Background").GetComponent<RectTransform>());
        }

        public void HideHexInfo()
        {
            gameObject.SetActive(false);
        }
    }
}
