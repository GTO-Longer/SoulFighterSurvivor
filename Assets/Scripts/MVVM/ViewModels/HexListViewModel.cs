using System;
using System.Collections.Generic;
using Classes;
using Components.UI;
using DataManagement;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace MVVM.ViewModels
{
    public class HexListViewModel : MonoBehaviour
    {
        public static HexListViewModel Instance;
        [HideInInspector]public List<Transform> hexSlotList;
        private Action UnBindEvent;

        private void Start()
        {
            Instance = this;
            hexSlotList = new List<Transform>();

            // 绑定海克斯信息显示
            for (var index = 0; index < 6; index++)
            {
                var slotTransform = transform.GetChild(index);
                hexSlotList.Add(slotTransform);
                
                var button = slotTransform.GetComponent<HexSlotButton>();
                if (button != null)
                {
                    button.onPointerEnter = () =>
                    {
                        var current = button.GetComponent<HexData>().hex;
                        if (current != null)
                        {
                            HexInfoViewModel.Instance.ShowHexInfo(current);
                        }
                    };

                    button.onPointerExit = () =>
                    {
                        HexInfoViewModel.Instance.HideHexInfo();
                    };
                }
            }
        }

        /// <summary>
        /// 设置海克斯
        /// </summary>
        public void SetHex(int index, Hex hex)
        {
            hexSlotList[index].GetComponent<HexData>().hex = hex;
            hexSlotList[index].GetComponent<Image>().sprite = hex.hexIcon;
            hexSlotList[index].GetComponent<Image>().material = hex.hexQuality switch
            {
                Quality.None => null,
                Quality.Silver => null,
                Quality.Gold => ResourceReader.LoadMaterial("Gold"),
                Quality.Prismatic => ResourceReader.LoadMaterial("Prismatic"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
