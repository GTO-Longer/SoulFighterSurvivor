using System.Collections;
using System.Collections.Generic;
using MVVM.ViewModels;
using UnityEngine;

namespace Components.UI
{
    public class HUDUIRoot : MonoBehaviour
    {
        public static HUDUIRoot Instance;
        
        private TargetAttributeViewModel targetAttribute;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {

        }
    }
}