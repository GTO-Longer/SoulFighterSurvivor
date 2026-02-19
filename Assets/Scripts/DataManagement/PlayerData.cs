using System;
using UnityEngine;

namespace DataManagement
{
    public class PlayerData : MonoBehaviour
    {
        public static PlayerData Instance;
        public string heroName;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(this);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}