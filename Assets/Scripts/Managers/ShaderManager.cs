using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ShaderManager : MonoBehaviour
    {
        public static ShaderManager Instance;
        private List<Material> materialList = new List<Material>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Update()
        {
            foreach (var material in materialList)
            {
                material.SetFloat("_UnscaledTime", Time.unscaledTime);
            }
        }

        public void AddMaterial(Material material)
        {
            materialList.Add(material);
        }

        public void RemoveMaterial(Material material)
        {
            material.SetFloat("_UnscaledTime", 0);
            materialList.Remove(material);
        }

        public void RemoveAllMaterial()
        {
            foreach (var material in materialList)
            {
                material.SetFloat("_UnscaledTime", 0);
            }
            materialList.Clear();
        }

        private void OnDestroy()
        {
            RemoveAllMaterial();
        }
    }
}